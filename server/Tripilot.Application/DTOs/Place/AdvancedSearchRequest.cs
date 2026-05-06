namespace Tripilot.Application.DTOs.Place;

/// <summary>
/// Request DTO for advanced place search with comprehensive filtering
/// </summary>
public class AdvancedSearchRequest
{
    /// <summary>
    /// Search term for full-text search across name, description, and tags
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Filter by city
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Filter by country
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Minimum rating (1-5)
    /// </summary>
    public decimal? MinRating { get; set; }

    /// <summary>
    /// Maximum rating (1-5)
    /// </summary>
    public decimal? MaxRating { get; set; }

    /// <summary>
    /// Minimum price level (1-4, where 1 is cheapest)
    /// </summary>
    public int? MinPriceLevel { get; set; }

    /// <summary>
    /// Maximum price level (1-4, where 4 is most expensive)
    /// </summary>
    public int? MaxPriceLevel { get; set; }

    /// <summary>
    /// Filter by verified status
    /// </summary>
    public bool? IsVerified { get; set; }

    /// <summary>
    /// Filter by amenities (comma-separated list)
    /// </summary>
    public string? Amenities { get; set; }

    /// <summary>
    /// Filter by open status at current time
    /// </summary>
    public bool? IsOpenNow { get; set; }

    /// <summary>
    /// Filter places open 24 hours
    /// </summary>
    public bool? Is24Hours { get; set; }

    /// <summary>
    /// Location-based search: latitude
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// Location-based search: longitude
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// Search radius in kilometers
    /// </summary>
    public double? RadiusKm { get; set; }

    /// <summary>
    /// Sort by: relevance, distance, rating, popularity, newest
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction: asc or desc
    /// </summary>
    public string? SortDirection { get; set; }

    /// <summary>
    /// Page number for pagination
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; set; } = 20;
}
