using Tripilot.Domain.Enums;

namespace Tripilot.Application.DTOs.Auth;

/// <summary>
/// DTO for authentication response
/// </summary>
public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenExpires { get; set; }
    public bool EmailVerified { get; set; }
}
