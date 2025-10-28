using Microsoft.AspNetCore.Mvc;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.DTOs.GoogleMaps;

namespace Tripilot.Api.Controllers;

/// <summary>
/// Controller for Google Maps integration services
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MapsController : ControllerBase
{
    private readonly IGeocodingService _geocodingService;
    private readonly IPlacesService _placesService;
    private readonly IDirectionsService _directionsService;
    private readonly IDistanceMatrixService _distanceMatrixService;
    private readonly ILogger<MapsController> _logger;

    public MapsController(
        IGeocodingService geocodingService,
        IPlacesService placesService,
        IDirectionsService directionsService,
        IDistanceMatrixService distanceMatrixService,
        ILogger<MapsController> logger)
    {
        _geocodingService = geocodingService;
        _placesService = placesService;
        _directionsService = directionsService;
        _distanceMatrixService = distanceMatrixService;
        _logger = logger;
    }

    /// <summary>
    /// Convert address to geographic coordinates (geocoding)
    /// </summary>
    [HttpGet("geocode")]
    [ProducesResponseType(typeof(GeocodingResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GeocodingResult>> GeocodeAddress(
        [FromQuery] string address,
        [FromQuery] string? language = null,
        [FromQuery] string? region = null)
    {
        var result = await _geocodingService.GeocodeAddressAsync(address, language, region);
        
        if (result == null)
        {
            return NotFound(new { message = "Address not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Convert coordinates to address (reverse geocoding)
    /// </summary>
    [HttpGet("reverse-geocode")]
    [ProducesResponseType(typeof(GeocodingResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GeocodingResult>> ReverseGeocode(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] string? language = null)
    {
        var result = await _geocodingService.ReverseGeocodeAsync(latitude, longitude, language);
        
        if (result == null)
        {
            return NotFound(new { message = "Address not found for coordinates" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get multiple geocoding results for an address
    /// </summary>
    [HttpGet("geocode/multiple")]
    [ProducesResponseType(typeof(List<GeocodingResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<GeocodingResult>>> GeocodeAddressMultiple(
        [FromQuery] string address,
        [FromQuery] int maxResults = 5,
        [FromQuery] string? language = null,
        [FromQuery] string? region = null)
    {
        var results = await _geocodingService.GeocodeAddressMultipleAsync(address, maxResults, language, region);
        return Ok(results);
    }

    /// <summary>
    /// Get detailed information about a place by Google Place ID
    /// </summary>
    [HttpGet("place/details")]
    [ProducesResponseType(typeof(PlaceDetailsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PlaceDetailsResult>> GetPlaceDetails(
        [FromQuery] string placeId,
        [FromQuery] string? language = null)
    {
        var result = await _placesService.GetPlaceDetailsAsync(placeId, language);
        
        if (result == null)
        {
            return NotFound(new { message = "Place not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Search for places by text query
    /// </summary>
    [HttpGet("place/search")]
    [ProducesResponseType(typeof(List<PlaceSearchResult>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PlaceSearchResult>>> SearchPlaces(
        [FromQuery] string query,
        [FromQuery] double? latitude = null,
        [FromQuery] double? longitude = null,
        [FromQuery] int radiusMeters = 5000,
        [FromQuery] string? language = null)
    {
        var results = await _placesService.SearchPlacesAsync(query, latitude, longitude, radiusMeters, language);
        return Ok(results);
    }

    /// <summary>
    /// Get photo URLs for a place
    /// </summary>
    [HttpGet("place/photos")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<string>>> GetPlacePhotos(
        [FromQuery] string placeId,
        [FromQuery] int maxPhotos = 5,
        [FromQuery] int photoWidth = 400,
        [FromQuery] int photoHeight = 400)
    {
        var photos = await _placesService.GetPlacePhotosAsync(placeId, maxPhotos, photoWidth, photoHeight);
        return Ok(photos);
    }

    /// <summary>
    /// Get directions between two points
    /// </summary>
    [HttpGet("directions")]
    [ProducesResponseType(typeof(DirectionsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DirectionsResult>> GetDirections(
        [FromQuery] double originLat,
        [FromQuery] double originLng,
        [FromQuery] double destinationLat,
        [FromQuery] double destinationLng,
        [FromQuery] TravelMode travelMode = TravelMode.Driving,
        [FromQuery] string? language = null)
    {
        var result = await _directionsService.GetDirectionsAsync(
            originLat, originLng, destinationLat, destinationLng, travelMode, language);
        
        if (result == null)
        {
            return NotFound(new { message = "Directions not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get directions with multiple waypoints
    /// </summary>
    [HttpPost("directions/waypoints")]
    [ProducesResponseType(typeof(DirectionsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DirectionsResult>> GetDirectionsWithWaypoints(
        [FromBody] DirectionsWithWaypointsRequest request)
    {
        var result = await _directionsService.GetDirectionsWithWaypointsAsync(
            request.OriginLat,
            request.OriginLng,
            request.DestinationLat,
            request.DestinationLng,
            request.Waypoints,
            request.TravelMode,
            request.OptimizeWaypoints,
            request.Language);
        
        if (result == null)
        {
            return NotFound(new { message = "Directions not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Calculate distance matrix between multiple origins and destinations
    /// </summary>
    [HttpPost("distance-matrix")]
    [ProducesResponseType(typeof(DistanceMatrixResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DistanceMatrixResult>> GetDistanceMatrix(
        [FromBody] DistanceMatrixRequest request)
    {
        var result = await _distanceMatrixService.GetDistanceMatrixAsync(
            request.Origins,
            request.Destinations,
            request.TravelMode,
            request.Language);
        
        if (result == null)
        {
            return NotFound(new { message = "Distance matrix calculation failed" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Calculate distance between two points
    /// </summary>
    [HttpGet("distance")]
    [ProducesResponseType(typeof(DistanceMatrixElement), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DistanceMatrixElement>> GetDistance(
        [FromQuery] double originLat,
        [FromQuery] double originLng,
        [FromQuery] double destinationLat,
        [FromQuery] double destinationLng,
        [FromQuery] TravelMode travelMode = TravelMode.Driving)
    {
        var result = await _distanceMatrixService.GetDistanceBetweenPointsAsync(
            originLat, originLng, destinationLat, destinationLng, travelMode);
        
        if (result == null)
        {
            return NotFound(new { message = "Distance calculation failed" });
        }

        return Ok(result);
    }
}

/// <summary>
/// Request model for directions with waypoints
/// </summary>
public class DirectionsWithWaypointsRequest
{
    public double OriginLat { get; set; }
    public double OriginLng { get; set; }
    public double DestinationLat { get; set; }
    public double DestinationLng { get; set; }
    public List<(double lat, double lng)> Waypoints { get; set; } = new();
    public TravelMode TravelMode { get; set; } = TravelMode.Walking;
    public bool OptimizeWaypoints { get; set; } = false;
    public string? Language { get; set; }
}

/// <summary>
/// Request model for distance matrix
/// </summary>
public class DistanceMatrixRequest
{
    public List<(double lat, double lng)> Origins { get; set; } = new();
    public List<(double lat, double lng)> Destinations { get; set; } = new();
    public TravelMode TravelMode { get; set; } = TravelMode.Driving;
    public string? Language { get; set; }
}
