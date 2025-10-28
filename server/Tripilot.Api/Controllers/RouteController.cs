using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.Route;
using Tripilot.Application.Features.Routes.Commands;
using Tripilot.Application.Features.Routes.Queries;

namespace Tripilot.Api.Controllers;

/// <summary>
/// Controller for route management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RouteController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RouteController> _logger;

    public RouteController(IMediator mediator, ILogger<RouteController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all public routes with filtering and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<RouteListResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RouteListResponse>>> GetRoutes(
        [FromQuery] string? difficulty = null,
        [FromQuery] string? privacy = null,
        [FromQuery] Guid? creatorId = null,
        [FromQuery] bool? isFeatured = null,
        [FromQuery] decimal? minRating = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool isDescending = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetRoutesListQuery
        {
            Difficulty = difficulty,
            Privacy = privacy,
            CreatorId = creatorId,
            IsFeatured = isFeatured,
            MinRating = minRating,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            IsDescending = isDescending,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a route by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RouteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RouteResponse>> GetRouteById(Guid id)
    {
        try
        {
            var query = new GetRouteByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create a new route
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(RouteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RouteResponse>> CreateRoute([FromBody] CreateRouteRequest request)
    {
        var userId = GetUserId();

        var command = new CreateRouteCommand
        {
            Name = request.Name,
            Description = request.Description,
            Difficulty = request.Difficulty,
            Privacy = request.Privacy,
            EstimatedDuration = request.EstimatedDuration,
            TotalDistance = request.TotalDistance,
            ImageUrl = request.ImageUrl,
            Tags = request.Tags,
            Places = request.Places,
            UserId = userId
        };

        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetRouteById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing route
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(RouteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RouteResponse>> UpdateRoute(Guid id, [FromBody] UpdateRouteRequest request)
    {
        var userId = GetUserId();

        var command = new UpdateRouteCommand
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Difficulty = request.Difficulty,
            Privacy = request.Privacy,
            EstimatedDuration = request.EstimatedDuration,
            TotalDistance = request.TotalDistance,
            ImageUrl = request.ImageUrl,
            Tags = request.Tags,
            IsActive = request.IsActive,
            Places = request.Places,
            UserId = userId
        };

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a route (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRoute(Guid id)
    {
        var userId = GetUserId();

        var command = new DeleteRouteCommand
        {
            Id = id,
            UserId = userId
        };

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
