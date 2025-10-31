using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Business;
using Tripilot.Application.DTOs.Dashboard;
using Tripilot.Application.DTOs.PlaceClaim;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Dashboard.Queries.Handlers;

public class GetBusinessDashboardQueryHandler : IRequestHandler<GetBusinessDashboardQuery, BusinessDashboardResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetBusinessDashboardQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<BusinessDashboardResponse> Handle(GetBusinessDashboardQuery request, CancellationToken cancellationToken)
    {
        var response = new BusinessDashboardResponse();

        // Get business profile
        var businessProfile = await _unitOfWork.Repository<BusinessProfile>()
            .GetQueryable()
            .FirstOrDefaultAsync(bp => bp.UserId == request.UserId, cancellationToken);

        if (businessProfile != null)
        {
            response.BusinessProfile = _mapper.Map<BusinessProfileResponse>(businessProfile);
        }

        // Get all places owned by the user
        var ownedPlaces = await _unitOfWork.Repository<Place>()
            .GetQueryable()
            .Where(p => p.OwnerId == request.UserId)
            .Include(p => p.Reviews)
            .ToListAsync(cancellationToken);

        // Get approved claims to find claim dates
        var approvedClaims = await _unitOfWork.Repository<PlaceClaim>()
            .GetQueryable()
            .Where(pc => pc.ClaimantUserId == request.UserId && pc.Status == ClaimStatus.Approved)
            .ToListAsync(cancellationToken);

        var claimDateDict = approvedClaims.ToDictionary(c => c.PlaceId, c => c.ReviewedAt ?? c.SubmittedAt);

        // Map claimed places with overview data
        response.ClaimedPlaces = ownedPlaces.Select(p => new PlaceOverview
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Category = p.Category,
            IsVerified = p.IsVerified,
            IsActive = p.IsActive,
            ReviewCount = p.Reviews?.Count ?? 0,
            AverageRating = p.Reviews?.Any() == true ? (decimal)p.Reviews.Average(r => r.OverallRating) : null,
            ClaimedAt = claimDateDict.TryGetValue(p.Id, out var claimDate) ? claimDate : p.CreatedAt
        }).OrderByDescending(p => p.ClaimedAt).ToList();

        // Get pending claims
        var pendingClaims = await _unitOfWork.Repository<PlaceClaim>()
            .GetQueryable()
            .Where(pc => pc.ClaimantUserId == request.UserId && 
                        (pc.Status == ClaimStatus.Pending || pc.Status == ClaimStatus.UnderReview))
            .Include(pc => pc.Place)
            .Include(pc => pc.BusinessProfile)
            .OrderByDescending(pc => pc.SubmittedAt)
            .ToListAsync(cancellationToken);

        response.PendingClaims = _mapper.Map<List<PlaceClaimResponse>>(pendingClaims);

        // Calculate analytics
        var totalReviews = ownedPlaces.Sum(p => p.Reviews?.Count ?? 0);
        var allReviews = ownedPlaces.SelectMany(p => p.Reviews ?? new List<Review>()).ToList();
        
        response.Analytics = new BusinessAnalytics
        {
            TotalPlaces = ownedPlaces.Count,
            TotalReviews = totalReviews,
            AverageRating = allReviews.Any() ? (decimal)allReviews.Average(r => r.OverallRating) : 0,
            PendingClaims = pendingClaims.Count(pc => pc.Status == ClaimStatus.Pending),
            ApprovedClaims = approvedClaims.Count,
            TopPlaces = ownedPlaces
                .OrderByDescending(p => p.Reviews?.Count ?? 0)
                .Take(5)
                .Select(p => new TopPlace
                {
                    PlaceId = p.Id,
                    PlaceName = p.Name,
                    Reviews = p.Reviews?.Count ?? 0,
                    AverageRating = p.Reviews?.Any() == true ? (decimal)p.Reviews.Average(r => r.OverallRating) : null
                })
                .ToList()
        };

        // Get all claims for this user
        var allClaims = await _unitOfWork.Repository<PlaceClaim>()
            .GetQueryable()
            .Where(pc => pc.ClaimantUserId == request.UserId)
            .ToListAsync(cancellationToken);

        response.Analytics.RejectedClaims = allClaims.Count(c => c.Status == ClaimStatus.Rejected);

        // Generate recent activities
        response.RecentActivities = new List<RecentActivity>();

        // Add recent reviews as activities
        var recentReviews = allReviews
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
            .Select(r => new RecentActivity
            {
                Id = r.Id,
                ActivityType = "Review",
                Description = string.IsNullOrEmpty(r.Content)
                    ? "New review received:"
                    : $"New review received: {(r.Content.Length > 50 ? r.Content.Substring(0, 50) + "..." : r.Content)}",
                RelatedEntity = "Place",
                RelatedEntityId = r.PlaceId,
                Timestamp = r.CreatedAt
            });

        response.RecentActivities.AddRange(recentReviews);

        // Add recent claims as activities
        var recentClaimActivities = allClaims
            .OrderByDescending(c => c.ReviewedAt ?? c.SubmittedAt)
            .Take(5)
            .Select(c => new RecentActivity
            {
                Id = c.Id,
                ActivityType = c.Status.ToString(),
                Description = c.Status switch
                {
                    ClaimStatus.Approved => "Place claim approved",
                    ClaimStatus.Rejected => "Place claim rejected",
                    ClaimStatus.UnderReview => "Place claim under review",
                    ClaimStatus.Pending => "Place claim submitted",
                    _ => "Place claim updated"
                },
                RelatedEntity = "PlaceClaim",
                RelatedEntityId = c.Id,
                Timestamp = c.ReviewedAt ?? c.SubmittedAt
            });

        response.RecentActivities.AddRange(recentClaimActivities);
        response.RecentActivities = response.RecentActivities
            .OrderByDescending(a => a.Timestamp)
            .Take(10)
            .ToList();

        return response;
    }
}
