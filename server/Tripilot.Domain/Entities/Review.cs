namespace Tripilot.Domain.Entities;

/// <summary>
/// Represents a review for either a Place or a Route
/// </summary>
public class Review : BaseEntity, IAuditableEntity
{
    // Review content
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    // Multi-aspect ratings (1-5 scale)
    public decimal OverallRating { get; set; }
    public decimal? CleanlinessRating { get; set; }
    public decimal? ServiceRating { get; set; }
    public decimal? ValueRating { get; set; }
    public decimal? LocationRating { get; set; }
    
    // Reviewer
    public Guid ReviewerId { get; set; }
    public User Reviewer { get; set; } = null!;
    
    // Polymorphic relationship - review can be for Place OR Route
    public Guid? PlaceId { get; set; }
    public Place? Place { get; set; }
    
    public Guid? RouteId { get; set; }
    public Route? Route { get; set; }
    
    // Review metadata
    public int HelpfulCount { get; set; }
    public int UnhelpfulCount { get; set; }
    public bool IsVerified { get; set; } // Verified purchase/visit
    public bool IsFlagged { get; set; } // Flagged for moderation
    public string? FlagReason { get; set; }
    
    // Response from owner
    public string? OwnerResponse { get; set; }
    public DateTime? OwnerResponseDate { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    
    // Audit properties
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
