import { useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Container,
  Box,
  Typography,
  CircularProgress,
  Alert,
  Button,
  Chip,
  Rating,
  Paper,
  Divider,
  Stack,
} from '@mui/material';
import {
  ArrowBack,
  LocationOn,
  Phone,
  Email,
  Language,
  AccessTime,
  Verified,
  AttachMoney,
} from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import {
  fetchPlaceById,
  selectCurrentPlace,
  selectPlaceLoading,
  selectPlaceError,
  clearCurrentPlace,
} from '../../store/slices/placeSlice';
import { PlaceCategoryNames } from '../../types/enums';

const PlaceDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const place = useAppSelector(selectCurrentPlace);
  const loading = useAppSelector(selectPlaceLoading);
  const error = useAppSelector(selectPlaceError);

  useEffect(() => {
    if (id) {
      dispatch(fetchPlaceById(id));
    }

    return () => {
      dispatch(clearCurrentPlace());
    };
  }, [id, dispatch]);

  if (loading) {
    return (
      <Container sx={{ py: 8, display: 'flex', justifyContent: 'center' }}>
        <CircularProgress />
      </Container>
    );
  }

  if (error) {
    return (
      <Container sx={{ py: 4 }}>
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
        <Button startIcon={<ArrowBack />} onClick={() => navigate('/places')}>
          Back to Places
        </Button>
      </Container>
    );
  }

  if (!place) {
    return (
      <Container sx={{ py: 4 }}>
        <Alert severity="info">Place not found</Alert>
        <Button startIcon={<ArrowBack />} onClick={() => navigate('/places')} sx={{ mt: 2 }}>
          Back to Places
        </Button>
      </Container>
    );
  }

  const location = [place.address, place.city, place.country].filter(Boolean).join(', ');
  const getPriceLevelText = (priceLevel?: number): string => {
    if (!priceLevel) return '';
    return '$'.repeat(priceLevel);
  };

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Button
        startIcon={<ArrowBack />}
        onClick={() => navigate('/places')}
        sx={{ mb: 3 }}
      >
        Back to Places
      </Button>

      {/* Header */}
      <Box sx={{ mb: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2 }}>
          <Typography variant="h3" component="h1" sx={{ flexGrow: 1 }}>
            {place.name}
          </Typography>
          {place.isVerified && (
            <Verified color="primary" fontSize="large" titleAccess="Verified" />
          )}
        </Box>

        <Stack direction="row" spacing={2} flexWrap="wrap" alignItems="center">
          <Chip
            label={PlaceCategoryNames[place.category as keyof typeof PlaceCategoryNames]}
            color="primary"
          />
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <Rating value={place.averageRating} precision={0.5} readOnly />
            <Typography variant="body1">
              {place.averageRating.toFixed(1)} ({place.reviewCount} reviews)
            </Typography>
          </Box>
          {place.priceLevel && (
            <Chip
              icon={<AttachMoney />}
              label={getPriceLevelText(place.priceLevel)}
              variant="outlined"
            />
          )}
        </Stack>
      </Box>

      {/* Image */}
      {place.imageUrl && (
        <Paper
          sx={{
            width: '100%',
            height: 400,
            mb: 4,
            overflow: 'hidden',
            borderRadius: 2,
          }}
        >
          <img
            src={place.imageUrl}
            alt={place.name}
            style={{
              width: '100%',
              height: '100%',
              objectFit: 'cover',
            }}
          />
        </Paper>
      )}

      {/* Description */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          About
        </Typography>
        <Typography variant="body1" color="text.secondary" paragraph>
          {place.description}
        </Typography>
      </Paper>

      {/* Contact Information */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          Contact Information
        </Typography>
        <Divider sx={{ mb: 2 }} />
        <Stack spacing={2}>
          {location && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
              <LocationOn color="action" />
              <Typography>{location}</Typography>
            </Box>
          )}
          {place.phone && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
              <Phone color="action" />
              <Typography>{place.phone}</Typography>
            </Box>
          )}
          {place.email && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
              <Email color="action" />
              <Typography>{place.email}</Typography>
            </Box>
          )}
          {place.website && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
              <Language color="action" />
              <Typography
                component="a"
                href={place.website}
                target="_blank"
                rel="noopener noreferrer"
                sx={{ color: 'primary.main', textDecoration: 'none' }}
              >
                {place.website}
              </Typography>
            </Box>
          )}
        </Stack>
      </Paper>

      {/* Operating Hours */}
      {(place.openTime || place.is24Hours) && (
        <Paper sx={{ p: 3, mb: 3 }}>
          <Typography variant="h5" gutterBottom>
            Opening Hours
          </Typography>
          <Divider sx={{ mb: 2 }} />
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
            <AccessTime color="action" />
            {place.is24Hours ? (
              <Typography>Open 24 hours</Typography>
            ) : (
              <Typography>
                {place.openTime} - {place.closeTime}
                {place.daysOfWeek && ` • ${place.daysOfWeek}`}
              </Typography>
            )}
          </Box>
          {place.specialNotes && (
            <Typography variant="body2" color="text.secondary" sx={{ mt: 2 }}>
              {place.specialNotes}
            </Typography>
          )}
        </Paper>
      )}

      {/* Amenities */}
      {place.amenities && (
        <Paper sx={{ p: 3, mb: 3 }}>
          <Typography variant="h5" gutterBottom>
            Amenities
          </Typography>
          <Divider sx={{ mb: 2 }} />
          <Typography variant="body1">{place.amenities}</Typography>
        </Paper>
      )}

      {/* Actions */}
      <Box sx={{ display: 'flex', gap: 2, justifyContent: 'center' }}>
        <Button variant="contained" size="large">
          Add to Route
        </Button>
        <Button variant="outlined" size="large">
          Write a Review
        </Button>
      </Box>
    </Container>
  );
};

export default PlaceDetailPage;
