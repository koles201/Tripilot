using System.Security.Claims;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Interfaces;

/// <summary>
/// Service interface for JWT token operations
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate JWT access token for user
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generate refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate JWT token and extract claims
    /// </summary>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Get token expiration time in minutes
    /// </summary>
    int GetTokenExpirationMinutes();
}
