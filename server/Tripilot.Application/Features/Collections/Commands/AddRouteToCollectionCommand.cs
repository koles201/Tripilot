using MediatR;

namespace Tripilot.Application.Features.Collections.Commands;

/// <summary>
/// Command to add a route to a collection
/// </summary>
public class AddRouteToCollectionCommand : IRequest<bool>
{
    public Guid CollectionId { get; set; }
    public Guid RouteId { get; set; }
    public Guid UserId { get; set; }
    public string? Note { get; set; }
}
