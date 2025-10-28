using MediatR;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Places.Commands;

/// <summary>
/// Handler for DeletePlaceCommand
/// </summary>
public class DeletePlaceCommandHandler : IRequestHandler<DeletePlaceCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeletePlaceCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeletePlaceCommand request, CancellationToken cancellationToken)
    {
        // Get existing place
        var place = await _unitOfWork.Repository<Place>().GetByIdAsync(request.PlaceId, cancellationToken);
        
        if (place == null)
        {
            throw new KeyNotFoundException($"Place with ID {request.PlaceId} not found");
        }

        // Authorization check: Only owner or admin can delete
        if (request.UserRole != "Admin" && place.OwnerId != request.UserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this place");
        }

        // Soft delete by setting IsActive to false
        place.IsActive = false;
        _unitOfWork.Repository<Place>().Update(place);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
