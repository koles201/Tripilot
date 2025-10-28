using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Review;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Reviews.Commands;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ReviewResponse> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.Repository<Review>()
            .GetQueryable()
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.IsActive, cancellationToken);

        if (review == null)
        {
            throw new KeyNotFoundException($"Review with ID {request.Id} not found");
        }

        // Check authorization
        if (review.ReviewerId != request.ReviewerId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this review");
        }

        // Update review properties
        review.Title = request.Title;
        review.Content = request.Content;
        review.OverallRating = request.OverallRating;
        review.CleanlinessRating = request.CleanlinessRating;
        review.ServiceRating = request.ServiceRating;
        review.ValueRating = request.ValueRating;
        review.LocationRating = request.LocationRating;

        _unitOfWork.Repository<Review>().Update(review);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Update aggregate ratings
        await UpdateAggregateRatings(review.PlaceId, review.RouteId, cancellationToken);

        // Reload with related data
        var updatedReview = await _unitOfWork.Repository<Review>()
            .GetQueryable()
            .Include(r => r.Reviewer)
            .Include(r => r.Place)
            .Include(r => r.Route)
            .FirstOrDefaultAsync(r => r.Id == review.Id, cancellationToken);

        var response = _mapper.Map<ReviewResponse>(updatedReview);
        return response;
    }

    private async Task UpdateAggregateRatings(Guid? placeId, Guid? routeId, CancellationToken cancellationToken)
    {
        if (placeId.HasValue)
        {
            var place = await _unitOfWork.Repository<Place>()
                .GetQueryable()
                .Include(p => p.Reviews.Where(r => r.IsActive))
                .FirstOrDefaultAsync(p => p.Id == placeId.Value, cancellationToken);

            if (place != null)
            {
                var reviews = place.Reviews.ToList();
                place.ReviewCount = reviews.Count;
                place.AverageRating = reviews.Any() ? reviews.Average(r => r.OverallRating) : 0;
                
                _unitOfWork.Repository<Place>().Update(place);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
        else if (routeId.HasValue)
        {
            var route = await _unitOfWork.Repository<Domain.Entities.Route>()
                .GetQueryable()
                .Include(r => r.Reviews.Where(rev => rev.IsActive))
                .FirstOrDefaultAsync(r => r.Id == routeId.Value, cancellationToken);

            if (route != null)
            {
                var reviews = route.Reviews.ToList();
                route.ReviewCount = reviews.Count;
                route.AverageRating = reviews.Any() ? reviews.Average(r => r.OverallRating) : 0;
                
                _unitOfWork.Repository<Domain.Entities.Route>().Update(route);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
