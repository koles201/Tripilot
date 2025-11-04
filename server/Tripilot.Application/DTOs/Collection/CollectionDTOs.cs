namespace Tripilot.Application.DTOs.Collection;

/// <summary>
/// Request to create a new route collection
/// </summary>
public class CreateCollectionRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? CoverImageUrl { get; set; }
}

/// <summary>
/// Request to update a route collection
/// </summary>
public class UpdateCollectionRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public string? CoverImageUrl { get; set; }
}

/// <summary>
/// Request to add a route to a collection
/// </summary>
public class AddRouteToCollectionRequest
{
    public Guid CollectionId { get; set; }
    public Guid RouteId { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// Response for a route collection
/// </summary>
public class CollectionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public string? CoverImageUrl { get; set; }
    public int ItemCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

/// <summary>
/// Response for a collection with its routes
/// </summary>
public class CollectionDetailResponse : CollectionResponse
{
    public List<CollectionItemResponse> Items { get; set; } = new();
}

/// <summary>
/// Response for a route within a collection
/// </summary>
public class CollectionItemResponse
{
    public Guid Id { get; set; }
    public Guid RouteId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public string? RouteImageUrl { get; set; }
    public int Order { get; set; }
    public string? Note { get; set; }
    public DateTime AddedAt { get; set; }
}
