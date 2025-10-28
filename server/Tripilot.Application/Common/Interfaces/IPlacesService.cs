using Tripilot.Application.DTOs.GoogleMaps;

namespace Tripilot.Application.Common.Interfaces;

/// <summary>
/// Interface for Google Places API services
/// </summary>
public interface IPlacesService
{
    /// <summary>
    /// Get detailed information about a place by Place ID
    /// </summary>
    Task<PlaceDetailsResult?> GetPlaceDetailsAsync(string placeId, string? language = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search for places by text query
    /// </summary>
    Task<List<PlaceSearchResult>> SearchPlacesAsync(string query, double? latitude = null, double? longitude = null, int radiusMeters = 5000, string? language = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get place photo URL by photo reference
    /// </summary>
    string GetPlacePhotoUrl(string photoReference, int maxWidth = 400, int maxHeight = 400);

    /// <summary>
    /// Get multiple photo URLs for a place
    /// </summary>
    Task<List<string>> GetPlacePhotosAsync(string placeId, int maxPhotos = 5, int photoWidth = 400, int photoHeight = 400, CancellationToken cancellationToken = default);
}
