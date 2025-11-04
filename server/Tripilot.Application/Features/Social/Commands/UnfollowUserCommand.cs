using MediatR;

namespace Tripilot.Application.Features.Social.Commands;

/// <summary>
/// Command to unfollow a user
/// </summary>
public class UnfollowUserCommand : IRequest<bool>
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
}
