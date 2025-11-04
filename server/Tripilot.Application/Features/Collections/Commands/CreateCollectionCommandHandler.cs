using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Collection;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Collections.Commands;

/// <summary>
/// Handler for CreateCollectionCommand
/// </summary>
public class CreateCollectionCommandHandler : IRequestHandler<CreateCollectionCommand, CollectionResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCollectionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CollectionResponse> Handle(CreateCollectionCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Repository<User>()
            .GetQueryable()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        var collection = new RouteCollection
        {
            Name = request.Name,
            Description = request.Description,
            UserId = request.UserId,
            IsPublic = request.IsPublic,
            CoverImageUrl = request.CoverImageUrl
        };

        await _unitOfWork.Repository<RouteCollection>().AddAsync(collection);

        // Create activity
        var activity = new UserActivity
        {
            UserId = request.UserId,
            ActivityType = Domain.Enums.ActivityType.CollectionCreated,
            EntityId = collection.Id,
            EntityType = "Collection",
            IsVisible = request.IsPublic
        };
        await _unitOfWork.Repository<UserActivity>().AddAsync(activity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CollectionResponse
        {
            Id = collection.Id,
            Name = collection.Name,
            Description = collection.Description,
            UserId = collection.UserId,
            UserName = user.FullName,
            IsPublic = collection.IsPublic,
            CoverImageUrl = collection.CoverImageUrl,
            ItemCount = 0,
            CreatedAt = collection.CreatedAt
        };
    }
}
