using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.GoogleMaps;
using Tripilot.Infrastructure.Configuration;

namespace Tripilot.Infrastructure.Services.GoogleMaps;

public class DirectionsService : IDirectionsService
{
    private readonly HttpClient _httpClient;
    private readonly GoogleMapsSettings _settings;
    private readonly ILogger<DirectionsService> _logger;

    public DirectionsService(
        IHttpClientFactory httpClientFactory,
        IOptions<GoogleMapsSettings> settings,
        ILogger<DirectionsService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<DirectionsResult?> GetDirectionsAsync(
        double originLat,
        double originLng,
        double destinationLat,
        double destinationLng,
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

            var origin = $"{originLat},{originLng}";
            var destination = $"{destinationLat},{destinationLng}";
            var mode = travelMode.ToString().ToLowerInvariant();

            var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin}&destination={destination}&mode={mode}&key={_settings.ApiKey}";
            if (!string.IsNullOrEmpty(language))
            {
                url += $"&language={language}";
            }

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<DirectionsApiResponse>(content);

            if (apiResponse?.Status != "OK" || apiResponse.Routes == null || !apiResponse.Routes.Any())
            {
                _logger.LogWarning("Directions API returned status: {Status}", apiResponse?.Status);
                return null;
            }

            return MapRouteToResult(apiResponse.Routes.First());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting directions from ({OriginLat},{OriginLng}) to ({DestLat},{DestLng})",
                originLat, originLng, destinationLat, destinationLng);
            throw;
        }
    }

    public async Task<DirectionsResult?> GetDirectionsWithWaypointsAsync(
        double originLat,
        double originLng,
        double destinationLat,
        double destinationLng,
        List<(double lat, double lng)> waypoints,
        TravelMode travelMode = TravelMode.Walking,
        bool optimizeWaypoints = false,
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

            var origin = $"{originLat},{originLng}";
            var destination = $"{destinationLat},{destinationLng}";
            var mode = travelMode.ToString().ToLowerInvariant();
            var waypointsParam = string.Join("|", waypoints.Select(w => $"{w.lat},{w.lng}"));

            var url = $"https://maps.googleapis.com/maps/api/directions/json?origin={origin}&destination={destination}&waypoints={(optimizeWaypoints ? "optimize:true|" : "")}{waypointsParam}&mode={mode}&key={_settings.ApiKey}";
            if (!string.IsNullOrEmpty(language))
            {
                url += $"&language={language}";
            }

            var response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiResponse = JsonSerializer.Deserialize<DirectionsApiResponse>(content);

            if (apiResponse?.Status != "OK" || apiResponse.Routes == null || !apiResponse.Routes.Any())
            {
                _logger.LogWarning("Directions API with waypoints returned status: {Status}", apiResponse?.Status);
                return null;
            }

            return MapRouteToResult(apiResponse.Routes.First());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting directions with waypoints from ({OriginLat},{OriginLng}) to ({DestLat},{DestLng})",
                originLat, originLng, destinationLat, destinationLng);
            throw;
        }
    }

    private DirectionsResult MapRouteToResult(RouteApiInfo route)
    {
        var allLegs = route.Legs ?? new List<LegApiInfo>();
        var totalDistance = allLegs.Sum(l => l.Distance?.Value ?? 0);
        var totalDuration = allLegs.Sum(l => l.Duration?.Value ?? 0);

        var firstLeg = allLegs.FirstOrDefault();
        var lastLeg = allLegs.LastOrDefault();

        return new DirectionsResult
        {
            Summary = route.Summary ?? string.Empty,
            DistanceText = FormatDistance(totalDistance),
            DurationText = FormatDuration(totalDuration),
            DistanceMeters = totalDistance,
            DurationSeconds = totalDuration,
            StartAddress = firstLeg?.StartAddress ?? string.Empty,
            EndAddress = lastLeg?.EndAddress ?? string.Empty,
            Steps = allLegs.SelectMany(leg => leg.Steps?.Select(s => new DirectionStep
            {
                DistanceText = s.Distance?.Text ?? string.Empty,
                DurationText = s.Duration?.Text ?? string.Empty,
                DistanceMeters = s.Distance?.Value ?? 0,
                DurationSeconds = s.Duration?.Value ?? 0,
                HtmlInstructions = s.HtmlInstructions ?? string.Empty,
                Instructions = StripHtml(s.HtmlInstructions ?? string.Empty),
                TravelMode = s.TravelMode ?? "DRIVING",
                StartLat = s.StartLocation?.Lat ?? 0,
                StartLng = s.StartLocation?.Lng ?? 0,
                EndLat = s.EndLocation?.Lat ?? 0,
                EndLng = s.EndLocation?.Lng ?? 0
            }) ?? Enumerable.Empty<DirectionStep>()).ToList(),
            EncodedPolyline = route.OverviewPolyline?.Points ?? string.Empty,
            PolylinePoints = new List<(double lat, double lng)>() // Could decode polyline here if needed
        };
    }

    private static string FormatDistance(int meters)
    {
        if (meters < 1000)
        {
            return $"{meters} m";
        }
        return $"{meters / 1000.0:F1} km";
    }

    private static string FormatDuration(int seconds)
    {
        var hours = seconds / 3600;
        var minutes = (seconds % 3600) / 60;
        
        if (hours > 0)
        {
            return $"{hours} hour{(hours != 1 ? "s" : "")} {minutes} min{(minutes != 1 ? "s" : "")}";
        }
        return $"{minutes} min{(minutes != 1 ? "s" : "")}";
    }

    private static string StripHtml(string html)
    {
        if (string.IsNullOrEmpty(html))
        {
            return string.Empty;
        }

        return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
    }

    #region API Response Models

    private class DirectionsApiResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("routes")]
        public List<RouteApiInfo>? Routes { get; set; }
    }

    private class RouteApiInfo
    {
        [JsonPropertyName("summary")]
        public string? Summary { get; set; }

        [JsonPropertyName("legs")]
        public List<LegApiInfo>? Legs { get; set; }

        [JsonPropertyName("overview_polyline")]
        public PolylineApiInfo? OverviewPolyline { get; set; }
    }

    private class LegApiInfo
    {
        [JsonPropertyName("distance")]
        public TextValueApiInfo? Distance { get; set; }

        [JsonPropertyName("duration")]
        public TextValueApiInfo? Duration { get; set; }

        [JsonPropertyName("start_address")]
        public string? StartAddress { get; set; }

        [JsonPropertyName("end_address")]
        public string? EndAddress { get; set; }

        [JsonPropertyName("steps")]
        public List<StepApiInfo>? Steps { get; set; }
    }

    private class StepApiInfo
    {
        [JsonPropertyName("distance")]
        public TextValueApiInfo? Distance { get; set; }

        [JsonPropertyName("duration")]
        public TextValueApiInfo? Duration { get; set; }

        [JsonPropertyName("html_instructions")]
        public string? HtmlInstructions { get; set; }

        [JsonPropertyName("travel_mode")]
        public string? TravelMode { get; set; }

        [JsonPropertyName("start_location")]
        public LatLngApiInfo? StartLocation { get; set; }

        [JsonPropertyName("end_location")]
        public LatLngApiInfo? EndLocation { get; set; }
    }

    private class TextValueApiInfo
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }

    private class LatLngApiInfo
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lng")]
        public double Lng { get; set; }
    }

    private class PolylineApiInfo
    {
        [JsonPropertyName("points")]
        public string? Points { get; set; }
    }

    #endregion
}
