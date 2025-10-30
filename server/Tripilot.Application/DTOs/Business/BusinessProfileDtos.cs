using System.ComponentModel.DataAnnotations;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.DTOs.Business;

public class BusinessProfileResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? BannerUrl { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public DateTime? VerificationSubmittedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? RejectionReason { get; set; }
    public int ClaimedPlacesCount { get; set; }
    public int TotalViews { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

public class CreateBusinessProfileRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string BusinessName { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [Url]
    [StringLength(300)]
    public string? Website { get; set; }

    [Url]
    [StringLength(500)]
    public string? LogoUrl { get; set; }

    [Url]
    [StringLength(500)]
    public string? BannerUrl { get; set; }
}

public class UpdateBusinessProfileRequest
{
    [StringLength(200, MinimumLength = 2)]
    public string? BusinessName { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(300)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [Url]
    [StringLength(300)]
    public string? Website { get; set; }

    [Url]
    [StringLength(500)]
    public string? LogoUrl { get; set; }

    [Url]
    [StringLength(500)]
    public string? BannerUrl { get; set; }
}

public class SubmitVerificationRequest
{
    // Placeholder for uploaded document references
    [Required]
    [MinLength(1)]
    public List<string> DocumentUrls { get; set; } = new();
}

public class AdminDecisionRequest
{
    [Required]
    public bool Approve { get; set; }

    [StringLength(1000)]
    public string? RejectionReason { get; set; }
}
