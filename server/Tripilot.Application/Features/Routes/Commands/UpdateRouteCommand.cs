using MediatR;
using Tripilot.Application.DTOs.Route;

namespace Tripilot.Application.Features.Routes.Commands;

public class UpdateRouteCommand : IRequest<RouteResponse>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Privacy { get; set; } = string.Empty;
    public int EstimatedDuration { get; set; }
    public decimal? TotalDistance { get; set; }
    public string? ImageUrl { get; set; }
    public string? Tags { get; set; }
    public bool IsActive { get; set; } = true;
    public List<RoutePlaceRequest> Places { get; set; } = new();
    public Guid UserId { get; set; }
}
