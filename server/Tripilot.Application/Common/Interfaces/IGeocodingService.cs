using Tripilot.Application.DTOs.GoogleMaps;

namespace Tripilot.Application.Common.Interfaces;

/// <summary>
/// Interface for geocoding services (address to coordinates)
/// </summary>
public interface IGeocodingService
{
    /// <summary>
    /// Convert an address to geographic coordinates
    /// </summary>
    Task<GeocodingResult?> GeocodeAddressAsync(string address, string? language = null, string? region = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Convert geographic coordinates to an address (reverse geocoding)
    /// </summary>
    Task<GeocodingResult?> ReverseGeocodeAsync(double latitude, double longitude, string? language = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get multiple geocoding results for an address
    /// </summary>
    Task<List<GeocodingResult>> GeocodeAddressMultipleAsync(string address, int maxResults = 5, string? language = null, string? region = null, CancellationToken cancellationToken = default);
}
