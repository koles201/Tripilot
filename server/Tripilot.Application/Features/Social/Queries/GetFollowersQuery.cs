using MediatR;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.DTOs.Social;

namespace Tripilot.Application.Features.Social.Queries;

/// <summary>
/// Query to get a user's followers
/// </summary>
public class GetFollowersQuery : IRequest<PaginatedResult<UserListItemResponse>>
{
    public Guid UserId { get; set; }
    public Guid? CurrentUserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
