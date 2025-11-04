using MediatR;
using Tripilot.Application.DTOs.RouteSharing;

namespace Tripilot.Application.Features.RouteSharing.Queries;

/// <summary>
/// Query to get route by share token
/// </summary>
public class GetRouteByShareTokenQuery : IRequest<RouteSocialMetadata>
{
    public string ShareToken { get; set; } = string.Empty;
}
