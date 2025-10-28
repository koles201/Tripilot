using Tripilot.Application.DTOs.GoogleMaps;

namespace Tripilot.Application.Common.Interfaces;

/// <summary>
/// Interface for Google Distance Matrix API services
/// </summary>
public interface IDistanceMatrixService
{
    /// <summary>
    /// Calculate distances and travel times between multiple origins and destinations
    /// </summary>
    Task<DistanceMatrixResult?> GetDistanceMatrixAsync(
        List<(double lat, double lng)> origins,
        List<(double lat, double lng)> destinations,
        TravelMode travelMode = TravelMode.Driving,
        string? language = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculate distance and travel time between two points
    /// </summary>
    Task<DistanceMatrixElement?> GetDistanceBetweenPointsAsync(
        double originLat,
        double originLng,
        double destinationLat,
        double destinationLng,
        TravelMode travelMode = TravelMode.Driving,
        CancellationToken cancellationToken = default);
}
