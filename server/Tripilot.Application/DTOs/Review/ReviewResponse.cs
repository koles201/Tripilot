namespace Tripilot.Application.DTOs.Review;

/// <summary>
/// Response DTO for review details
/// </summary>
public class ReviewResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    public decimal OverallRating { get; set; }
    public decimal? CleanlinessRating { get; set; }
    public decimal? ServiceRating { get; set; }
    public decimal? ValueRating { get; set; }
    public decimal? LocationRating { get; set; }
    
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
    
    public Guid? PlaceId { get; set; }
    public string? PlaceName { get; set; }
    
    public Guid? RouteId { get; set; }
    public string? RouteName { get; set; }
    
    public int HelpfulCount { get; set; }
    public int UnhelpfulCount { get; set; }
    public bool IsVerified { get; set; }
    public bool IsFlagged { get; set; }
    
    public string? OwnerResponse { get; set; }
    public DateTime? OwnerResponseDate { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
