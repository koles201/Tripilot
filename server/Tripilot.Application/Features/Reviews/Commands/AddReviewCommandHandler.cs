using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Review;
using Tripilot.Domain.Entities;

namespace Tripilot.Application.Features.Reviews.Commands;

public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, ReviewResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddReviewCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ReviewResponse> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        // Validate that either PlaceId or RouteId is provided, but not both
        if ((!request.PlaceId.HasValue && !request.RouteId.HasValue) ||
            (request.PlaceId.HasValue && request.RouteId.HasValue))
        {
            throw new ArgumentException("Must provide either PlaceId or RouteId, but not both");
        }

        // Check if place or route exists
        if (request.PlaceId.HasValue)
        {
            var placeExists = await _unitOfWork.Repository<Place>()
                .GetQueryable()
                .AnyAsync(p => p.Id == request.PlaceId.Value && p.IsActive, cancellationToken);

            if (!placeExists)
            {
                throw new KeyNotFoundException($"Place with ID {request.PlaceId.Value} not found");
            }

            // Check for duplicate review
            var existingReview = await _unitOfWork.Repository<Review>()
                .GetQueryable()
                .AnyAsync(r => r.PlaceId == request.PlaceId.Value && 
                              r.ReviewerId == request.ReviewerId && 
                              r.IsActive, cancellationToken);

            if (existingReview)
            {
                throw new InvalidOperationException("You have already reviewed this place");
            }
        }
        else if (request.RouteId.HasValue)
        {
            var routeExists = await _unitOfWork.Repository<Domain.Entities.Route>()
                .GetQueryable()
                .AnyAsync(r => r.Id == request.RouteId.Value && r.IsActive, cancellationToken);

            if (!routeExists)
            {
                throw new KeyNotFoundException($"Route with ID {request.RouteId.Value} not found");
            }

            // Check for duplicate review
            var existingReview = await _unitOfWork.Repository<Review>()
                .GetQueryable()
                .AnyAsync(r => r.RouteId == request.RouteId.Value && 
                              r.ReviewerId == request.ReviewerId && 
                              r.IsActive, cancellationToken);

            if (existingReview)
            {
                throw new InvalidOperationException("You have already reviewed this route");
            }
        }

        // Create review
        var review = new Review
        {
            Title = request.Title,
            Content = request.Content,
            OverallRating = request.OverallRating,
            CleanlinessRating = request.CleanlinessRating,
            ServiceRating = request.ServiceRating,
            ValueRating = request.ValueRating,
            LocationRating = request.LocationRating,
            PlaceId = request.PlaceId,
            RouteId = request.RouteId,
            ReviewerId = request.ReviewerId,
            IsActive = true,
            IsVerified = false,
            IsFlagged = false,
            HelpfulCount = 0,
            UnhelpfulCount = 0
        };

        await _unitOfWork.Repository<Review>().AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Update aggregate ratings
        await UpdateAggregateRatings(review.PlaceId, review.RouteId, cancellationToken);

        // Reload with related data
        var createdReview = await _unitOfWork.Repository<Review>()
            .GetQueryable()
            .Include(r => r.Reviewer)
            .Include(r => r.Place)
            .Include(r => r.Route)
            .FirstOrDefaultAsync(r => r.Id == review.Id, cancellationToken);

        var response = _mapper.Map<ReviewResponse>(createdReview);
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
