using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.RouteSharing;
using Tripilot.Application.Features.RouteSharing.Commands;
using Tripilot.Application.Features.RouteSharing.Queries;

namespace Tripilot.Api.Controllers;

/// <summary>
/// Controller for route sharing features
/// </summary>
[ApiController]
[Route("api/routes/sharing")]
[Produces("application/json")]
public class RouteSharingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RouteSharingController> _logger;

    public RouteSharingController(IMediator mediator, ILogger<RouteSharingController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Generate a shareable link for a route
    /// </summary>
    [HttpPost("{routeId}/generate-link")]
    [Authorize]
    [ProducesResponseType(typeof(ShareLinkResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShareLinkResponse>> GenerateShareLink(Guid routeId)
    {
        var userId = GetUserId();

        var command = new GenerateShareLinkCommand
        {
            RouteId = routeId,
            UserId = userId
        };

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating share link for route {RouteId}", routeId);
            return BadRequest(new { message = "Failed to generate share link" });
        }
    }

    /// <summary>
    /// Get route metadata by share token (for social media previews)
    /// </summary>
    [HttpGet("token/{shareToken}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RouteSocialMetadata), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RouteSocialMetadata>> GetRouteByShareToken(string shareToken)
    {
        var query = new GetRouteByShareTokenQuery
        {
            ShareToken = shareToken
        };

        try
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving route by share token {ShareToken}", shareToken);
            return BadRequest(new { message = "Failed to retrieve route" });
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
