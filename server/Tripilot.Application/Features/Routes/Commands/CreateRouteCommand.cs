using MediatR;
using Tripilot.Application.DTOs.Route;

namespace Tripilot.Application.Features.Routes.Commands;

/// <summary>
/// Command to create a new route
/// </summary>
public class CreateRouteCommand : IRequest<RouteResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Privacy { get; set; } = string.Empty;
    public int EstimatedDuration { get; set; }
    public decimal? TotalDistance { get; set; }
    public string? ImageUrl { get; set; }
    public string? Tags { get; set; }
    public List<RoutePlaceRequest> Places { get; set; } = new();
    
    // User context (set by controller)
    public Guid UserId { get; set; }
}
