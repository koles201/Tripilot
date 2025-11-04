using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Social.Commands;

/// <summary>
/// Handler for UnfollowUserCommand
/// </summary>
public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnfollowUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
    {
        var userFollow = await _unitOfWork.Repository<UserFollow>()
            .GetQueryable()
            .FirstOrDefaultAsync(uf => uf.FollowerId == request.FollowerId && uf.FollowingId == request.FollowingId, cancellationToken);

        if (userFollow == null)
        {
            return false;
        }

        // Update follower counts
        var follower = await _unitOfWork.Repository<User>()
            .GetQueryable()
            .FirstOrDefaultAsync(u => u.Id == request.FollowerId, cancellationToken);

        var following = await _unitOfWork.Repository<User>()
            .GetQueryable()
            .FirstOrDefaultAsync(u => u.Id == request.FollowingId, cancellationToken);

        if (follower != null)
        {
            follower.FollowingCount = Math.Max(0, follower.FollowingCount - 1);
        }

        if (following != null)
        {
            following.FollowerCount = Math.Max(0, following.FollowerCount - 1);
        }

        _unitOfWork.Repository<UserFollow>().Delete(userFollow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
