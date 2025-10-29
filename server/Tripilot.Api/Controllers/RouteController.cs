using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.DTOs.Route;
using Tripilot.Application.Features.Routes.Commands;
using Tripilot.Application.Features.Routes.Queries;

namespace Tripilot.Api.Controllers;

/// <summary>
/// Controller for route management
/// </summary>
[ApiController]
[Route("api/routes")]
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
    [ProducesResponseType(typeof(PaginatedResult<RouteListResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<RouteListResponse>>> GetRoutes(
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
    /// Get routes created by the authenticated user
    /// </summary>
    [HttpGet("my-routes")]
    [Authorize]
    [ProducesResponseType(typeof(PaginatedResult<RouteListResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PaginatedResult<RouteListResponse>>> GetMyRoutes(
        [FromQuery] string? difficulty = null,
        [FromQuery] string? privacy = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool isDescending = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var userId = GetUserId();

        var query = new GetRoutesListQuery
        {
            Difficulty = difficulty,
            Privacy = privacy,
            CreatorId = userId,
            SearchTerm = searchTerm,
            SortBy = sortBy,
            IsDescending = isDescending,
            PageNumber = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get routes nearby a specific location
    /// </summary>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(List<RouteListResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<RouteListResponse>>> GetRoutesNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10,
        [FromQuery] string? difficulty = null,
        [FromQuery] decimal? minRating = null,
        [FromQuery] int maxResults = 50)
    {
        var query = new GetRoutesNearbyQuery
        {
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm,
            Difficulty = difficulty,
            MinRating = minRating,
            MaxResults = maxResults
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
    /// Update places in a route (order, duration, notes)
    /// </summary>
    [HttpPut("{id}/places")]
    [Authorize]
    [ProducesResponseType(typeof(RouteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RouteResponse>> UpdateRoutePlaces(Guid id, [FromBody] UpdateRouteRequest request)
    {
        var userId = GetUserId();

        // Reuse UpdateRouteCommand with just the places updated
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

    /// <summary>
    /// Duplicate a route
    /// </summary>
    [HttpPost("{id}/duplicate")]
    [Authorize]
    [ProducesResponseType(typeof(RouteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RouteResponse>> DuplicateRoute(Guid id)
    {
        var userId = GetUserId();

        try
        {
            // Get the original route
            var getQuery = new GetRouteByIdQuery { Id = id };
            var originalRoute = await _mediator.Send(getQuery);

            // Create a new route with the same data
            var command = new CreateRouteCommand
            {
                Name = $"{originalRoute.Name} (Copy)",
                Description = originalRoute.Description,
                Difficulty = originalRoute.Difficulty,
                Privacy = originalRoute.Privacy,
                EstimatedDuration = originalRoute.EstimatedDuration,
                TotalDistance = originalRoute.TotalDistance,
                ImageUrl = originalRoute.ImageUrl,
                Tags = originalRoute.Tags,
                Places = originalRoute.Places.Select(rp => new RoutePlaceRequest
                {
                    PlaceId = rp.PlaceId,
                    Order = rp.Order,
                    EstimatedTimeAtPlace = rp.EstimatedTimeAtPlace,
                    Notes = rp.Notes
                }).ToList(),
                UserId = userId
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetRouteById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Toggle publish status of a route
    /// </summary>
    [HttpPatch("{id}/publish")]
    [Authorize]
    [ProducesResponseType(typeof(RouteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RouteResponse>> TogglePublish(Guid id, [FromBody] TogglePublishRequest request)
    {
        var userId = GetUserId();

        try
        {
            // Get the route first
            var getQuery = new GetRouteByIdQuery { Id = id };
            var route = await _mediator.Send(getQuery);

            // Update with new publish status
            var command = new UpdateRouteCommand
            {
                Id = id,
                Name = route.Name,
                Description = route.Description,
                Difficulty = route.Difficulty,
                Privacy = route.Privacy,
                EstimatedDuration = route.EstimatedDuration,
                TotalDistance = route.TotalDistance,
                ImageUrl = route.ImageUrl,
                Tags = route.Tags,
                IsActive = request.IsPublished, // Use IsActive field for published status
                Places = route.Places.Select(rp => new RoutePlaceRequest
                {
                    PlaceId = rp.PlaceId,
                    Order = rp.Order,
                    EstimatedTimeAtPlace = rp.EstimatedTimeAtPlace,
                    Notes = rp.Notes
                }).ToList(),
                UserId = userId
            };

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
    }

    /// <summary>
    /// Get optimized route order for given places
    /// </summary>
    [HttpPost("optimize")]
    [Authorize]
    [ProducesResponseType(typeof(OptimizeRouteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<OptimizeRouteResponse> OptimizeRoute([FromBody] OptimizeRouteRequest request)
    {
        // TODO: Implement proper route optimization algorithm (e.g., using Google Maps Directions API or TSP algorithm)
        // For now, return the original order
        _logger.LogWarning("Route optimization not implemented yet. Returning original order.");
        
        return Ok(new OptimizeRouteResponse
        {
            OptimizedOrder = request.PlaceIds
        });
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }
}

/// <summary>
/// Request model for toggling publish status
/// </summary>
public class TogglePublishRequest
{
    public bool IsPublished { get; set; }
}

/// <summary>
/// Request model for route optimization
/// </summary>
public class OptimizeRouteRequest
{
    public List<Guid> PlaceIds { get; set; } = new();
}

/// <summary>
/// Response model for route optimization
/// </summary>
public class OptimizeRouteResponse
{
    public List<Guid> OptimizedOrder { get; set; } = new();
}
