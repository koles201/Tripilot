using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.Review;
using Tripilot.Application.Features.Reviews.Commands;
using Tripilot.Application.Features.Reviews.Queries;

namespace Tripilot.Api.Controllers;

/// <summary>
/// Controller for review management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReviewController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReviewController> _logger;

    public ReviewController(IMediator mediator, ILogger<ReviewController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get reviews for a specific place
    /// </summary>
    [HttpGet("place/{placeId}")]
    [ProducesResponseType(typeof(List<ReviewResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReviewResponse>>> GetReviewsForPlace(
        Guid placeId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] decimal? minRating = null,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool isDescending = true)
    {
        var query = new GetReviewsForPlaceQuery
        {
            PlaceId = placeId,
            PageNumber = pageNumber,
            PageSize = pageSize,
            MinRating = minRating,
            SortBy = sortBy,
            IsDescending = isDescending
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get reviews for a specific route
    /// </summary>
    [HttpGet("route/{routeId}")]
    [ProducesResponseType(typeof(List<ReviewResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReviewResponse>>> GetReviewsForRoute(
        Guid routeId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] decimal? minRating = null,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool isDescending = true)
    {
        var query = new GetReviewsForRouteQuery
        {
            RouteId = routeId,
            PageNumber = pageNumber,
            PageSize = pageSize,
            MinRating = minRating,
            SortBy = sortBy,
            IsDescending = isDescending
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Add a new review
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReviewResponse>> AddReview([FromBody] CreateReviewRequest request)
    {
        var reviewerId = GetUserId();

        var command = new AddReviewCommand
        {
            Title = request.Title,
            Content = request.Content,
            OverallRating = request.OverallRating,
            CleanlinessRating = request.CleanlinessRating,
            ServiceRating = request.ServiceRating,
            ValueRating = request.ValueRating,
            LocationRating = request.LocationRating,
            PlaceId = request.PlaceId,
            RouteId = request.RouteId,
            ReviewerId = reviewerId
        };

        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetReviewsForPlace), new { placeId = result.PlaceId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing review
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewResponse>> UpdateReview(Guid id, [FromBody] UpdateReviewRequest request)
    {
        var reviewerId = GetUserId();

        var command = new UpdateReviewCommand
        {
            Id = id,
            Title = request.Title,
            Content = request.Content,
            OverallRating = request.OverallRating,
            CleanlinessRating = request.CleanlinessRating,
            ServiceRating = request.ServiceRating,
            ValueRating = request.ValueRating,
            LocationRating = request.LocationRating,
            ReviewerId = reviewerId
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
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Delete a review (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var reviewerId = GetUserId();

        var command = new DeleteReviewCommand
        {
            Id = id,
            ReviewerId = reviewerId
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
        catch (UnauthorizedAccessException)
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
