using MediatR;

namespace Tripilot.Application.Features.Favorites.Commands;

/// <summary>
/// Command to remove a place from user's favorites
/// </summary>
public class RemoveFavoriteCommand : IRequest<bool>
{
    /// <summary>
    /// ID of the place to unfavorite
    /// </summary>
    public Guid PlaceId { get; set; }

    /// <summary>
    /// ID of the user removing the favorite
    /// </summary>
    public Guid UserId { get; set; }
}
