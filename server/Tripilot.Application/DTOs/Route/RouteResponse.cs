namespace Tripilot.Application.DTOs.Route;

/// <summary>
/// Response DTO for route details
/// </summary>
public class RouteResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string Difficulty { get; set; } = string.Empty;
    public string Privacy { get; set; } = string.Empty;
    public int EstimatedDuration { get; set; }
    public decimal? TotalDistance { get; set; }
    
    public string? ImageUrl { get; set; }
    public string? Tags { get; set; }
    
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int ViewCount { get; set; }
    public int FavoriteCount { get; set; }
    
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    
    public Guid CreatorId { get; set; }
    public string? CreatorName { get; set; }
    
    public List<RoutePlaceResponse> Places { get; set; } = new();
    
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// Response DTO for place in a route
/// </summary>
public class RoutePlaceResponse
{
    public Guid PlaceId { get; set; }
    public string PlaceName { get; set; } = string.Empty;
    public string? PlaceImageUrl { get; set; }
    public string? PlaceCity { get; set; }
    public string? PlaceCountry { get; set; }
    public int Order { get; set; }
    public string? Notes { get; set; }
    public int? EstimatedTimeAtPlace { get; set; }
}
