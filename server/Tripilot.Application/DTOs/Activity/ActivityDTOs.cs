namespace Tripilot.Application.DTOs.Activity;

/// <summary>
/// Response for a user activity item
/// </summary>
public class ActivityResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string? EntityType { get; set; }
    public string? EntityName { get; set; }
    public string? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request to get activity feed
/// </summary>
public class GetActivityFeedRequest
{
    /// <summary>
    /// Only show activities from followed users
    /// </summary>
    public bool FollowedOnly { get; set; }
    
    /// <summary>
    /// Filter by activity type
    /// </summary>
    public string? ActivityType { get; set; }
    
    /// <summary>
    /// Page number
    /// </summary>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 20;
}
