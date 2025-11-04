namespace Tripilot.Domain.Entities;

/// <summary>
/// Represents a collection or list of routes created by a user
/// </summary>
public class RouteCollection : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// Name of the collection
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the collection
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// ID of the user who created the collection
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// User who created the collection
    /// </summary>
    public User User { get; set; } = null!;
    
    /// <summary>
    /// Whether the collection is public
    /// </summary>
    public bool IsPublic { get; set; } = true;
    
    /// <summary>
    /// Cover image URL for the collection
    /// </summary>
    public string? CoverImageUrl { get; set; }
    
    /// <summary>
    /// Routes in this collection
    /// </summary>
    public ICollection<RouteCollectionItem> Items { get; set; } = new List<RouteCollectionItem>();
    
    // IAuditableEntity implementation
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
