namespace Tripilot.Domain.Enums;

/// <summary>
/// Verification status for business profiles
/// </summary>
public enum VerificationStatus
{
    Pending = 0,      // Profile created but not submitted for verification
    Submitted = 1,    // User submitted documents; awaiting review
    Approved = 2,     // Verified by admin
    Rejected = 3      // Rejected by admin with reason
}
