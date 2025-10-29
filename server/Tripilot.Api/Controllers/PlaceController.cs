using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.Place;
using Tripilot.Application.Features.Places.Commands;
using Tripilot.Application.Features.Places.Queries;

namespace Tripilot.Api.Controllers;

/// <summary>
/// Controller for place management
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PlaceController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlaceController> _logger;

    public PlaceController(IMediator mediator, ILogger<PlaceController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all places with filtering and pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PlacesListResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<PlacesListResult>> GetPlaces(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? category = null,
        [FromQuery] string? city = null,
        [FromQuery] string? country = null,
        [FromQuery] decimal? minRating = null,
        [FromQuery] int? priceLevel = null,
        [FromQuery] bool? isVerified = null,
        [FromQuery] string? sortBy = null)
    {
        var query = new GetPlacesListQuery
        {
            Page = page,
            PageSize = pageSize,
            Category = category,
            City = city,
            Country = country,
            MinRating = minRating,
            PriceLevel = priceLevel,
            IsVerified = isVerified,
            SortBy = sortBy
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a place by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PlaceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlaceResponse>> GetPlaceById(Guid id)
    {
        var query = new GetPlaceByIdQuery { PlaceId = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound(new { message = "Place not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Search places by name, description, or location
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(PlacesListResult), StatusCodes.Status200OK)]
    public async Task<ActionResult<PlacesListResult>> SearchPlaces(
        [FromQuery] string searchTerm = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? category = null,
        [FromQuery] double? latitude = null,
        [FromQuery] double? longitude = null,
        [FromQuery] double? radiusKm = null)
    {
        var query = new SearchPlacesQuery
        {
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize,
            Category = category,
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get places nearby a specific location
    /// </summary>
    [HttpGet("nearby")]
    [ProducesResponseType(typeof(List<PlaceListResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PlaceListResponse>>> GetPlacesNearby(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10,
        [FromQuery] string? category = null,
        [FromQuery] decimal? minRating = null,
        [FromQuery] int maxResults = 50)
    {
        var query = new GetPlacesNearbyQuery
        {
            Latitude = latitude,
            Longitude = longitude,
            RadiusKm = radiusKm,
            Category = category,
            MinRating = minRating,
            MaxResults = maxResults
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new place
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(PlaceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PlaceResponse>> CreatePlace([FromBody] CreatePlaceRequest request)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        var command = new CreatePlaceCommand
        {
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            PostalCode = request.PostalCode,
            Phone = request.Phone,
            Email = request.Email,
            Website = request.Website,
            OpenTime = request.OpenTime,
            CloseTime = request.CloseTime,
            DaysOfWeek = request.DaysOfWeek,
            Is24Hours = request.Is24Hours,
            SpecialNotes = request.SpecialNotes,
            PriceLevel = request.PriceLevel,
            Amenities = request.Amenities,
            ImageUrl = request.ImageUrl,
            GalleryImages = request.GalleryImages,
            UserId = userId,
            UserRole = userRole
        };

        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetPlaceById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing place
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(PlaceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlaceResponse>> UpdatePlace(Guid id, [FromBody] UpdatePlaceRequest request)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        var command = new UpdatePlaceCommand
        {
            PlaceId = id,
            Name = request.Name,
            Description = request.Description,
            Category = request.Category,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            PostalCode = request.PostalCode,
            Phone = request.Phone,
            Email = request.Email,
            Website = request.Website,
            OpenTime = request.OpenTime,
            CloseTime = request.CloseTime,
            DaysOfWeek = request.DaysOfWeek,
            Is24Hours = request.Is24Hours,
            SpecialNotes = request.SpecialNotes,
            PriceLevel = request.PriceLevel,
            Amenities = request.Amenities,
            ImageUrl = request.ImageUrl,
            GalleryImages = request.GalleryImages,
            IsActive = request.IsActive,
            UserId = userId,
            UserRole = userRole
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
    /// Delete a place (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePlace(Guid id)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        var command = new DeletePlaceCommand
        {
            PlaceId = id,
            UserId = userId,
            UserRole = userRole
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

    private string GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? "Tourist";
    }
}
