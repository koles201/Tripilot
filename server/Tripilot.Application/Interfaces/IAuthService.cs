using Tripilot.Application.DTOs.Auth;

namespace Tripilot.Application.Interfaces;

/// <summary>
/// Service interface for authentication operations
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Register a new user
    /// </summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticate user and generate JWT token
    /// </summary>
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh JWT token using refresh token
    /// </summary>
    Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify email using verification token
    /// </summary>
    Task<bool> VerifyEmailAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Request password reset
    /// </summary>
    Task<bool> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reset password using reset token
    /// </summary>
    Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout user (invalidate refresh token)
    /// </summary>
    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}
