using Tripilot.Domain.Enums;

namespace Tripilot.Domain.Entities;

public class PlaceClaim : BaseEntity, IAuditableEntity
{
    public Guid PlaceId { get; set; }
    public Place Place { get; set; } = null!;
    public Guid ClaimantUserId { get; set; }
    public User Claimant { get; set; } = null!;
    public Guid BusinessProfileId { get; set; }
    public BusinessProfile BusinessProfile { get; set; } = null!;
    public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
    public string ClaimReason { get; set; } = string.Empty;
    public string? DocumentUrls { get; set; }
    public DateTime SubmittedAt { get; set; }
    public Guid? ReviewedByAdminId { get; set; }
    public User? ReviewedByAdmin { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? AdminDecisionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
