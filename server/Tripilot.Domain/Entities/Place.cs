using Tripilot.Domain.Enums;
using Tripilot.Domain.ValueObjects;

namespace Tripilot.Domain.Entities;

/// <summary>
/// Place entity representing tourist destinations
/// </summary>
public class Place : BaseEntity, IAuditableEntity
{
    /// <summary>
    /// Place name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Detailed description of the place
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Place category
    /// </summary>
    public PlaceCategory Category { get; set; }

    /// <summary>
    /// Location information
    /// </summary>
    public Location Location { get; set; } = null!;

    /// <summary>
    /// Contact information
    /// </summary>
    public ContactInfo? ContactInfo { get; set; }

    /// <summary>
    /// Operating hours
    /// </summary>
    public OperatingHours? OperatingHours { get; set; }

    /// <summary>
    /// Average rating (1-5)
    /// </summary>
    public decimal AverageRating { get; set; }

    /// <summary>
    /// Total number of reviews
    /// </summary>
    public int ReviewCount { get; set; }

    /// <summary>
    /// Price level (1-4: $, UTF8, UTF8$, UTF8UTF8)
    /// </summary>
    public int? PriceLevel { get; set; }

    /// <summary>
    /// List of amenities (JSON array: ["wifi", "parking", "wheelchair_accessible"])
    /// </summary>
    public string? Amenities { get; set; }

    /// <summary>
    /// Main image URL
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Gallery images (JSON array of URLs)
    /// </summary>
    public string? GalleryImages { get; set; }

    /// <summary>
    /// Whether this place is verified/claimed by a business owner
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// User ID of the business owner (if claimed)
    /// </summary>
    public Guid? OwnerId { get; set; }

    /// <summary>
    /// Business owner navigation property
    /// </summary>
    public User? Owner { get; set; }

    /// <summary>
    /// Whether the place is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// View count for analytics
    /// </summary>
    public int ViewCount { get; set; }

    // IAuditableEntity implementation
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
