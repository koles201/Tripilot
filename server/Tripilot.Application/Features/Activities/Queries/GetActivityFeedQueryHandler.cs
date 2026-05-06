using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Activity;
using Tripilot.Application.DTOs.Common;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Activities.Queries;

/// <summary>
/// Handler for GetActivityFeedQuery
/// </summary>
public class GetActivityFeedQueryHandler : IRequestHandler<GetActivityFeedQuery, PaginatedResult<ActivityResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetActivityFeedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<ActivityResponse>> Handle(GetActivityFeedQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<UserActivity>()
            .GetQueryable()
            .Include(a => a.User)
            .Where(a => a.IsVisible);

        // Filter by followed users
        if (request.FollowedOnly && request.UserId.HasValue)
        {
            var followingIds = await _unitOfWork.Repository<UserFollow>()
                .GetQueryable()
                .Where(uf => uf.FollowerId == request.UserId.Value)
                .Select(uf => uf.FollowingId)
                .ToListAsync(cancellationToken);

            query = query.Where(a => followingIds.Contains(a.UserId));
        }
        else if (request.UserId.HasValue && !request.FollowedOnly)
        {
            // Show only the specific user's activities
            query = query.Where(a => a.UserId == request.UserId.Value);
        }

        // Filter by activity type
        if (!string.IsNullOrEmpty(request.ActivityType))
        {
            if (Enum.TryParse<ActivityType>(request.ActivityType, true, out var activityType))
            {
                query = query.Where(a => a.ActivityType == activityType);
            }
        }

        query = query.OrderByDescending(a => a.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var activities = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Get entity names for activities
        var routeIds = activities.Where(a => a.EntityType == "Route" && a.EntityId.HasValue).Select(a => a.EntityId!.Value).ToList();
        var routes = await _unitOfWork.Repository<Route>()
            .GetQueryable()
            .Where(r => routeIds.Contains(r.Id))
            .ToDictionaryAsync(r => r.Id, r => r.Name, cancellationToken);

        var collectionIds = activities.Where(a => a.EntityType == "Collection" && a.EntityId.HasValue).Select(a => a.EntityId!.Value).ToList();
        var collections = await _unitOfWork.Repository<RouteCollection>()
            .GetQueryable()
            .Where(c => collectionIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken);

        var items = activities.Select(a =>
        {
            string? entityName = null;
            if (a.EntityId.HasValue)
            {
                if (a.EntityType == "Route" && routes.ContainsKey(a.EntityId.Value))
                {
                    entityName = routes[a.EntityId.Value];
                }
                else if (a.EntityType == "Collection" && collections.ContainsKey(a.EntityId.Value))
                {
                    entityName = collections[a.EntityId.Value];
                }
            }

            return new ActivityResponse
            {
                Id = a.Id,
                UserId = a.UserId,
                UserName = a.User.FullName,
                UserAvatarUrl = a.User.AvatarUrl,
                ActivityType = a.ActivityType.ToString(),
                EntityId = a.EntityId,
                EntityType = a.EntityType,
                EntityName = entityName,
                Metadata = a.Metadata,
                CreatedAt = a.CreatedAt
            };
        }).ToList();

        return new PaginatedResult<ActivityResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages
        };
    }
}
