namespace Tripilot.Domain.Enums;

/// <summary>
/// Types of user activities tracked in the activity feed
/// </summary>
public enum ActivityType
{
    /// <summary>
    /// User created a new route
    /// </summary>
    RouteCreated = 1,
    
    /// <summary>
    /// User updated a route
    /// </summary>
    RouteUpdated = 2,
    
    /// <summary>
    /// User favorited a route
    /// </summary>
    RouteFavorited = 3,
    
    /// <summary>
    /// User created a review
    /// </summary>
    ReviewCreated = 4,
    
    /// <summary>
    /// User created a collection
    /// </summary>
    CollectionCreated = 5,
    
    /// <summary>
    /// User added a route to a collection
    /// </summary>
    RouteAddedToCollection = 6,
    
    /// <summary>
    /// User started following another user
    /// </summary>
    UserFollowed = 7
}
