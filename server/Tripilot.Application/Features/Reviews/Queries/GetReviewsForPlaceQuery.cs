using MediatR;
using Tripilot.Application.DTOs.Review;

namespace Tripilot.Application.Features.Reviews.Queries;

public class GetReviewsForPlaceQuery : IRequest<List<ReviewResponse>>
{
    public Guid PlaceId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public decimal? MinRating { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool IsDescending { get; set; } = true;
}
