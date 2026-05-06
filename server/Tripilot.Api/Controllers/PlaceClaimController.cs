using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.PlaceClaim;
using Tripilot.Application.Features.PlaceClaims.Commands;
using Tripilot.Application.Features.PlaceClaims.Queries;
using Tripilot.Domain.Enums;

namespace Tripilot.Api.Controllers;

[ApiController]
[Route("api/place-claims")]
[Produces("application/json")]
public class PlaceClaimController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PlaceClaimController> _logger;

    public PlaceClaimController(IMediator mediator, ILogger<PlaceClaimController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(PlaceClaimResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyClaims([FromQuery] ClaimStatus? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetMyPlaceClaimsQuery 
        { 
            UserId = userId, 
            Status = status, 
            Page = page, 
            PageSize = pageSize 
        });
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(PlaceClaimResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> SubmitClaim([FromBody] SubmitPlaceClaimRequest request)
    {
        var userId = GetUserId();
        var command = new SubmitPlaceClaimCommand
        {
            UserId = userId,
            PlaceId = request.PlaceId,
            ClaimReason = request.ClaimReason,
            DocumentUrls = request.DocumentUrls
        };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMyClaims), new { id = result.Id }, result);
    }

    [HttpPost("{id}/approve")]
    [Authorize]
    [ProducesResponseType(typeof(PlaceClaimResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveClaim(Guid id, [FromBody] ReviewPlaceClaimRequest request)
    {
        var (userId, role) = GetUserContext();
        var result = await _mediator.Send(new ApprovePlaceClaimCommand 
        { 
            ClaimId = id, 
            AdminUserId = userId, 
            AdminRole = role, 
            DecisionReason = request.DecisionReason 
        });
        return Ok(result);
    }

    [HttpPost("{id}/reject")]
    [Authorize]
    [ProducesResponseType(typeof(PlaceClaimResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectClaim(Guid id, [FromBody] ReviewPlaceClaimRequest request)
    {
        var (userId, role) = GetUserContext();
        var result = await _mediator.Send(new RejectPlaceClaimCommand 
        { 
            ClaimId = id, 
            AdminUserId = userId, 
            AdminRole = role, 
            RejectionReason = request.DecisionReason 
        });
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PlaceClaimResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllClaims([FromQuery] ClaimStatus? status, [FromQuery] string? searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var (_, role) = GetUserContext();
        if (role != UserRole.Admin) return Forbid();
        
        var result = await _mediator.Send(new GetAllPlaceClaimsQuery 
        { 
            Status = status, 
            SearchTerm = searchTerm, 
            Page = page, 
            PageSize = pageSize 
        });
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }

    private (Guid userId, UserRole role) GetUserContext()
    {
        var id = GetUserId();
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        return (id, Enum.TryParse<UserRole>(roleClaim, true, out var role) ? role : UserRole.Tourist);
    }
}
