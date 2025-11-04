using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Collections.Commands;

/// <summary>
/// Handler for AddRouteToCollectionCommand
/// </summary>
public class AddRouteToCollectionCommandHandler : IRequestHandler<AddRouteToCollectionCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddRouteToCollectionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AddRouteToCollectionCommand request, CancellationToken cancellationToken)
    {
        var collection = await _unitOfWork.Repository<RouteCollection>()
            .GetQueryable()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == request.CollectionId, cancellationToken);

        if (collection == null)
        {
            throw new InvalidOperationException("Collection not found");
        }

        // Check permission
        if (collection.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to modify this collection");
        }

        var route = await _unitOfWork.Repository<Route>()
            .GetQueryable()
            .FirstOrDefaultAsync(r => r.Id == request.RouteId, cancellationToken);

        if (route == null)
        {
            throw new InvalidOperationException("Route not found");
        }

        // Check if already in collection
        var existingItem = collection.Items.FirstOrDefault(i => i.RouteId == request.RouteId);
        if (existingItem != null)
        {
            return false;
        }

        // Get next order number
        var maxOrder = collection.Items.Any() ? collection.Items.Max(i => i.Order) : 0;

        var item = new RouteCollectionItem
        {
            CollectionId = request.CollectionId,
            RouteId = request.RouteId,
            Order = maxOrder + 1,
            Note = request.Note
        };

        await _unitOfWork.Repository<RouteCollectionItem>().AddAsync(item);

        // Create activity
        var activity = new UserActivity
        {
            UserId = request.UserId,
            ActivityType = Domain.Enums.ActivityType.RouteAddedToCollection,
            EntityId = request.RouteId,
            EntityType = "Route",
            Metadata = $"{{\"collectionId\":\"{request.CollectionId}\",\"collectionName\":\"{collection.Name}\"}}",
            IsVisible = collection.IsPublic
        };
        await _unitOfWork.Repository<UserActivity>().AddAsync(activity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
