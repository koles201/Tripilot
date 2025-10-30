using Tripilot.Domain.Enums;

namespace Tripilot.Domain.Entities;

/// <summary>
/// Business profile representing a company or venue owned by a user
/// </summary>
public class BusinessProfile : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// Owning user
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Business name (display name)
    /// </summary>
    public string BusinessName { get; set; } = string.Empty;

    /// <summary>
    /// Short marketing description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Address details
    /// </summary>
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }

    /// <summary>
    /// Contact info
    /// </summary>
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    /// <summary>
    /// Media assets
    /// </summary>
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }

    /// <summary>
    /// Verification workflow state
    /// </summary>
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;

    /// <summary>
    /// Timestamps for verification events
    /// </summary>
    public DateTime? VerificationSubmittedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }

    /// <summary>
    /// Reason provided when rejected
    /// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// Analytics placeholders
    /// </summary>
    public int ClaimedPlacesCount { get; set; }
    public int TotalViews { get; set; }

    // Auditing
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
