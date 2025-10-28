using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Review;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Reviews.Queries;

public class GetReviewsForRouteQueryHandler : IRequestHandler<GetReviewsForRouteQuery, List<ReviewResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetReviewsForRouteQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ReviewResponse>> Handle(GetReviewsForRouteQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Review>()
            .GetQueryable()
            .Include(r => r.Reviewer)
            .Include(r => r.Route)
            .Where(r => r.RouteId == request.RouteId && r.IsActive);

        // Filter by rating
        if (request.MinRating.HasValue)
        {
            query = query.Where(r => r.OverallRating >= request.MinRating.Value);
        }

        // Sorting
        query = request.SortBy?.ToLower() switch
        {
            "rating" => request.IsDescending ? query.OrderByDescending(r => r.OverallRating) : query.OrderBy(r => r.OverallRating),
            "helpful" => request.IsDescending ? query.OrderByDescending(r => r.HelpfulCount) : query.OrderBy(r => r.HelpfulCount),
            _ => request.IsDescending ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt)
        };

        // Pagination
        var reviews = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var response = _mapper.Map<List<ReviewResponse>>(reviews);
        return response;
    }
}
