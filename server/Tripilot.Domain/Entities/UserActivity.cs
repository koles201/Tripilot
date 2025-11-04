using Tripilot.Domain.Enums;

namespace Tripilot.Domain.Entities;

/// <summary>
/// Represents a user activity for the activity feed
/// </summary>
public class UserActivity : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// ID of the user who performed the activity
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// User who performed the activity
    /// </summary>
    public User User { get; set; } = null!;
    
    /// <summary>
    /// Type of activity
    /// </summary>
    public ActivityType ActivityType { get; set; }
    
    /// <summary>
    /// ID of the related entity (route, review, etc.)
    /// </summary>
    public Guid? EntityId { get; set; }
    
    /// <summary>
    /// Type of the related entity
    /// </summary>
    public string? EntityType { get; set; }
    
    /// <summary>
    /// Additional metadata in JSON format
    /// </summary>
    public string? Metadata { get; set; }
    
    /// <summary>
    /// Whether this activity is visible in feeds
    /// </summary>
    public bool IsVisible { get; set; } = true;
    
    // IAuditableEntity implementation
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
