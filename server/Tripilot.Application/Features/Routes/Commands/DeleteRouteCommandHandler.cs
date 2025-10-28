using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;

namespace Tripilot.Application.Features.Routes.Commands;

public class DeleteRouteCommandHandler : IRequestHandler<DeleteRouteCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRouteCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteRouteCommand request, CancellationToken cancellationToken)
    {
        var route = await _unitOfWork.Repository<Domain.Entities.Route>()
            .GetQueryable()
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (route == null)
        {
            throw new KeyNotFoundException($"Route with ID {request.Id} not found");
        }

        // Check authorization (only creator can delete)
        if (route.CreatorId != request.UserId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this route");
        }

        // Soft delete by setting IsActive to false
        route.IsActive = false;
        _unitOfWork.Repository<Domain.Entities.Route>().Update(route);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
