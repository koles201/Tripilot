using MediatR;
using Tripilot.Application.DTOs.Route;

namespace Tripilot.Application.Features.Routes.Queries;

/// <summary>
/// Query to get routes near a specific location
/// </summary>
public class GetRoutesNearbyQuery : IRequest<List<RouteListResponse>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 10; // Default 10km radius
    public string? Difficulty { get; set; }
    public decimal? MinRating { get; set; }
    public int MaxResults { get; set; } = 50; // Default max 50 routes
}
