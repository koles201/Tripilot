using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.Collection;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.Features.Collections.Commands;
using Tripilot.Application.Features.Collections.Queries;

namespace Tripilot.Api.Controllers;

/// <summary>
/// Controller for route collections
/// </summary>
[ApiController]
[Route("api/collections")]
[Produces("application/json")]
public class CollectionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CollectionController> _logger;

    public CollectionController(IMediator mediator, ILogger<CollectionController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new collection
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CollectionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CollectionResponse>> CreateCollection([FromBody] CreateCollectionRequest request)
    {
        var userId = GetUserId();

        var command = new CreateCollectionCommand
        {
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            IsPublic = request.IsPublic,
            CoverImageUrl = request.CoverImageUrl
        };

        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetCollectionById), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating collection for user {UserId}", userId);
            return BadRequest(new { message = "Failed to create collection" });
        }
    }

    /// <summary>
    /// Add a route to a collection
    /// </summary>
    [HttpPost("{collectionId}/routes")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddRouteToCollection(Guid collectionId, [FromBody] AddRouteToCollectionRequest request)
    {
        var userId = GetUserId();

        var command = new AddRouteToCollectionCommand
        {
            CollectionId = collectionId,
            RouteId = request.RouteId,
            UserId = userId,
            Note = request.Note
        };

        try
        {
            await _mediator.Send(command);
            return NoContent();
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
            _logger.LogError(ex, "Error adding route to collection {CollectionId}", collectionId);
            return BadRequest(new { message = "Failed to add route to collection" });
        }
    }

    /// <summary>
    /// Get user's collections
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(PaginatedResult<CollectionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<CollectionResponse>>> GetUserCollections(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var currentUserId = GetUserIdOptional();

        var query = new GetUserCollectionsQuery
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
    /// Get collection by ID with all routes
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CollectionDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CollectionDetailResponse>> GetCollectionById(Guid id)
    {
        var currentUserId = GetUserIdOptional();

        var query = new GetCollectionByIdQuery
        {
            CollectionId = id,
            CurrentUserId = currentUserId
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
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving collection {CollectionId}", id);
            return BadRequest(new { message = "Failed to retrieve collection" });
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
