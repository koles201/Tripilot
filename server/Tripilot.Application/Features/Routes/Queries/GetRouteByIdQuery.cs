using MediatR;
using Tripilot.Application.DTOs.Route;

namespace Tripilot.Application.Features.Routes.Queries;

public class GetRouteByIdQuery : IRequest<RouteResponse>
{
    public Guid Id { get; set; }
}
