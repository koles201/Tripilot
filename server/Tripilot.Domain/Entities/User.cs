using Tripilot.Domain.Enums;

namespace Tripilot.Domain.Entities;

/// <summary>
/// User entity representing application users
/// </summary>
public class User : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// User's email address (unique identifier for login)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// User's role in the system
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Tourist;

    /// <summary>
    /// Whether the email has been verified
    /// </summary>
    public bool EmailVerified { get; set; } = false;

    /// <summary>
    /// Email verification token
    /// </summary>
    public string? EmailVerificationToken { get; set; }

    /// <summary>
    /// Email verification token expiration
    /// </summary>
    public DateTime? EmailVerificationTokenExpires { get; set; }

    /// <summary>
    /// Password reset token
    /// </summary>
    public string? PasswordResetToken { get; set; }

    /// <summary>
    /// Password reset token expiration
    /// </summary>
    public DateTime? PasswordResetTokenExpires { get; set; }

    /// <summary>
    /// Refresh token for JWT authentication
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Refresh token expiration
    /// </summary>
    public DateTime? RefreshTokenExpires { get; set; }

    /// <summary>
    /// Whether the user account is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>
    /// User's bio/description
    /// </summary>
    public string? Bio { get; set; }
    
    /// <summary>
    /// User's avatar URL
    /// </summary>
    public string? AvatarUrl { get; set; }
    
    /// <summary>
    /// Count of users following this user
    /// </summary>
    public int FollowerCount { get; set; }
    
    /// <summary>
    /// Count of users this user is following
    /// </summary>
    public int FollowingCount { get; set; }

    // IAuditableEntity implementation
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
