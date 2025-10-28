using MediatR;
using Tripilot.Application.DTOs.Review;

namespace Tripilot.Application.Features.Reviews.Commands;

public class UpdateReviewCommand : IRequest<ReviewResponse>
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public decimal OverallRating { get; set; }
    public decimal? CleanlinessRating { get; set; }
    public decimal? ServiceRating { get; set; }
    public decimal? ValueRating { get; set; }
    public decimal? LocationRating { get; set; }
    
    // User context
    public Guid ReviewerId { get; set; }
}
