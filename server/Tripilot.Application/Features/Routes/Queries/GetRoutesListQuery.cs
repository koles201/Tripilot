using MediatR;
using Tripilot.Application.DTOs.Common;
using Tripilot.Application.DTOs.Route;

namespace Tripilot.Application.Features.Routes.Queries;

/// <summary>
/// Query to get a list of routes
/// </summary>
public class GetRoutesListQuery : IRequest<PaginatedResult<RouteListResponse>>
{
    public string? Difficulty { get; set; }
    public string? Privacy { get; set; }
    public Guid? CreatorId { get; set; }
    public bool? IsFeatured { get; set; }
    public decimal? MinRating { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool IsDescending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
