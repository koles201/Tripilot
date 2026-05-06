namespace Tripilot.Application.DTOs.Social;

/// <summary>
/// Request to follow a user
/// </summary>
public class FollowUserRequest
{
    /// <summary>
    /// ID of the user to follow
    /// </summary>
    public Guid UserId { get; set; }
}

/// <summary>
/// Response for a user follow relationship
/// </summary>
public class UserFollowResponse
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// User profile with social information
/// </summary>
public class UserProfileResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
    public bool IsFollowing { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Response for a list of followers or following
/// </summary>
public class UserListItemResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsFollowing { get; set; }
    public DateTime FollowedAt { get; set; }
}
