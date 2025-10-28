using MediatR;
using Tripilot.Application.DTOs.Place;

namespace Tripilot.Application.Features.Places.Queries;

public class GetPlacesNearbyQuery : IRequest<List<PlaceListResponse>>
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RadiusKm { get; set; } = 10; // Default 10km radius
    public string? Category { get; set; }
    public decimal? MinRating { get; set; }
    public int MaxResults { get; set; } = 50;
}
