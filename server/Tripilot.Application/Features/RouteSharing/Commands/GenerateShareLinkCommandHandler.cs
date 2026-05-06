using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.RouteSharing;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.RouteSharing.Commands;

/// <summary>
/// Handler for GenerateShareLinkCommand
/// </summary>
public class GenerateShareLinkCommandHandler : IRequestHandler<GenerateShareLinkCommand, ShareLinkResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GenerateShareLinkCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ShareLinkResponse> Handle(GenerateShareLinkCommand request, CancellationToken cancellationToken)
    {
        var route = await _unitOfWork.Repository<Route>()
            .GetQueryable()
            .FirstOrDefaultAsync(r => r.Id == request.RouteId, cancellationToken);

        if (route == null)
        {
            throw new InvalidOperationException($"Route with ID {request.RouteId} not found");
        }

        // Check if user has permission to share (route must be public or user is creator)
        if (route.Privacy != Domain.Enums.RoutePrivacy.Public && route.CreatorId != request.UserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to share this route");
        }

        // Generate or retrieve share token
        if (string.IsNullOrEmpty(route.ShareToken))
        {
            route.ShareToken = GenerateShareToken();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Increment share count
        route.ShareCount++;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Get base URL from configuration (IConfiguration or appsettings.json)
        var baseUrl = "https://tripilot.com";
        var shareUrl = $"{baseUrl}/routes/shared/{route.ShareToken}";
        var embedCode = GenerateEmbedCode(route.ShareToken, 600, 400, baseUrl);

        return new ShareLinkResponse
        {
            ShareToken = route.ShareToken,
            ShareUrl = shareUrl,
            EmbedCode = embedCode
        };
    }

    private string GenerateShareToken()
    {
        // Generate a cryptographically secure random token
        var randomBytes = new byte[9]; // 9 bytes = 12 base64 characters
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "")
            .Substring(0, 12);
    }

    private string GenerateEmbedCode(string shareToken, int width, int height, string baseUrl)
    {
        return $"<iframe src=\"{baseUrl}/embed/route/{shareToken}\" width=\"{width}\" height=\"{height}\" frameborder=\"0\" allowfullscreen></iframe>";
    }
}
