namespace Tripilot.Application.DTOs.Place;

/// <summary>
/// Response DTO for place list item (simplified)
/// </summary>
public class PlaceListResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    
    public string? City { get; set; }
    public string? Country { get; set; }
    
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    
    public int? PriceLevel { get; set; }
    public string? ImageUrl { get; set; }
    
    public bool IsVerified { get; set; }
    public int ViewCount { get; set; }
}
