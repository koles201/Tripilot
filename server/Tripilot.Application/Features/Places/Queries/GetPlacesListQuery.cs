using MediatR;
using Tripilot.Application.DTOs.Place;

namespace Tripilot.Application.Features.Places.Queries;

/// <summary>
/// Query to get a paginated list of places
/// </summary>
public class GetPlacesListQuery : IRequest<PlacesListResult>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Category { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal? MinRating { get; set; }
    public int? PriceLevel { get; set; }
    public bool? IsVerified { get; set; }
    public string? SortBy { get; set; }
}

/// <summary>
/// Result for places list query
/// </summary>
public class PlacesListResult
{
    public List<PlaceListResponse> Places { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
