namespace Tripilot.Application.DTOs.Place;

/// <summary>
/// Result container for advanced search with metadata
/// </summary>
public class AdvancedSearchResult
{
    public List<AdvancedSearchResponse> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    
    /// <summary>
    /// Applied filters summary
    /// </summary>
    public SearchFilters? AppliedFilters { get; set; }
    
    /// <summary>
    /// Search execution time in milliseconds
    /// </summary>
    public long ExecutionTimeMs { get; set; }
}

/// <summary>
/// Summary of applied search filters
/// </summary>
public class SearchFilters
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
    public int? MinPriceLevel { get; set; }
    public int? MaxPriceLevel { get; set; }
    public bool? IsVerified { get; set; }
    public List<string>? Amenities { get; set; }
    public bool? IsOpenNow { get; set; }
    public bool? Is24Hours { get; set; }
    public LocationFilter? Location { get; set; }
}

/// <summary>
/// Location filter information
/// </summary>
public class LocationFilter
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; }
}
