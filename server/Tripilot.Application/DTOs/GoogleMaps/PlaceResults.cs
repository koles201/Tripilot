namespace Tripilot.Application.DTOs.GoogleMaps;

/// <summary>
/// Travel mode for directions and distance calculations
/// </summary>
public enum TravelMode
{
    Driving,
    Walking,
    Bicycling,
    Transit
}

/// <summary>
/// Result from place details lookup
/// </summary>
public class PlaceDetailsResult
{
    public string PlaceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FormattedAddress { get; set; } = string.Empty;
    public string FormattedPhoneNumber { get; set; } = string.Empty;
    public string InternationalPhoneNumber { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int UserRatingsTotal { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<string> Types { get; set; } = new();
    public List<string> PhotoReferences { get; set; } = new();
    public OpeningHours? OpeningHours { get; set; }
    public string PriceLevel { get; set; } = string.Empty;
}

public class OpeningHours
{
    public bool OpenNow { get; set; }
    public List<string> WeekdayText { get; set; } = new();
}

/// <summary>
/// Result from place search
/// </summary>
public class PlaceSearchResult
{
    public string PlaceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FormattedAddress { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Rating { get; set; }
    public int UserRatingsTotal { get; set; }
    public List<string> Types { get; set; } = new();
    public List<string> PhotoReferences { get; set; } = new();
}
