using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.DTOs.Social;
using Tripilot.Application.Features.Social.Commands;
using Tripilot.Application.Features.Social.Queries;

namespace Tripilot.Api.Controllers;

/// <summary>
/// Controller for social features (following/followers)
/// </summary>
[ApiController]
[Route("api/social")]
[Produces("application/json")]
public class SocialController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SocialController> _logger;

    public SocialController(IMediator mediator, ILogger<SocialController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Follow a user
    /// </summary>
    [HttpPost("follow")]
    [Authorize]
    [ProducesResponseType(typeof(UserFollowResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserFollowResponse>> FollowUser([FromBody] FollowUserRequest request)
    {
        var userId = GetUserId();

        var command = new FollowUserCommand
        {
            FollowerId = userId,
            FollowingId = request.UserId
        };

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following user {UserId}", request.UserId);
            return BadRequest(new { message = "Failed to follow user" });
        }
    }

    /// <summary>
    /// Unfollow a user
    /// </summary>
    [HttpPost("unfollow")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnfollowUser([FromBody] FollowUserRequest request)
    {
        var userId = GetUserId();

        var command = new UnfollowUserCommand
        {
            FollowerId = userId,
            FollowingId = request.UserId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Get followers of a user
    /// </summary>
    [HttpGet("users/{userId}/followers")]
    [ProducesResponseType(typeof(PaginatedResult<UserListItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<UserListItemResponse>>> GetFollowers(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var currentUserId = GetUserIdOptional();

        var query = new GetFollowersQuery
        {
            UserId = userId,
            CurrentUserId = currentUserId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get users that a user is following
    /// </summary>
    [HttpGet("users/{userId}/following")]
    [ProducesResponseType(typeof(PaginatedResult<UserListItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<UserListItemResponse>>> GetFollowing(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var currentUserId = GetUserIdOptional();

        var query = new GetFollowingQuery
        {
            UserId = userId,
            CurrentUserId = currentUserId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
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

    private Guid? GetUserIdOptional()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }
        return userId;
    }
}
