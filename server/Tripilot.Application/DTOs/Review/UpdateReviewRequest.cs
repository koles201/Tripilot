using System.ComponentModel.DataAnnotations;

namespace Tripilot.Application.DTOs.Review;

/// <summary>
/// Request DTO for updating an existing review
/// </summary>
public class UpdateReviewRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Content { get; set; } = string.Empty;

    [Required]
    [Range(1, 5)]
    public decimal OverallRating { get; set; }

    [Range(1, 5)]
    public decimal? CleanlinessRating { get; set; }

    [Range(1, 5)]
    public decimal? ServiceRating { get; set; }

    [Range(1, 5)]
    public decimal? ValueRating { get; set; }

    [Range(1, 5)]
    public decimal? LocationRating { get; set; }
}
