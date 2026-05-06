using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.DTOs.Social;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Social.Queries;

/// <summary>
/// Handler for GetFollowersQuery
/// </summary>
public class GetFollowersQueryHandler : IRequestHandler<GetFollowersQuery, PaginatedResult<UserListItemResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFollowersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<UserListItemResponse>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<UserFollow>()
            .GetQueryable()
            .Include(uf => uf.Follower)
            .Where(uf => uf.FollowingId == request.UserId)
            .OrderByDescending(uf => uf.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var follows = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Get current user's following list to check IsFollowing
        var currentUserFollowing = new HashSet<Guid>();
        if (request.CurrentUserId.HasValue)
        {
            currentUserFollowing = await _unitOfWork.Repository<UserFollow>()
                .GetQueryable()
                .Where(uf => uf.FollowerId == request.CurrentUserId.Value)
                .Select(uf => uf.FollowingId)
                .ToHashSetAsync(cancellationToken);
        }

        var items = follows.Select(uf => new UserListItemResponse
        {
            Id = uf.Follower.Id,
            FullName = uf.Follower.FullName,
            Bio = uf.Follower.Bio,
            AvatarUrl = uf.Follower.AvatarUrl,
            IsFollowing = currentUserFollowing.Contains(uf.Follower.Id),
            FollowedAt = uf.CreatedAt
        }).ToList();

        return new PaginatedResult<UserListItemResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.PageNumber,
            PageSize = request.PageSize,
            TotalPages = totalPages
        };
    }
}
