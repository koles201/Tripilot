using MediatR;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.DTOs.Social;

namespace Tripilot.Application.Features.Social.Queries;

/// <summary>
/// Query to get users that a user is following
/// </summary>
public class GetFollowingQuery : IRequest<PaginatedResult<UserListItemResponse>>
{
    public Guid UserId { get; set; }
    public Guid? CurrentUserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
