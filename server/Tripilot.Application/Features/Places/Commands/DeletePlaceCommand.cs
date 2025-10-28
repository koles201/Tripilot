using MediatR;

namespace Tripilot.Application.Features.Places.Commands;

/// <summary>
/// Command to delete a place
/// </summary>
public class DeletePlaceCommand : IRequest<bool>
{
    public Guid PlaceId { get; set; }
    
    // User context (set by controller)
    public Guid UserId { get; set; }
    public string UserRole { get; set; } = string.Empty;
}
