using MediatR;
using Tripilot.Application.DTOs.Favorite;

namespace Tripilot.Application.Features.Favorites.Queries;

/// <summary>
/// Query to check if a place is favorited by a user
/// </summary>
public class IsFavoriteQuery : IRequest<IsFavoriteResponse>
{
    /// <summary>
    /// ID of the user
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ID of the place to check
    /// </summary>
    public Guid PlaceId { get; set; }
}
