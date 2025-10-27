using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tripilot.Application.DTOs.Auth;
using Tripilot.Application.Interfaces;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Interfaces;
using Tripilot.Infrastructure.Data;
using Tripilot.Shared.Settings;
using BCrypt.Net;

namespace Tripilot.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthService> _logger;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        ApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork,
        ILogger<AuthService> logger,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Create new user
        var user = new User
        {
            Email = request.Email.ToLower(),
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            EmailVerificationToken = Guid.NewGuid().ToString(),
            EmailVerificationTokenExpires = DateTime.UtcNow.AddHours(24)
        };

        await _context.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Email} registered successfully", user.Email);

        // TODO: Send verification email
        // await _emailService.SendVerificationEmailAsync(user.Email, user.EmailVerificationToken);

        // Generate tokens
        var token = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            Token = token,
            RefreshToken = refreshToken,
            TokenExpires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            EmailVerified = user.EmailVerified
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower(), cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is deactivated");
        }

        // Generate tokens
        var token = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        user.LastLoginAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {Email} logged in successfully", user.Email);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            Token = token,
            RefreshToken = refreshToken,
            TokenExpires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            EmailVerified = user.EmailVerified
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);

        if (user == null || user.RefreshTokenExpires < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Generate new tokens
        var newToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role,
            Token = newToken,
            RefreshToken = newRefreshToken,
            TokenExpires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            EmailVerified = user.EmailVerified
        };
    }

    public async Task<bool> VerifyEmailAsync(string token, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == token, cancellationToken);

        if (user == null || user.EmailVerificationTokenExpires < DateTime.UtcNow)
        {
            return false;
        }

        user.EmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpires = null;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Email verified for user {Email}", user.Email);

        return true;
    }

    public async Task<bool> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);

        if (user == null)
        {
            // Don't reveal that user doesn't exist
            return true;
        }

        user.PasswordResetToken = Guid.NewGuid().ToString();
        user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password reset requested for user {Email}", user.Email);

        // TODO: Send password reset email
        // await _emailService.SendPasswordResetEmailAsync(user.Email, user.PasswordResetToken);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == token, cancellationToken);

        if (user == null || user.PasswordResetTokenExpires < DateTime.UtcNow)
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password reset successfully for user {Email}", user.Email);

        return true;
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);

        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpires = null;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {Email} logged out successfully", user.Email);
        }
    }
}
