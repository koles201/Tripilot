using MediatR;
using Tripilot.Application.DTOs.Favorite;

namespace Tripilot.Application.Features.Favorites.Queries;

/// <summary>
/// Query to get all favorites for a user
/// </summary>
public class GetUserFavoritesQuery : IRequest<FavoriteListResult>
{
    /// <summary>
    /// ID of the user
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Filter by category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Sort by: createdAt, name, rating
    /// </summary>
    public string? SortBy { get; set; } = "createdAt";

    /// <summary>
    /// Sort direction: asc, desc
    /// </summary>
    public string? SortDirection { get; set; } = "desc";
}
