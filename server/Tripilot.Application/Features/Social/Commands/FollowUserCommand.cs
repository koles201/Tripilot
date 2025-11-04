using MediatR;
using Tripilot.Application.DTOs.Social;

namespace Tripilot.Application.Features.Social.Commands;

/// <summary>
/// Command to follow a user
/// </summary>
public class FollowUserCommand : IRequest<UserFollowResponse>
{
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
}
