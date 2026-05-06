using System.ComponentModel.DataAnnotations;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.DTOs.PlaceClaim;

public class PlaceClaimResponse
{
    public Guid Id { get; set; }
    public Guid PlaceId { get; set; }
    public string PlaceName { get; set; } = string.Empty;
    public Guid ClaimantUserId { get; set; }
    public string ClaimantName { get; set; } = string.Empty;
    public Guid BusinessProfileId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public ClaimStatus Status { get; set; }
    public string ClaimReason { get; set; } = string.Empty;
    public List<string>? DocumentUrls { get; set; }
    public DateTime SubmittedAt { get; set; }
    public Guid? ReviewedByAdminId { get; set; }
    public string? ReviewedByAdminName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? AdminDecisionReason { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SubmitPlaceClaimRequest
{
    [Required]
    public Guid PlaceId { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 20)]
    public string ClaimReason { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public List<string> DocumentUrls { get; set; } = new();
}

public class ReviewPlaceClaimRequest
{
    [StringLength(2000)]
    public string? DecisionReason { get; set; }
}
