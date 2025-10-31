namespace Tripilot.Domain.Entities;

/// <summary>
/// Represents a user's favorite place (bookmark)
/// </summary>
public class Favorite
{
    /// <summary>
    /// Unique identifier for the favorite
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID of the user who favorited the place
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Navigation property to User
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// ID of the favorited place
    /// </summary>
    public Guid PlaceId { get; set; }

    /// <summary>
    /// Navigation property to Place
    /// </summary>
    public Place Place { get; set; } = null!;

    /// <summary>
    /// When the place was favorited
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
