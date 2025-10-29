import React, { useEffect, useState, useMemo } from 'react';
import { GoogleMap, LoadScript, Marker, Polyline, InfoWindow } from '@react-google-maps/api';
import { Box, Paper, Typography, CircularProgress, Alert, Chip } from '@mui/material';
import { Place as PlaceIcon, Room as RoomIcon } from '@mui/icons-material';
import type { PlaceListItem } from '../../types/place';

interface RouteMapProps {
  places: PlaceListItem[];
  height?: string | number;
  showRoute?: boolean;
  interactive?: boolean;
  center?: { lat: number; lng: number };
  zoom?: number;
}

const containerStyle = {
  width: '100%',
  height: '100%',
};

const defaultCenter = {
  lat: 40.7128, // New York
  lng: -74.006,
};

/**
 * Route map component with Google Maps
 */
export const RouteMap: React.FC<RouteMapProps> = ({
  places,
  height = '500px',
  showRoute = true,
  interactive = true,
  center,
  zoom = 12,
}) => {
  const [map, setMap] = useState<google.maps.Map | null>(null);
  const [selectedPlace, setSelectedPlace] = useState<PlaceListItem | null>(null);
  const [totalDistance, setTotalDistance] = useState<number>(0);
  const [totalDuration, setTotalDuration] = useState<number>(0);
  const [isCalculating, setIsCalculating] = useState(false);

  const apiKey = import.meta.env.VITE_GOOGLE_MAPS_API_KEY;

  // Calculate map center from places
  const mapCenter = useMemo(() => {
    if (center) return center;
    if (places.length === 0) return defaultCenter;

    const validPlaces = places.filter(p => p.latitude && p.longitude);
    if (validPlaces.length === 0) return defaultCenter;

    const avgLat = validPlaces.reduce((sum, p) => sum + p.latitude!, 0) / validPlaces.length;
    const avgLng = validPlaces.reduce((sum, p) => sum + p.longitude!, 0) / validPlaces.length;

    return { lat: avgLat, lng: avgLng };
  }, [places, center]);

  // Create path for polyline
  const path = useMemo(() => {
    return places
      .filter(p => p.latitude && p.longitude)
      .map(p => ({
        lat: p.latitude!,
        lng: p.longitude!,
      }));
  }, [places]);

  // Calculate distance and duration
  useEffect(() => {
    if (!showRoute || places.length < 2 || !window.google) return;

    const calculateRoute = async () => {
      setIsCalculating(true);
      try {
        const service = new google.maps.DistanceMatrixService();
        let totalDist = 0;
        let totalDur = 0;

        // Calculate distance between consecutive places
        for (let i = 0; i < places.length - 1; i++) {
          const origin = places[i];
          const destination = places[i + 1];

          if (!origin.latitude || !origin.longitude || !destination.latitude || !destination.longitude) {
            continue;
          }

          const response = await service.getDistanceMatrix({
            origins: [{ lat: origin.latitude, lng: origin.longitude }],
            destinations: [{ lat: destination.latitude, lng: destination.longitude }],
            travelMode: google.maps.TravelMode.DRIVING,
            unitSystem: google.maps.UnitSystem.METRIC,
          });

          if (response.rows[0]?.elements[0]?.status === 'OK') {
            const element = response.rows[0].elements[0];
            totalDist += element.distance?.value || 0;
            totalDur += element.duration?.value || 0;
          }
        }

        setTotalDistance(totalDist / 1000); // Convert to km
        setTotalDuration(Math.round(totalDur / 60)); // Convert to minutes
      } catch (error) {
        console.error('Error calculating route:', error);
      } finally {
        setIsCalculating(false);
      }
    };

    calculateRoute();
  }, [places, showRoute]);

  // Fit bounds to show all markers
  useEffect(() => {
    if (!map || places.length === 0) return;

    const bounds = new google.maps.LatLngBounds();
    places.forEach(place => {
      if (place.latitude && place.longitude) {
        bounds.extend({ lat: place.latitude, lng: place.longitude });
      }
    });

    map.fitBounds(bounds);
  }, [map, places]);

  const onLoad = React.useCallback((map: google.maps.Map) => {
    setMap(map);
  }, []);

  const onUnmount = React.useCallback(() => {
    setMap(null);
  }, []);

  if (!apiKey) {
    return (
      <Alert severity="error">
        Google Maps API key is not configured. Please set VITE_GOOGLE_MAPS_API_KEY in your
        environment variables.
      </Alert>
    );
  }

  if (places.length === 0) {
    return (
      <Paper
        sx={{
          height,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          bgcolor: 'grey.100',
        }}
      >
        <Box sx={{ textAlign: 'center', color: 'text.secondary' }}>
          <RoomIcon sx={{ fontSize: 60, mb: 2, opacity: 0.3 }} />
          <Typography variant="body1">No places to display on map</Typography>
        </Box>
      </Paper>
    );
  }

  return (
    <Box sx={{ height, position: 'relative' }}>
      <LoadScript googleMapsApiKey={apiKey}>
        <GoogleMap
          mapContainerStyle={{ ...containerStyle, height }}
          center={mapCenter}
          zoom={zoom}
          onLoad={onLoad}
          onUnmount={onUnmount}
          options={{
            disableDefaultUI: !interactive,
            zoomControl: interactive,
            mapTypeControl: false,
            streetViewControl: false,
            fullscreenControl: interactive,
          }}
        >
          {/* Markers for each place */}
          {places.map((place, index) => {
            if (!place.latitude || !place.longitude) return null;

            return (
              <Marker
                key={place.id}
                position={{ lat: place.latitude, lng: place.longitude }}
                label={{
                  text: `${index + 1}`,
                  color: 'white',
                  fontWeight: 'bold',
                }}
                onClick={() => setSelectedPlace(place)}
              />
            );
          })}

          {/* Polyline connecting places */}
          {showRoute && path.length > 1 && (
            <Polyline
              path={path}
              options={{
                strokeColor: '#1976d2',
                strokeOpacity: 0.8,
                strokeWeight: 4,
                geodesic: true,
              }}
            />
          )}

          {/* Info window for selected place */}
          {selectedPlace && selectedPlace.latitude && selectedPlace.longitude && (
            <InfoWindow
              position={{ lat: selectedPlace.latitude, lng: selectedPlace.longitude }}
              onCloseClick={() => setSelectedPlace(null)}
            >
              <Box sx={{ p: 1, maxWidth: 250 }}>
                <Typography variant="subtitle2" fontWeight="bold" gutterBottom>
                  {selectedPlace.name}
                </Typography>
                {selectedPlace.address && (
                  <Typography variant="caption" color="text.secondary" display="block" gutterBottom>
                    {selectedPlace.address}
                  </Typography>
                )}
                {selectedPlace.category && (
                  <Chip
                    label={selectedPlace.category}
                    size="small"
                    icon={<PlaceIcon />}
                    sx={{ mt: 0.5 }}
                  />
                )}
              </Box>
            </InfoWindow>
          )}
        </GoogleMap>
      </LoadScript>

      {/* Route statistics */}
      {showRoute && places.length > 1 && (
        <Paper
          sx={{
            position: 'absolute',
            top: 16,
            right: 16,
            p: 2,
            minWidth: 200,
            zIndex: 1,
          }}
        >
          <Typography variant="subtitle2" fontWeight="bold" gutterBottom>
            Route Statistics
          </Typography>
          {isCalculating ? (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <CircularProgress size={16} />
              <Typography variant="caption">Calculating...</Typography>
            </Box>
          ) : (
            <>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 0.5 }}>
                <Typography variant="body2" color="text.secondary">
                  Total Distance:
                </Typography>
                <Typography variant="body2" fontWeight={500}>
                  {totalDistance.toFixed(1)} km
                </Typography>
              </Box>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 0.5 }}>
                <Typography variant="body2" color="text.secondary">
                  Travel Time:
                </Typography>
                <Typography variant="body2" fontWeight={500}>
                  {totalDuration} min
                </Typography>
              </Box>
              <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                <Typography variant="body2" color="text.secondary">
                  Places:
                </Typography>
                <Typography variant="body2" fontWeight={500}>
                  {places.length}
                </Typography>
              </Box>
            </>
          )}
        </Paper>
      )}
    </Box>
  );
};

export default RouteMap;
