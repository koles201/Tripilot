namespace Tripilot.Domain.Entities;

/// <summary>
/// Represents a route within a collection
/// </summary>
public class RouteCollectionItem : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// ID of the collection
    /// </summary>
    public Guid CollectionId { get; set; }
    
    /// <summary>
    /// Collection this item belongs to
    /// </summary>
    public RouteCollection Collection { get; set; } = null!;
    
    /// <summary>
    /// ID of the route
    /// </summary>
    public Guid RouteId { get; set; }
    
    /// <summary>
    /// Route in the collection
    /// </summary>
    public Route Route { get; set; } = null!;
    
    /// <summary>
    /// Order of the route in the collection
    /// </summary>
    public int Order { get; set; }
    
    /// <summary>
    /// Optional note about this route in the collection
    /// </summary>
    public string? Note { get; set; }
    
    // IAuditableEntity implementation
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
