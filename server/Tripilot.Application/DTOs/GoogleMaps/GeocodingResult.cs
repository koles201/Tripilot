namespace Tripilot.Application.DTOs.GoogleMaps;

/// <summary>
/// Result from geocoding or reverse geocoding operation
/// </summary>
public class GeocodingResult
{
    public string FormattedAddress { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string PlaceId { get; set; } = string.Empty;
    public List<AddressComponent> AddressComponents { get; set; } = new();
    public string LocationType { get; set; } = string.Empty;
}

public class AddressComponent
{
    public string LongName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public List<string> Types { get; set; } = new();
}
