namespace Tripilot.Application.DTOs.RouteSharing;

/// <summary>
/// Request to generate a shareable link for a route
/// </summary>
public class GenerateShareLinkRequest
{
    /// <summary>
    /// ID of the route to share
    /// </summary>
    public Guid RouteId { get; set; }
}

/// <summary>
/// Response containing the shareable link
/// </summary>
public class ShareLinkResponse
{
    /// <summary>
    /// Unique share token
    /// </summary>
    public string ShareToken { get; set; } = string.Empty;
    
    /// <summary>
    /// Full shareable URL
    /// </summary>
    public string ShareUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Embed code for the route
    /// </summary>
    public string? EmbedCode { get; set; }
}

/// <summary>
/// Request to get embed code for a route
/// </summary>
public class GetEmbedCodeRequest
{
    /// <summary>
    /// Share token or route ID
    /// </summary>
    public string Identifier { get; set; } = string.Empty;
    
    /// <summary>
    /// Width of the embed widget
    /// </summary>
    public int Width { get; set; } = 600;
    
    /// <summary>
    /// Height of the embed widget
    /// </summary>
    public int Height { get; set; } = 400;
}

/// <summary>
/// Response containing social media metadata for a route
/// </summary>
public class RouteSocialMetadata
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string Url { get; set; } = string.Empty;
    public string CreatorName { get; set; } = string.Empty;
    public decimal AverageRating { get; set; }
    public int ViewCount { get; set; }
    public int FavoriteCount { get; set; }
}
