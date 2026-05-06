using MediatR;
using Tripilot.Application.DTOs.Activity;
using Tripilot.Application.DTOs.Common;

namespace Tripilot.Application.Features.Activities.Queries;

/// <summary>
/// Query to get activity feed
/// </summary>
public class GetActivityFeedQuery : IRequest<PaginatedResult<ActivityResponse>>
{
    public Guid? UserId { get; set; }
    public bool FollowedOnly { get; set; }
    public string? ActivityType { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
