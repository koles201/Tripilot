using MediatR;
using Tripilot.Application.DTOs.RouteSharing;

namespace Tripilot.Application.Features.RouteSharing.Commands;

/// <summary>
/// Command to generate a shareable link for a route
/// </summary>
public class GenerateShareLinkCommand : IRequest<ShareLinkResponse>
{
    public Guid RouteId { get; set; }
    public Guid UserId { get; set; }
}
