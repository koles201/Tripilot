using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Place;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Places.Queries;

public class GetPlacesNearbyQueryHandler : IRequestHandler<GetPlacesNearbyQuery, List<PlaceListResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetPlacesNearbyQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<PlaceListResponse>> Handle(GetPlacesNearbyQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.Repository<Place>()
            .GetQueryable()
            .Where(p => p.IsActive);

        // Filter by category
        if (!string.IsNullOrEmpty(request.Category) && Enum.TryParse<PlaceCategory>(request.Category, true, out var category))
        {
            query = query.Where(p => p.Category == category);
        }

        // Filter by rating
        if (request.MinRating.HasValue)
        {
            query = query.Where(p => p.AverageRating >= request.MinRating.Value);
        }

        // Get all places (we'll filter by distance in memory for simplicity)
        // In production, you'd use PostGIS or similar for geospatial queries
        var places = await query.ToListAsync(cancellationToken);

        // Calculate distance and filter by radius
        var nearbyPlaces = places
            .Select(p => new
            {
                Place = p,
                Distance = CalculateDistance(request.Latitude, request.Longitude, 
                    p.Location.Latitude, p.Location.Longitude)
            })
            .Where(x => x.Distance <= request.RadiusKm)
            .OrderBy(x => x.Distance)
            .Take(request.MaxResults)
            .Select(x => x.Place)
            .ToList();

        var response = _mapper.Map<List<PlaceListResponse>>(nearbyPlaces);
        return response;
    }

    /// <summary>
    /// Calculate distance between two coordinates using Haversine formula
    /// </summary>
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
