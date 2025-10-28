namespace Tripilot.Application.DTOs.GoogleMaps;

/// <summary>
/// Result from distance matrix API
/// </summary>
public class DistanceMatrixResult
{
    public List<string> OriginAddresses { get; set; } = new();
    public List<string> DestinationAddresses { get; set; } = new();
    public List<DistanceMatrixRow> Rows { get; set; } = new();
}

public class DistanceMatrixRow
{
    public List<DistanceMatrixElement> Elements { get; set; } = new();
}

public class DistanceMatrixElement
{
    public string Status { get; set; } = string.Empty;
    public string DistanceText { get; set; } = string.Empty;
    public int DistanceMeters { get; set; }
    public string DurationText { get; set; } = string.Empty;
    public int DurationSeconds { get; set; }
}
