namespace Tripilot.Application.DTOs.GoogleMaps;

/// <summary>
/// Result from directions API
/// </summary>
public class DirectionsResult
{
    public string Summary { get; set; } = string.Empty;
    public string DistanceText { get; set; } = string.Empty;
    public int DistanceMeters { get; set; }
    public string DurationText { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public string StartAddress { get; set; } = string.Empty;
    public string EndAddress { get; set; } = string.Empty;
    public List<DirectionStep> Steps { get; set; } = new();
    public string EncodedPolyline { get; set; } = string.Empty;
    public List<(double lat, double lng)> PolylinePoints { get; set; } = new();
}

public class DirectionStep
{
    public string Instructions { get; set; } = string.Empty;
    public string HtmlInstructions { get; set; } = string.Empty;
    public string DistanceText { get; set; } = string.Empty;
    public int DistanceMeters { get; set; }
    public string DurationText { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
    public double StartLat { get; set; }
    public double StartLng { get; set; }
    public double EndLat { get; set; }
    public double EndLng { get; set; }
    public string TravelMode { get; set; } = string.Empty;
}
