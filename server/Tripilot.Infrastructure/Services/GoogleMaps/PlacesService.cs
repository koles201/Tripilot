using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.GoogleMaps;
using Tripilot.Infrastructure.Configuration;

namespace Tripilot.Infrastructure.Services.GoogleMaps;

public class PlacesService : IPlacesService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GoogleMapsSettings _settings;
    private readonly ILogger<PlacesService> _logger;

    public PlacesService(
        IHttpClientFactory httpClientFactory,
        IOptions<GoogleMapsSettings> settings,
        ILogger<PlacesService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<PlaceDetailsResult?> GetPlaceDetailsAsync(string placeId, string? language = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_settings.Enabled)
            {
                _logger.LogWarning("Google Maps integration is disabled");
                return null;
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&key={_settings.ApiKey}";
            
            if (!string.IsNullOrEmpty(language))
                url += $"&language={language}";

            var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<PlaceDetailsApiResponse>(content);

            if (apiResponse?.Status != "OK" || apiResponse.Result == null)
            {
                _logger.LogWarning("Place details failed for place_id: {PlaceId}. Status: {Status}", placeId, apiResponse?.Status);
                return null;
            }

            var result = apiResponse.Result;
            return new PlaceDetailsResult
            {
                PlaceId = result.PlaceId ?? string.Empty,
                Name = result.Name ?? string.Empty,
                FormattedAddress = result.FormattedAddress ?? string.Empty,
                Latitude = result.Geometry?.Location?.Lat ?? 0,
                Longitude = result.Geometry?.Location?.Lng ?? 0,
                Rating = result.Rating ?? 0,
                UserRatingsTotal = result.UserRatingsTotal ?? 0,
                PriceLevel = result.PriceLevel.HasValue ? result.PriceLevel.Value.ToString() : string.Empty,
                FormattedPhoneNumber = result.FormattedPhoneNumber ?? string.Empty,
                InternationalPhoneNumber = result.InternationalPhoneNumber ?? string.Empty,
                Website = result.Website ?? string.Empty,
                OpeningHours = result.OpeningHours != null ? new OpeningHours
                {
                    OpenNow = result.OpeningHours.OpenNow ?? false,
                    WeekdayText = result.OpeningHours.WeekdayText ?? new List<string>()
                } : null,
                PhotoReferences = result.Photos?.Select(p => p.PhotoReference ?? string.Empty).ToList() ?? new List<string>(),
                Types = result.Types ?? new List<string>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting place details for place_id: {PlaceId}", placeId);
            return null;
        }
    }

    public async Task<List<PlaceSearchResult>> SearchPlacesAsync(string query, double? latitude, double? longitude, int radiusMeters = 5000, string? language = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_settings.Enabled)
            {
                _logger.LogWarning("Google Maps integration is disabled");
                return new List<PlaceSearchResult>();
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"https://maps.googleapis.com/maps/api/place/textsearch/json?query={Uri.EscapeDataString(query)}&key={_settings.ApiKey}";
            
            if (latitude.HasValue && longitude.HasValue)
            {
                url += $"&location={latitude.Value},{longitude.Value}&radius={radiusMeters}";
            }
            
            if (!string.IsNullOrEmpty(language))
                url += $"&language={language}";

            var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<PlaceSearchApiResponse>(content);

            if (apiResponse?.Status != "OK" || apiResponse.Results == null)
            {
                _logger.LogWarning("Place search failed for query: {Query}. Status: {Status}", query, apiResponse?.Status);
                return new List<PlaceSearchResult>();
            }

            return apiResponse.Results.Select(r => new PlaceSearchResult
            {
                PlaceId = r.PlaceId ?? string.Empty,
                Name = r.Name ?? string.Empty,
                FormattedAddress = r.FormattedAddress ?? string.Empty,
                Latitude = r.Geometry?.Location?.Lat ?? 0,
                Longitude = r.Geometry?.Location?.Lng ?? 0,
                Rating = r.Rating ?? 0,
                UserRatingsTotal = r.UserRatingsTotal ?? 0,
                Types = r.Types ?? new List<string>(),
                PhotoReferences = r.Photos?.Select(p => p.PhotoReference ?? string.Empty).ToList() ?? new List<string>()
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching places for query: {Query}", query);
            return new List<PlaceSearchResult>();
        }
    }

    public string GetPlacePhotoUrl(string photoReference, int maxWidth = 400, int maxHeight = 400)
    {
        return $"https://maps.googleapis.com/maps/api/place/photo?maxwidth={maxWidth}&maxheight={maxHeight}&photo_reference={photoReference}&key={_settings.ApiKey}";
    }

    public async Task<List<string>> GetPlacePhotosAsync(string placeId, int maxPhotos = 5, int photoWidth = 400, int photoHeight = 400, CancellationToken cancellationToken = default)
    {
        try
        {
            var placeDetails = await GetPlaceDetailsAsync(placeId, null, cancellationToken);
            
            if (placeDetails == null || placeDetails.PhotoReferences == null || !placeDetails.PhotoReferences.Any())
            {
                return new List<string>();
            }

            var photoUrls = placeDetails.PhotoReferences
                .Take(Math.Min(maxPhotos, _settings.MaxPhotoReferences))
                .Select(photoRef => GetPlacePhotoUrl(photoRef, photoWidth, photoHeight))
                .ToList();

            return photoUrls;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting place photos for place_id: {PlaceId}", placeId);
            return new List<string>();
        }
    }

    private class PlaceDetailsApiResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        
        [JsonPropertyName("result")]
        public PlaceDetailsApiResult? Result { get; set; }
    }

    private class PlaceDetailsApiResult
    {
        [JsonPropertyName("place_id")]
        public string? PlaceId { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("formatted_address")]
        public string? FormattedAddress { get; set; }
        
        [JsonPropertyName("geometry")]
        public GeometryApiInfo? Geometry { get; set; }
        
        [JsonPropertyName("rating")]
        public double? Rating { get; set; }
        
        [JsonPropertyName("user_ratings_total")]
        public int? UserRatingsTotal { get; set; }
        
        [JsonPropertyName("price_level")]
        public int? PriceLevel { get; set; }
        
        [JsonPropertyName("formatted_phone_number")]
        public string? FormattedPhoneNumber { get; set; }
        
        [JsonPropertyName("international_phone_number")]
        public string? InternationalPhoneNumber { get; set; }
        
        [JsonPropertyName("website")]
        public string? Website { get; set; }
        
        [JsonPropertyName("opening_hours")]
        public OpeningHoursApiInfo? OpeningHours { get; set; }
        
        [JsonPropertyName("photos")]
        public List<PhotoApiInfo>? Photos { get; set; }
        
        [JsonPropertyName("types")]
        public List<string>? Types { get; set; }
    }

    private class PlaceSearchApiResponse
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        
        [JsonPropertyName("results")]
        public List<PlaceSearchApiResult>? Results { get; set; }
    }

    private class PlaceSearchApiResult
    {
        [JsonPropertyName("place_id")]
        public string? PlaceId { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("formatted_address")]
        public string? FormattedAddress { get; set; }
        
        [JsonPropertyName("geometry")]
        public GeometryApiInfo? Geometry { get; set; }
        
        [JsonPropertyName("rating")]
        public double? Rating { get; set; }
        
        [JsonPropertyName("user_ratings_total")]
        public int? UserRatingsTotal { get; set; }
        
        [JsonPropertyName("price_level")]
        public int? PriceLevel { get; set; }
        
        [JsonPropertyName("photos")]
        public List<PhotoApiInfo>? Photos { get; set; }
        
        [JsonPropertyName("types")]
        public List<string>? Types { get; set; }
    }

    private class GeometryApiInfo
    {
        [JsonPropertyName("location")]
        public LocationApiInfo? Location { get; set; }
    }

    private class LocationApiInfo
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }
        
        [JsonPropertyName("lng")]
        public double Lng { get; set; }
    }

    private class OpeningHoursApiInfo
    {
        [JsonPropertyName("open_now")]
        public bool? OpenNow { get; set; }
        
        [JsonPropertyName("weekday_text")]
        public List<string>? WeekdayText { get; set; }
    }

    private class PhotoApiInfo
    {
        [JsonPropertyName("photo_reference")]
        public string? PhotoReference { get; set; }
    }
}