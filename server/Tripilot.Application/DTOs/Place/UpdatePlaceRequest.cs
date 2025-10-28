using System.ComponentModel.DataAnnotations;

namespace Tripilot.Application.DTOs.Place;

/// <summary>
/// Request DTO for updating an existing place
/// </summary>
public class UpdatePlaceRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    // Location
    [Required]
    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Required]
    [Range(-180, 180)]
    public double Longitude { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    // Contact Info
    [StringLength(50)]
    public string? Phone { get; set; }

    [EmailAddress]
    [StringLength(256)]
    public string? Email { get; set; }

    [Url]
    [StringLength(500)]
    public string? Website { get; set; }

    // Operating Hours
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    
    [StringLength(100)]
    public string? DaysOfWeek { get; set; }
    
    public bool Is24Hours { get; set; }
    
    [StringLength(500)]
    public string? SpecialNotes { get; set; }

    // Additional Info
    [Range(1, 4)]
    public int? PriceLevel { get; set; }

    [StringLength(1000)]
    public string? Amenities { get; set; }

    [Url]
    [StringLength(1000)]
    public string? ImageUrl { get; set; }

    [StringLength(4000)]
    public string? GalleryImages { get; set; }

    public bool IsActive { get; set; } = true;
}
