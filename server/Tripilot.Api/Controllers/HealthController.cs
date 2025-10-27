using Microsoft.AspNetCore.Mvc;

namespace Tripilot.Api.Controllers;

/// <summary>
/// Health check controller for monitoring API status
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Basic health check endpoint
    /// </summary>
    /// <returns>Health status</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        _logger.LogInformation("Health check requested");

        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "Tripilot API",
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Detailed health check endpoint
    /// </summary>
    /// <returns>Detailed health information</returns>
    [HttpGet("detailed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetDetailed()
    {
        _logger.LogInformation("Detailed health check requested");

        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            service = "Tripilot API",
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(),
            checks = new
            {
                database = "Not configured yet",
                cache = "Not configured yet"
            }
        });
    }
}
