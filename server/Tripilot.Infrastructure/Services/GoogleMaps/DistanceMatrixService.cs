using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.GoogleMaps;
using Tripilot.Infrastructure.Configuration;

namespace Tripilot.Infrastructure.Services.GoogleMaps;

public class DistanceMatrixService : IDistanceMatrixService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleMapsSettings _settings;
    private readonly ILogger<DistanceMatrixService> _logger;

    public DistanceMatrixService(
        IHttpClientFactory httpClientFactory,
        IOptions<GoogleMapsSettings> settings,
        ILogger<DistanceMatrixService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<DistanceMatrixResult?> GetDistanceMatrixAsync(
        List<(double lat, double lng)> origins,
        List<(double lat, double lng)> destinations,
        TravelMode travelMode = TravelMode.Driving,
        string? language = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_settings.Enabled)
            {
                _logger.LogWarning("Google Maps API is disabled");
                return null;
            }

            var originsParam = string.Join("|", origins.Select(o => $"{o.lat},{o.lng}"));
            var destinationsParam = string.Join("|", destinations.Select(d => $"{d.lat},{d.lng}"));
            var mode = travelMode.ToString().ToLowerInvariant();

            var url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={originsParam}&destinations={destinationsParam}&mode={mode}&key={_settings.ApiKey}";
            if (!string.IsNullOrEmpty(language))
            {
                url += $"&language={language}";
            }

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<DistanceMatrixApiResponse>(content);

            if (apiResponse?.Status != "OK" || apiResponse.Rows == null)
            {
                _logger.LogWarning("Distance Matrix API returned status: {Status}", apiResponse?.Status);
                return null;
            }

            var result = new DistanceMatrixResult
            {
                OriginAddresses = apiResponse.OriginAddresses ?? new List<string>(),
                DestinationAddresses = apiResponse.DestinationAddresses ?? new List<string>(),
                Rows = apiResponse.Rows.Select(row => new DistanceMatrixRow
                {
                    Elements = row.Elements?.Select(element => new DistanceMatrixElement
                    {
                        Status = element.Status ?? "UNKNOWN",
                        DistanceText = element.Distance?.Text ?? string.Empty,
                        DurationText = element.Duration?.Text ?? string.Empty,
                        DistanceMeters = element.Distance?.Value ?? 0,
                        DurationSeconds = element.Duration?.Value ?? 0
                    }).ToList() ?? new List<DistanceMatrixElement>()
                }).ToList()
            };

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting distance matrix");
            throw;
        }
    }

    public async Task<DistanceMatrixElement?> GetDistanceBetweenPointsAsync(
        double originLat,
        double originLng,
        double destinationLat,
        double destinationLng,
        TravelMode travelMode = TravelMode.Driving,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var origins = new List<(double, double)> { (originLat, originLng) };
            var destinations = new List<(double, double)> { (destinationLat, destinationLng) };

            var matrix = await GetDistanceMatrixAsync(origins, destinations, travelMode, null, cancellationToken);

            if (matrix?.Rows == null || !matrix.Rows.Any() || !matrix.Rows[0].Elements.Any())
            {
                _logger.LogWarning("No distance information found for points ({OriginLat},{OriginLng}) to ({DestLat},{DestLng})",
                    originLat, originLng, destinationLat, destinationLng);
                return null;
            }

            var element = matrix.Rows[0].Elements[0];

            if (element.Status != "OK")
            {
                _logger.LogWarning("Distance calculation failed with status: {Status}", element.Status);
                return null;
            }

            return element;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting distance between points ({OriginLat},{OriginLng}) and ({DestLat},{DestLng})",
                originLat, originLng, destinationLat, destinationLng);
            throw;
        }
    }

    #region API Response Models

    private class DistanceMatrixApiResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("origin_addresses")]
        public List<string>? OriginAddresses { get; set; }

        [JsonPropertyName("destination_addresses")]
        public List<string>? DestinationAddresses { get; set; }

        [JsonPropertyName("rows")]
        public List<RowApiInfo>? Rows { get; set; }
    }

    private class RowApiInfo
    {
        [JsonPropertyName("elements")]
        public List<ElementApiInfo>? Elements { get; set; }
    }

    private class ElementApiInfo
    {
        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("distance")]
        public TextValueApiInfo? Distance { get; set; }

        [JsonPropertyName("duration")]
        public TextValueApiInfo? Duration { get; set; }
    }

    private class TextValueApiInfo
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }

    #endregion
}
