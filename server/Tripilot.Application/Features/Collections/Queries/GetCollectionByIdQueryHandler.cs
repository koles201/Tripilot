using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Collection;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Collections.Queries;

/// <summary>
/// Handler for GetCollectionByIdQuery
/// </summary>
public class GetCollectionByIdQueryHandler : IRequestHandler<GetCollectionByIdQuery, CollectionDetailResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCollectionByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CollectionDetailResponse> Handle(GetCollectionByIdQuery request, CancellationToken cancellationToken)
    {
        var collection = await _unitOfWork.Repository<RouteCollection>()
            .GetQueryable()
            .Include(c => c.User)
            .Include(c => c.Items.OrderBy(i => i.Order))
                .ThenInclude(i => i.Route)
            .FirstOrDefaultAsync(c => c.Id == request.CollectionId, cancellationToken);

        if (collection == null)
        {
            throw new InvalidOperationException("Collection not found");
        }

        // Check permission
        if (!collection.IsPublic && collection.UserId != request.CurrentUserId)
        {
            throw new UnauthorizedAccessException("You do not have permission to view this collection");
        }

        var items = collection.Items
            .OrderBy(i => i.Order)
            .Select(i => new CollectionItemResponse
            {
                Id = i.Id,
                RouteId = i.RouteId,
                RouteName = i.Route.Name,
                RouteImageUrl = i.Route.ImageUrl,
                Order = i.Order,
                Note = i.Note,
                AddedAt = i.CreatedAt
            }).ToList();

        return new CollectionDetailResponse
        {
            Id = collection.Id,
            Name = collection.Name,
            Description = collection.Description,
            UserId = collection.UserId,
            UserName = collection.User.FullName,
            IsPublic = collection.IsPublic,
            CoverImageUrl = collection.CoverImageUrl,
            ItemCount = collection.Items.Count,
            CreatedAt = collection.CreatedAt,
            ModifiedAt = collection.ModifiedAt,
            Items = items
        };
    }
}
