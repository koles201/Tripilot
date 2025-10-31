using MediatR;
using Tripilot.Application.DTOs.Place;

namespace Tripilot.Application.Features.Places.Queries;

/// <summary>
/// Query for advanced place search with comprehensive filtering
/// </summary>
public class AdvancedSearchQuery : IRequest<AdvancedSearchResult>
{
    public string? SearchTerm { get; set; }
    public string? Category { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
    public int? MinPriceLevel { get; set; }
    public int? MaxPriceLevel { get; set; }
    public bool? IsVerified { get; set; }
    public string? Amenities { get; set; }
    public bool? IsOpenNow { get; set; }
    public bool? Is24Hours { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusKm { get; set; }
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
