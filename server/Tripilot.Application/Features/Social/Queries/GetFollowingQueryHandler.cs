using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.DTOs.Social;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Social.Queries;

/// <summary>
/// Handler for GetFollowingQuery
/// </summary>
public class GetFollowingQueryHandler : IRequestHandler<GetFollowingQuery, PaginatedResult<UserListItemResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetFollowingQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<UserListItemResponse>> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<UserFollow>()
            .GetQueryable()
            .Include(uf => uf.Following)
            .Where(uf => uf.FollowerId == request.UserId)
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
            Id = uf.Following.Id,
            FullName = uf.Following.FullName,
            Bio = uf.Following.Bio,
            AvatarUrl = uf.Following.AvatarUrl,
            IsFollowing = currentUserFollowing.Contains(uf.Following.Id),
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
