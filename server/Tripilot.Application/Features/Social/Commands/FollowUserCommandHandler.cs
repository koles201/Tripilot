using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Social;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Social.Commands;

/// <summary>
/// Handler for FollowUserCommand
/// </summary>
public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, UserFollowResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public FollowUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserFollowResponse> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        // Validate users exist
        var follower = await _unitOfWork.Repository<User>()
            .GetQueryable()
            .FirstOrDefaultAsync(u => u.Id == request.FollowerId, cancellationToken);

        var following = await _unitOfWork.Repository<User>()
            .GetQueryable()
            .FirstOrDefaultAsync(u => u.Id == request.FollowingId, cancellationToken);

        if (follower == null || following == null)
        {
            throw new InvalidOperationException("User not found");
        }

        if (request.FollowerId == request.FollowingId)
        {
            throw new InvalidOperationException("Cannot follow yourself");
        }

        // Check if already following
        var existingFollow = await _unitOfWork.Repository<UserFollow>()
            .GetQueryable()
            .FirstOrDefaultAsync(uf => uf.FollowerId == request.FollowerId && uf.FollowingId == request.FollowingId, cancellationToken);

        if (existingFollow != null)
        {
            throw new InvalidOperationException("Already following this user");
        }

        // Create follow relationship
        var userFollow = new UserFollow
        {
            FollowerId = request.FollowerId,
            FollowingId = request.FollowingId,
            IsNotified = false
        };

        await _unitOfWork.Repository<UserFollow>().AddAsync(userFollow);

        // Update follower counts
        follower.FollowingCount++;
        following.FollowerCount++;

        // Create activity
        var activity = new UserActivity
        {
            UserId = request.FollowerId,
            ActivityType = Domain.Enums.ActivityType.UserFollowed,
            EntityId = request.FollowingId,
            EntityType = "User",
            IsVisible = true
        };
        await _unitOfWork.Repository<UserActivity>().AddAsync(activity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UserFollowResponse
        {
            Id = userFollow.Id,
            FollowerId = userFollow.FollowerId,
            FollowingId = userFollow.FollowingId,
            CreatedAt = userFollow.CreatedAt
        };
    }
}
