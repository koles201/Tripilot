namespace Tripilot.Application.DTOs.Favorite;

/// <summary>
/// Request to add a place to favorites
/// </summary>
public class AddFavoriteRequest
{
    /// <summary>
    /// ID of the place to favorite
    /// </summary>
    public Guid PlaceId { get; set; }
}

/// <summary>
/// Response for favorite operations
/// </summary>
public class FavoriteResponse
{
    /// <summary>
    /// Favorite ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User ID who favorited
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Place ID that was favorited
    /// </summary>
    public Guid PlaceId { get; set; }

    /// <summary>
    /// Place name
    /// </summary>
    public string PlaceName { get; set; } = string.Empty;

    /// <summary>
    /// Place category
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Place city
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Place country
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Place image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Place average rating
    /// </summary>
    public decimal? AverageRating { get; set; }

    /// <summary>
    /// When the favorite was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Paginated list of user's favorites
/// </summary>
public class FavoriteListResult
{
    /// <summary>
    /// List of favorites
    /// </summary>
    public List<FavoriteResponse> Favorites { get; set; } = new();

    /// <summary>
    /// Total count of favorites
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; set; }
}

/// <summary>
/// Result of checking if a place is favorited
/// </summary>
public class IsFavoriteResponse
{
    /// <summary>
    /// Whether the place is favorited by the user
    /// </summary>
    public bool IsFavorite { get; set; }

    /// <summary>
    /// Favorite ID if it exists
    /// </summary>
    public Guid? FavoriteId { get; set; }
}
