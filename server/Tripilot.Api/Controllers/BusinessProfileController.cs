using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.Business;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.Features.BusinessProfiles.Commands;
using Tripilot.Application.Features.BusinessProfiles.Commands.Handlers;
using Tripilot.Application.Features.BusinessProfiles.Queries;
using Tripilot.Application.Features.BusinessProfiles.Queries.Handlers;
using Tripilot.Domain.Enums;

namespace Tripilot.Api.Controllers;

[ApiController]
[Route("api/business-profiles")]
[Produces("application/json")]
public class BusinessProfileController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BusinessProfileController> _logger;

    public BusinessProfileController(IMediator mediator, ILogger<BusinessProfileController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(BusinessProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<BusinessProfileResponse?>> GetMy()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetMyBusinessProfileQuery { UserId = userId });
        if (result == null) return NoContent();
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(BusinessProfileResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<BusinessProfileResponse>> Create([FromBody] CreateBusinessProfileRequest request)
    {
        var userId = GetUserId();
        var command = new CreateBusinessProfileCommand
        {
            UserId = userId,
            BusinessName = request.BusinessName,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            PostalCode = request.PostalCode,
            Phone = request.Phone,
            Email = request.Email,
            Website = request.Website,
            LogoUrl = request.LogoUrl,
            BannerUrl = request.BannerUrl
        };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMy), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(BusinessProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BusinessProfileResponse>> Update(Guid id, [FromBody] UpdateBusinessProfileRequest request)
    {
        var userId = GetUserId();
        var command = new UpdateBusinessProfileCommand
        {
            Id = id,
            UserId = userId,
            BusinessName = request.BusinessName,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            PostalCode = request.PostalCode,
            Phone = request.Phone,
            Email = request.Email,
            Website = request.Website,
            LogoUrl = request.LogoUrl,
            BannerUrl = request.BannerUrl
        };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/submit-verification")]
    [Authorize]
    [ProducesResponseType(typeof(BusinessProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BusinessProfileResponse>> SubmitVerification(Guid id, [FromBody] SubmitVerificationRequest request)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new SubmitVerificationCommand { Id = id, UserId = userId, DocumentUrls = request.DocumentUrls });
        return Ok(result);
    }

    [HttpPost("{id}/approve")]
    [Authorize]
    [ProducesResponseType(typeof(BusinessProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BusinessProfileResponse>> Approve(Guid id)
    {
        var (userId, role) = GetUserContext();
        var result = await _mediator.Send(new ApproveBusinessProfileCommand { Id = id, AdminUserId = userId, AdminRole = role });
        return Ok(result);
    }

    [HttpPost("{id}/reject")]
    [Authorize]
    [ProducesResponseType(typeof(BusinessProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BusinessProfileResponse>> Reject(Guid id, [FromBody] AdminDecisionRequest request)
    {
        var (userId, role) = GetUserContext();
        var result = await _mediator.Send(new RejectBusinessProfileCommand { Id = id, AdminUserId = userId, AdminRole = role, RejectionReason = request.RejectionReason });
        return Ok(result);
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(PaginatedResult<BusinessProfileResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<BusinessProfileResponse>>> GetList([FromQuery] VerificationStatus? status, [FromQuery] string? searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var (_, role) = GetUserContext();
        if (role != UserRole.Admin) return Forbid();
        var result = await _mediator.Send(new GetBusinessProfilesQuery { VerificationStatus = status, SearchTerm = searchTerm, Page = page, PageSize = pageSize });
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
