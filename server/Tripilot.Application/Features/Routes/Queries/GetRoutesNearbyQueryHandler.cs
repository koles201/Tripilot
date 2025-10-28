using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.Route;
using Tripilot.Domain.Enums;

namespace Tripilot.Application.Features.Routes.Queries;

/// <summary>
/// Handler for getting routes near a specific location
/// </summary>
public class GetRoutesNearbyQueryHandler : IRequestHandler<GetRoutesNearbyQuery, List<RouteListResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRoutesNearbyQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<RouteListResponse>> Handle(GetRoutesNearbyQuery request, CancellationToken cancellationToken)
    {
        // Get all active public routes with their first place (for location)
        var routesQuery = _unitOfWork.Repository<Domain.Entities.Route>()
            .GetQueryable()
            .Include(r => r.Creator)
            .Include(r => r.RoutePlaces.OrderBy(rp => rp.Order).Take(1))
                .ThenInclude(rp => rp.Place)
            .Where(r => r.IsActive && r.Privacy == RoutePrivacy.Public);

        // Filter by difficulty if specified
        if (!string.IsNullOrEmpty(request.Difficulty) && Enum.TryParse<RouteDifficulty>(request.Difficulty, true, out var difficulty))
        {
            routesQuery = routesQuery.Where(r => r.Difficulty == difficulty);
        }

        // Filter by minimum rating if specified
        if (request.MinRating.HasValue)
        {
            routesQuery = routesQuery.Where(r => r.AverageRating >= request.MinRating.Value);
        }

        var routes = await routesQuery.ToListAsync(cancellationToken);

        // Calculate distances and filter routes within radius
        // Note: In production, consider using PostGIS for better performance with geospatial queries
        var routesWithDistance = routes
            .Where(r => r.RoutePlaces.Any()) // Only routes with at least one place
            .Select(route =>
            {
                var firstPlace = route.RoutePlaces.OrderBy(rp => rp.Order).First().Place;
                var distance = CalculateDistance(
                    request.Latitude,
                    request.Longitude,
                    firstPlace.Location.Latitude,
                    firstPlace.Location.Longitude);
                return new { Route = route, Distance = distance };
            })
            .Where(x => x.Distance <= request.RadiusKm)
            .OrderBy(x => x.Distance)
            .Take(request.MaxResults)
            .ToList();

        return _mapper.Map<List<RouteListResponse>>(routesWithDistance.Select(x => x.Route).ToList());
    }

    /// <summary>
    /// Calculate distance between two points using Haversine formula
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
        return degrees * Math.PI / 180;
    }
}
