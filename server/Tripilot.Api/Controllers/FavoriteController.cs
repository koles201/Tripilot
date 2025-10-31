using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.Favorite;
using Tripilot.Application.Features.Favorites.Commands;
using Tripilot.Application.Features.Favorites.Queries;

namespace Tripilot.Api.Controllers;

/// <summary>
/// Controller for managing user favorites
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class FavoriteController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FavoriteController> _logger;

    public FavoriteController(IMediator mediator, ILogger<FavoriteController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Add a place to favorites
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(FavoriteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FavoriteResponse>> AddFavorite([FromBody] AddFavoriteRequest request)
    {
        var userId = GetUserId();

        var command = new AddFavoriteCommand
        {
            PlaceId = request.PlaceId,
            UserId = userId
        };

        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetFavorites), new { }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding favorite for user {UserId}", userId);
            return BadRequest(new { message = "Failed to add favorite" });
        }
    }

    /// <summary>
    /// Remove a place from favorites
    /// </summary>
    [HttpDelete("{placeId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveFavorite(Guid placeId)
    {
        var userId = GetUserId();

        var command = new RemoveFavoriteCommand
        {
            PlaceId = placeId,
            UserId = userId
        };

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Get all favorites for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(FavoriteListResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<FavoriteListResult>> GetFavorites(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? category = null,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortDirection = "desc")
    {
        var userId = GetUserId();

        var query = new GetUserFavoritesQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize,
            Category = category,
            SortBy = sortBy,
            SortDirection = sortDirection
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Check if a place is favorited by the current user
    /// </summary>
    [HttpGet("check/{placeId}")]
    [ProducesResponseType(typeof(IsFavoriteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IsFavoriteResponse>> CheckFavorite(Guid placeId)
    {
        var userId = GetUserId();

        var query = new IsFavoriteQuery
        {
            UserId = userId,
            PlaceId = placeId
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get favorites count for the current user
    /// </summary>
    [HttpGet("count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<int>> GetFavoritesCount()
    {
        var userId = GetUserId();

        var query = new GetUserFavoritesQuery
        {
            UserId = userId,
            Page = 1,
            PageSize = 1 // We only need the count
        };

        var result = await _mediator.Send(query);
        return Ok(result.TotalCount);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}
