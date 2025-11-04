namespace Tripilot.Domain.Entities;

/// <summary>
/// Represents a following relationship between users
/// </summary>
public class UserFollow : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// ID of the user who is following
    /// </summary>
    public Guid FollowerId { get; set; }
    
    /// <summary>
    /// User who is following
    /// </summary>
    public User Follower { get; set; } = null!;
    
    /// <summary>
    /// ID of the user being followed
    /// </summary>
    public Guid FollowingId { get; set; }
    
    /// <summary>
    /// User being followed
    /// </summary>
    public User Following { get; set; } = null!;
    
    /// <summary>
    /// Whether the followed user has been notified
    /// </summary>
    public bool IsNotified { get; set; }
    
    // IAuditableEntity implementation
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
