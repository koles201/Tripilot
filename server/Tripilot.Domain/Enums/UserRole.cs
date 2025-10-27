namespace Tripilot.Domain.Enums;

/// <summary>
/// User roles in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Tourist - regular user who creates and follows routes
    /// </summary>
    Tourist = 1,

    /// <summary>
    /// Business Owner - manages places and business listings
    /// </summary>
    BusinessOwner = 2,

    /// <summary>
    /// Administrator - full system access
    /// </summary>
    Admin = 3
}
