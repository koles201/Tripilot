using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tripilot.Application.DTOs.Dashboard;
using Tripilot.Application.Features.Dashboard.Queries;

namespace Tripilot.Api.Controllers;

[ApiController]
[Route("api/business-dashboard")]
[Produces("application/json")]
[Authorize]
public class BusinessDashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BusinessDashboardController> _logger;

    public BusinessDashboardController(IMediator mediator, ILogger<BusinessDashboardController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive business dashboard data
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(BusinessDashboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDashboard()
    {
        var userId = GetUserId();
        var query = new GetBusinessDashboardQuery { UserId = userId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get detailed analytics with date range
    /// </summary>
    [HttpGet("analytics")]
    [ProducesResponseType(typeof(BusinessAnalytics), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAnalytics(
        [FromQuery] DateTime? startDate, 
        [FromQuery] DateTime? endDate)
    {
        var userId = GetUserId();
        var query = new GetBusinessAnalyticsQuery 
        { 
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate
        };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("User ID not found in token");
        }
        return userId;
    }
}
