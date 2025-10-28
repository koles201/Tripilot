using Tripilot.Application.DTOs.GoogleMaps;

namespace Tripilot.Application.Common.Interfaces;

/// <summary>
/// Interface for Google Directions API services
/// </summary>
public interface IDirectionsService
{
    /// <summary>
    /// Get directions between two points
    /// </summary>
    Task<DirectionsResult?> GetDirectionsAsync(
        double originLat, 
        double originLng, 
        double destinationLat, 
        double destinationLng, 
        TravelMode travelMode = TravelMode.Driving,
        string? language = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get directions with waypoints (for routes with multiple stops)
    /// </summary>
    Task<DirectionsResult?> GetDirectionsWithWaypointsAsync(
        double originLat,
        double originLng,
        double destinationLat,
        double destinationLng,
        List<(double lat, double lng)> waypoints,
        TravelMode travelMode = TravelMode.Walking,
        bool optimizeWaypoints = false,
        string? language = null,
        CancellationToken cancellationToken = default);
}
