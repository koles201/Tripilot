using MediatR;
using Tripilot.Application.DTOs.Favorite;

namespace Tripilot.Application.Features.Favorites.Commands;

/// <summary>
/// Command to add a place to user's favorites
/// </summary>
public class AddFavoriteCommand : IRequest<FavoriteResponse>
{
    /// <summary>
    /// ID of the place to favorite
    /// </summary>
    public Guid PlaceId { get; set; }

    /// <summary>
    /// ID of the user adding the favorite
    /// </summary>
    public Guid UserId { get; set; }
}
