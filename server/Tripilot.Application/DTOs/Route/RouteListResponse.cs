namespace Tripilot.Application.DTOs.Route;

/// <summary>
/// Response DTO for route list item (simplified)
/// </summary>
public class RouteListResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string Difficulty { get; set; } = string.Empty;
    public int EstimatedDuration { get; set; }
    public decimal? TotalDistance { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int ViewCount { get; set; }
    public int FavoriteCount { get; set; }
    
    public bool IsFeatured { get; set; }
    
    public Guid CreatorId { get; set; }
    public string? CreatorName { get; set; }
    
    public int PlaceCount { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
