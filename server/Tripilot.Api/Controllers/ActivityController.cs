using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.Activity;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.Features.Activities.Queries;

namespace Tripilot.Api.Controllers;

/// <summary>
/// Controller for activity feeds
/// </summary>
[ApiController]
[Route("api/activities")]
[Produces("application/json")]
public class ActivityController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ActivityController> _logger;

    public ActivityController(IMediator mediator, ILogger<ActivityController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get activity feed for the authenticated user (showing followed users' activities)
    /// </summary>
    [HttpGet("feed")]
    [Authorize]
    [ProducesResponseType(typeof(PaginatedResult<ActivityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaginatedResult<ActivityResponse>>> GetActivityFeed(
        [FromQuery] bool followedOnly = true,
        [FromQuery] string? activityType = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = GetUserId();

        var query = new GetActivityFeedQuery
        {
            UserId = userId,
            FollowedOnly = followedOnly,
            ActivityType = activityType,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        try
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activity feed for user {UserId}", userId);
            return BadRequest(new { message = "Failed to retrieve activity feed" });
        }
    }

    /// <summary>
    /// Get activity feed for a specific user
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(PaginatedResult<ActivityResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<ActivityResponse>>> GetUserActivities(
        Guid userId,
        [FromQuery] string? activityType = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetActivityFeedQuery
        {
            UserId = userId,
            FollowedOnly = false,
            ActivityType = activityType,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        try
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activities for user {UserId}", userId);
            return BadRequest(new { message = "Failed to retrieve activities" });
        }
    }

    /// <summary>
    /// Get global activity feed (all public activities)
    /// </summary>
    [HttpGet("global")]
    [ProducesResponseType(typeof(PaginatedResult<ActivityResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<ActivityResponse>>> GetGlobalFeed(
        [FromQuery] string? activityType = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetActivityFeedQuery
        {
            UserId = null,
            FollowedOnly = false,
            ActivityType = activityType,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        try
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving global activity feed");
            return BadRequest(new { message = "Failed to retrieve activity feed" });
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User not authenticated");
        }
        return userId;
    }
}
