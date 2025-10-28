using System.ComponentModel.DataAnnotations;

namespace Tripilot.Application.DTOs.Route;

/// <summary>
/// Request DTO for updating an existing route
/// </summary>
public class UpdateRouteRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Difficulty { get; set; } = string.Empty;

    [Required]
    public string Privacy { get; set; } = string.Empty;

    [Required]
    [Range(1, 10000)]
    public int EstimatedDuration { get; set; }

    [Range(0, 10000)]
    public decimal? TotalDistance { get; set; }

    [Url]
    [StringLength(1000)]
    public string? ImageUrl { get; set; }

    [StringLength(1000)]
    public string? Tags { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public List<RoutePlaceRequest> Places { get; set; } = new();
}
