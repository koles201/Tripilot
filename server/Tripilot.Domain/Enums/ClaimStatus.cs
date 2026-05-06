namespace Tripilot.Domain.Enums;

/// <summary>
/// Status of a place claim
/// </summary>
public enum ClaimStatus
{
    Pending = 0,       // Claim submitted, awaiting initial review
    UnderReview = 1,   // Admin is reviewing the claim
    Approved = 2,      // Claim approved, ownership transferred
    Rejected = 3       // Claim rejected
}
