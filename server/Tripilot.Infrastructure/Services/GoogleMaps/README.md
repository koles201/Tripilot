# Google Maps API Integration

## Overview
This folder contains service implementations for Google Maps Platform APIs using direct HTTP REST API calls.

## Services Implemented

### 1. GeocodingService 
- Convert address to coordinates
- Reverse geocoding (coordinates to address)
- Multiple geocoding results

### 2. PlacesService (TO IMPLEMENT)
- Place details lookup
- Place search
- Place photos

### 3. DirectionsService (TO IMPLEMENT)
- Turn-by-turn directions
- Directions with waypoints
- Route optimization

### 4. DistanceMatrixService (TO IMPLEMENT)
- Distance/duration between multiple origins and destinations
- Single point-to-point distance calculation

## API Keys Required
Configure in `appsettings.json`:
```json
"GoogleMaps": {
  "ApiKey": "YOUR_GOOGLE_MAPS_API_KEY",
  "Enabled": true
}
```

## APIs Required
Enable these APIs in Google Cloud Console:
1. Geocoding API
2. Places API (New)
3. Directions API
4. Distance Matrix API
5. Maps JavaScript API (for frontend)

## Implementation Status
-  Geocoding Service: Complete with HTTP REST API
-  Places Service: Needs HTTP REST API implementation
-  Directions Service: Needs HTTP REST API implementation
-  Distance Matrix Service: Needs HTTP REST API implementation

## Next Steps
1. Implement remaining services using HttpClient
2. Add comprehensive error handling
3. Add response caching
4. Add rate limiting
5. Add retry policies with Polly
