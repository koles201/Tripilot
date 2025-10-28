using MediatR;
using Tripilot.Application.DTOs.Review;

namespace Tripilot.Application.Features.Reviews.Commands;

public class AddReviewCommand : IRequest<ReviewResponse>
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public decimal OverallRating { get; set; }
    public decimal? CleanlinessRating { get; set; }
    public decimal? ServiceRating { get; set; }
    public decimal? ValueRating { get; set; }
    public decimal? LocationRating { get; set; }
    public Guid? PlaceId { get; set; }
    public Guid? RouteId { get; set; }
    
    // User context (set by controller)
    public Guid ReviewerId { get; set; }
}
