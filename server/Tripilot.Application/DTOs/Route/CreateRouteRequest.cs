using System.ComponentModel.DataAnnotations;

namespace Tripilot.Application.DTOs.Route;

/// <summary>
/// Request DTO for creating a new route
/// </summary>
public class CreateRouteRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Difficulty { get; set; } = string.Empty; // Easy, Moderate, Challenging, Difficult

    [Required]
    public string Privacy { get; set; } = string.Empty; // Public, Private, Unlisted

    [Required]
    [Range(1, 10000)]
    public int EstimatedDuration { get; set; } // in minutes

    [Range(0, 10000)]
    public decimal? TotalDistance { get; set; } // in kilometers

    [Url]
    [StringLength(1000)]
    public string? ImageUrl { get; set; }

    [StringLength(1000)]
    public string? Tags { get; set; }

    [Required]
    public List<RoutePlaceRequest> Places { get; set; } = new();
}

/// <summary>
/// Request DTO for place in a route
/// </summary>
public class RoutePlaceRequest
{
    [Required]
    public Guid PlaceId { get; set; }

    [Required]
    [Range(1, 1000)]
    public int Order { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [Range(1, 10000)]
    public int? EstimatedTimeAtPlace { get; set; } // in minutes
}
