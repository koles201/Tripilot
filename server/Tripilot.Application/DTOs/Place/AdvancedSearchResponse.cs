namespace Tripilot.Application.DTOs.Place;

/// <summary>
/// Response DTO for advanced search with enhanced place information
/// </summary>
public class AdvancedSearchResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Address { get; set; }
    
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    
    public int? PriceLevel { get; set; }
    public string? ImageUrl { get; set; }
    
    public bool IsVerified { get; set; }
    public int ViewCount { get; set; }
    
    /// <summary>
    /// Distance from search location in kilometers (if location-based search)
    /// </summary>
    public double? DistanceKm { get; set; }
    
    /// <summary>
    /// Search relevance score (0-1)
    /// </summary>
    public double? RelevanceScore { get; set; }
    
    /// <summary>
    /// Highlighted search term matches in name
    /// </summary>
    public string? HighlightedName { get; set; }
    
    /// <summary>
    /// Highlighted search term matches in description
    /// </summary>
    public string? HighlightedDescription { get; set; }
    
    /// <summary>
    /// List of available amenities
    /// </summary>
    public List<string>? Amenities { get; set; }
    
    /// <summary>
    /// Whether the place is currently open
    /// </summary>
    public bool? IsOpenNow { get; set; }
    
    /// <summary>
    /// Operating hours for today
    /// </summary>
    public string? TodayHours { get; set; }
}
