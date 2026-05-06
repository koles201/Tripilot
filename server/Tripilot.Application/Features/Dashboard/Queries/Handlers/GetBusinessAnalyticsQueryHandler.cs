using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Dashboard;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Dashboard.Queries.Handlers;

public class GetBusinessAnalyticsQueryHandler : IRequestHandler<GetBusinessAnalyticsQuery, BusinessAnalytics>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBusinessAnalyticsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BusinessAnalytics> Handle(GetBusinessAnalyticsQuery request, CancellationToken cancellationToken)
    {
        var endDate = request.EndDate ?? DateTime.UtcNow;
        var startDate = request.StartDate ?? endDate.AddDays(-30);

        // Get all places owned by the user
        var ownedPlaces = await _unitOfWork.Repository<Place>()
            .GetQueryable()
            .Where(p => p.OwnerId == request.UserId)
            .Include(p => p.Reviews)
            .ToListAsync(cancellationToken);

        // Get all claims
        var claims = await _unitOfWork.Repository<PlaceClaim>()
            .GetQueryable()
            .Where(pc => pc.ClaimantUserId == request.UserId)
            .ToListAsync(cancellationToken);

        // Filter reviews by date range
        var allReviews = ownedPlaces
            .SelectMany(p => p.Reviews ?? new List<Review>())
            .Where(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate)
            .ToList();
        var totalReviews = allReviews.Count;
        var averageRating = allReviews.Any() ? (decimal)allReviews.Average(r => r.OverallRating) : 0;

        // Calculate top places within the date range
        var topPlaces = ownedPlaces
            .Select(p => new
            {
                Place = p,
                FilteredReviews = (p.Reviews ?? new List<Review>()).Where(r => r.CreatedAt >= startDate && r.CreatedAt <= endDate).ToList()
            })
            .OrderByDescending(x => x.FilteredReviews.Count)
            .Take(10)
            .Select(x => new TopPlace
            {
                PlaceId = x.Place.Id,
                PlaceName = x.Place.Name,
                Reviews = x.FilteredReviews.Count,
                AverageRating = x.FilteredReviews.Any() ? (decimal?)x.FilteredReviews.Average(r => r.OverallRating) : null
            })
            .ToList();

        // Note: Views analytics would require a separate tracking system.
        // For now, using deterministic placeholder data based on reviews and date to ensure consistency.
        var viewsAnalytics = new PlaceViewsAnalytics
        {
            TodayViews = totalReviews > 0 ? totalReviews * 10 : 0,
            WeekViews = totalReviews > 0 ? totalReviews * 50 : 0,
            MonthViews = totalReviews > 0 ? totalReviews * 200 : 0,
            Last30Days = Enumerable.Range(0, 30)
                .Select(i => {
                    var date = endDate.AddDays(-29 + i).Date;
                    // Use a deterministic seed based on the date to ensure consistent placeholder data
                    var seed = date.GetHashCode();
                    var rng = new Random(seed);
                    return new DailyViews
                    {
                        Date = date,
                        ViewCount = totalReviews > 0 ? rng.Next(5, 50) : 0
                    };
                })
                .ToList()
        };

        return new BusinessAnalytics
        {
            TotalPlaces = ownedPlaces.Count,
            TotalReviews = totalReviews,
            AverageRating = averageRating,
            PendingClaims = claims.Count(c => c.Status == ClaimStatus.Pending),
            ApprovedClaims = claims.Count(c => c.Status == ClaimStatus.Approved),
            RejectedClaims = claims.Count(c => c.Status == ClaimStatus.Rejected),
            ViewsAnalytics = viewsAnalytics,
            TopPlaces = topPlaces
        };
    }
}
