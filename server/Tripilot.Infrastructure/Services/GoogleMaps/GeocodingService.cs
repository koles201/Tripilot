using System.Text.Json;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.GoogleMaps;
using Tripilot.Infrastructure.Configuration;

namespace Tripilot.Infrastructure.Services.GoogleMaps;

public class GeocodingService : IGeocodingService
{
    private readonly GoogleMapsSettings _settings;
    private readonly ILogger<GeocodingService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private const string BaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";

    public GeocodingService(
        IOptions<GoogleMapsSettings> settings,
        ILogger<GeocodingService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _settings = settings.Value;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GeocodingResult?> GeocodeAddressAsync(string address, string? language = null, string? region = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_settings.Enabled)
            {
                _logger.LogWarning("Google Maps integration is disabled");
                return null;
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"{BaseUrl}?address={Uri.EscapeDataString(address)}&key={_settings.ApiKey}";
            
            if (!string.IsNullOrEmpty(language))
                url += $"&language={language}";
            if (!string.IsNullOrEmpty(region))
                url += $"&region={region}";

            var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<GeocodeResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Status != "OK" || result.Results == null || !result.Results.Any())
            {
                _logger.LogWarning("Geocoding failed for address: {Address}. Status: {Status}", address, result?.Status);
                return null;
            }

            return MapToGeocodingResult(result.Results.First());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error geocoding address: {Address}", address);
            return null;
        }
    }

    public async Task<GeocodingResult?> ReverseGeocodeAsync(double latitude, double longitude, string? language = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_settings.Enabled)
            {
                _logger.LogWarning("Google Maps integration is disabled");
                return null;
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"{BaseUrl}?latlng={latitude},{longitude}&key={_settings.ApiKey}";
            
            if (!string.IsNullOrEmpty(language))
                url += $"&language={language}";

            var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<GeocodeResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Status != "OK" || result.Results == null || !result.Results.Any())
            {
                _logger.LogWarning("Reverse geocoding failed for coordinates: {Lat}, {Lng}. Status: {Status}", latitude, longitude, result?.Status);
                return null;
            }

            return MapToGeocodingResult(result.Results.First());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reverse geocoding coordinates: {Lat}, {Lng}", latitude, longitude);
            return null;
        }
    }

    public async Task<List<GeocodingResult>> GeocodeAddressMultipleAsync(string address, int maxResults = 5, string? language = null, string? region = null, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_settings.Enabled)
            {
                _logger.LogWarning("Google Maps integration is disabled");
                return new List<GeocodingResult>();
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"{BaseUrl}?address={Uri.EscapeDataString(address)}&key={_settings.ApiKey}";
            
            if (!string.IsNullOrEmpty(language))
                url += $"&language={language}";
            if (!string.IsNullOrEmpty(region))
                url += $"&region={region}";

            var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<GeocodeResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Status != "OK" || result.Results == null || !result.Results.Any())
            {
                _logger.LogWarning("Multiple geocoding failed for address: {Address}. Status: {Status}", address, result?.Status);
                return new List<GeocodingResult>();
            }

            return result.Results
                .Take(maxResults)
                .Select(MapToGeocodingResult)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error geocoding address (multiple): {Address}", address);
            return new List<GeocodingResult>();
        }
    }

    private GeocodingResult MapToGeocodingResult(GeocodeResultItem result)
    {
        return new GeocodingResult
        {
            FormattedAddress = result.Formatted_Address ?? string.Empty,
            Latitude = result.Geometry?.Location?.Lat ?? 0,
            Longitude = result.Geometry?.Location?.Lng ?? 0,
            PlaceId = result.Place_Id ?? string.Empty,
            LocationType = result.Geometry?.Location_Type ?? string.Empty,
            AddressComponents = result.Address_Components?.Select(ac => new Application.DTOs.GoogleMaps.AddressComponent
            {
                LongName = ac.Long_Name ?? string.Empty,
                ShortName = ac.Short_Name ?? string.Empty,
                Types = ac.Types ?? new List<string>()
            }).ToList() ?? new List<Application.DTOs.GoogleMaps.AddressComponent>()
        };
    }

    // API Response Models
    private class GeocodeResponse
    {
        public string? Status { get; set; }
        public List<GeocodeResultItem>? Results { get; set; }
    }

    private class GeocodeResultItem
    {
        public string? Formatted_Address { get; set; }
        public string? Place_Id { get; set; }
        public GeometryInfo? Geometry { get; set; }
        public List<AddressComponentItem>? Address_Components { get; set; }
    }

    private class GeometryInfo
    {
        public LocationInfo? Location { get; set; }
        public string? Location_Type { get; set; }
    }

    private class LocationInfo
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    private class AddressComponentItem
    {
        public string? Long_Name { get; set; }
        public string? Short_Name { get; set; }
        public List<string>? Types { get; set; }
    }
}
