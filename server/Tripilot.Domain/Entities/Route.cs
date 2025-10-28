using Tripilot.Domain.Enums;

namespace Tripilot.Domain.Entities;

/// <summary>
/// Represents a tourist route containing multiple places
/// </summary>
public class Route : BaseEntity, IAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Creator and ownership
    public Guid CreatorId { get; set; }
    public User Creator { get; set; } = null!;
    
    // Route details
    public RouteDifficulty Difficulty { get; set; }
    public RoutePrivacy Privacy { get; set; }
    public int EstimatedDuration { get; set; } // in minutes
    public decimal? TotalDistance { get; set; } // in kilometers
    
    // Route image
    public string? ImageUrl { get; set; }
    
    // Statistics
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int ViewCount { get; set; }
    public int FavoriteCount { get; set; }
    
    // Tags for categorization (JSON array)
    public string? Tags { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
    
    // Relationships
    public ICollection<RoutePlace> RoutePlaces { get; set; } = new List<RoutePlace>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    
    // Audit properties
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
