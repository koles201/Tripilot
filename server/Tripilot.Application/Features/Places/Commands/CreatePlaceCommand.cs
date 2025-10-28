using MediatR;
using Tripilot.Application.DTOs.Place;

namespace Tripilot.Application.Features.Places.Commands;

/// <summary>
/// Command to create a new place
/// </summary>
public class CreatePlaceCommand : IRequest<PlaceResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    
    // Location
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    
    // Contact Info
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    
    // Operating Hours
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
    public string? DaysOfWeek { get; set; }
    public bool Is24Hours { get; set; }
    public string? SpecialNotes { get; set; }
    
    // Additional Info
    public int? PriceLevel { get; set; }
    public string? Amenities { get; set; }
    public string? ImageUrl { get; set; }
    public string? GalleryImages { get; set; }
    
    // User context (set by controller)
    public Guid UserId { get; set; }
    public string UserRole { get; set; } = string.Empty;
}
