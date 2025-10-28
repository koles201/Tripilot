namespace Tripilot.Domain.Entities;

/// <summary>
/// Junction entity for many-to-many relationship between Route and Place with ordering
/// </summary>
public class RoutePlace
{
    public Guid RouteId { get; set; }
    public Route Route { get; set; } = null!;
    
    public Guid PlaceId { get; set; }
    public Place Place { get; set; } = null!;
    
    // Order of the place in the route (1-based)
    public int Order { get; set; }
    
    // Optional notes for this place in the route
    public string? Notes { get; set; }
    
    // Estimated time to spend at this place (in minutes)
    public int? EstimatedTimeAtPlace { get; set; }
    
    // Created timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
