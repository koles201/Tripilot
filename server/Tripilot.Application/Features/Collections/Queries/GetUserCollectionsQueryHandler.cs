using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.DTOs.Collection;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Collections.Queries;

/// <summary>
/// Handler for GetUserCollectionsQuery
/// </summary>
public class GetUserCollectionsQueryHandler : IRequestHandler<GetUserCollectionsQuery, PaginatedResult<CollectionResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserCollectionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<CollectionResponse>> Handle(GetUserCollectionsQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<RouteCollection>()
            .GetQueryable()
            .Include(c => c.User)
            .Include(c => c.Items)
            .Where(c => c.UserId == request.UserId);

        // If not the owner, only show public collections
        if (request.CurrentUserId != request.UserId)
        {
            query = query.Where(c => c.IsPublic);
        }

        query = query.OrderByDescending(c => c.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var collections = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = collections.Select(c => new CollectionResponse
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            UserId = c.UserId,
            UserName = c.User.FullName,
            IsPublic = c.IsPublic,
            CoverImageUrl = c.CoverImageUrl,
            ItemCount = c.Items.Count,
            CreatedAt = c.CreatedAt,
            ModifiedAt = c.ModifiedAt
        }).ToList();

        return new PaginatedResult<CollectionResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages
        };
    }
}
