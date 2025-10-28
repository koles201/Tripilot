using MediatR;
using Tripilot.Application.DTOs.Place;

namespace Tripilot.Application.Features.Places.Queries;

/// <summary>
/// Query to search places by name or description
/// </summary>
public class SearchPlacesQuery : IRequest<PlacesListResult>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Category { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusKm { get; set; }
}
