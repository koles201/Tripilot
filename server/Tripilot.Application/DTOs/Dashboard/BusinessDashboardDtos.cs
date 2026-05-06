using Tripilot.Application.DTOs.Business;
using Tripilot.Application.DTOs.Place;
using Tripilot.Application.DTOs.PlaceClaim;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.DTOs.Dashboard;

public class BusinessDashboardResponse
{
    public BusinessProfileResponse? BusinessProfile { get; set; }
    public BusinessAnalytics Analytics { get; set; } = new();
    public List<PlaceOverview> ClaimedPlaces { get; set; } = new();
    public List<PlaceClaimResponse> PendingClaims { get; set; } = new();
    public List<RecentActivity> RecentActivities { get; set; } = new();
}

public class BusinessAnalytics
{
    public int TotalPlaces { get; set; }
    /// <summary>
    /// Total number of views across all claimed places. Must be populated in both dashboard and analytics queries.
    /// </summary>
    public int TotalViews { get; set; }
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    public int PendingClaims { get; set; }
    public int ApprovedClaims { get; set; }
    public int RejectedClaims { get; set; }
    public PlaceViewsAnalytics ViewsAnalytics { get; set; } = new();
    public List<TopPlace> TopPlaces { get; set; } = new();
}

public class PlaceViewsAnalytics
{
    public int TodayViews { get; set; }
    public int WeekViews { get; set; }
    public int MonthViews { get; set; }
    public List<DailyViews> Last30Days { get; set; } = new();
}

public class DailyViews
{
    public DateTime Date { get; set; }
    public int ViewCount { get; set; }
}

public class TopPlace
{
    public Guid PlaceId { get; set; }
    public string PlaceName { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Reviews { get; set; }
    public decimal? AverageRating { get; set; }
}

public class PlaceOverview
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PlaceCategory Category { get; set; }
    public bool IsVerified { get; set; }
    public bool IsActive { get; set; }
    public int ReviewCount { get; set; }
    public decimal? AverageRating { get; set; }
    public DateTime ClaimedAt { get; set; }
}

public class RecentActivity
{
    public Guid Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? RelatedEntity { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public DateTime Timestamp { get; set; }
}
