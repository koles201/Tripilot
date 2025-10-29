import type { FC } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Card,
  CardMedia,
  CardContent,
  CardActions,
  Typography,
  Button,
  Chip,
  Box,
  Rating,
} from '@mui/material';
import { Verified, LocationOn } from '@mui/icons-material';
import type { PlaceListItem } from '../../types/place';
import { PlaceCategory, PlaceCategoryNames } from '../../types/enums';

interface PlaceCardProps {
  place: PlaceListItem;
}

const PlaceCard: FC<PlaceCardProps> = ({ place }) => {
  const navigate = useNavigate();

  const handleViewDetails = () => {
    navigate(`/places/${place.id}`);
  };

  const getCategoryColor = (category: number): 'primary' | 'secondary' | 'success' | 'warning' | 'info' => {
    switch (category) {
      case PlaceCategory.Restaurant:
      case PlaceCategory.Cafe:
        return 'warning';
      case PlaceCategory.Museum:
      case PlaceCategory.HistoricalSite:
        return 'info';
      case PlaceCategory.Entertainment:
        return 'secondary';
      case PlaceCategory.Hotel:
        return 'success';
      default:
        return 'primary';
    }
  };

  const getPriceLevelText = (priceLevel?: number): string => {
    if (!priceLevel) return '';
    return '$'.repeat(priceLevel);
  };

  const location = [place.city, place.country].filter(Boolean).join(', ');

  return (
    <Card
      sx={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        transition: 'transform 0.2s, box-shadow 0.2s',
        '&:hover': {
          transform: 'translateY(-4px)',
          boxShadow: 6,
        },
      }}
    >
      <CardMedia
        component="img"
        height="200"
        image={place.imageUrl || '/placeholder-image.jpg'}
        alt={place.name}
        sx={{ objectFit: 'cover' }}
      />
      
      <CardContent sx={{ flexGrow: 1, pb: 1 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
          <Typography variant="h6" component="h2" sx={{ flexGrow: 1 }} noWrap>
            {place.name}
          </Typography>
          {place.isVerified && (
            <Verified color="primary" fontSize="small" titleAccess="Verified" />
          )}
        </Box>

        <Box sx={{ display: 'flex', gap: 1, mb: 1, alignItems: 'center' }}>
          <Chip
            label={PlaceCategoryNames[place.category as keyof typeof PlaceCategoryNames]}
            size="small"
            color={getCategoryColor(place.category)}
          />
          {place.priceLevel && (
            <Typography variant="body2" color="text.secondary" fontWeight="bold">
              {getPriceLevelText(place.priceLevel)}
            </Typography>
          )}
        </Box>

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mb: 1 }}>
          <Rating value={place.averageRating} precision={0.5} size="small" readOnly />
          <Typography variant="body2" color="text.secondary">
            {place.averageRating.toFixed(1)} ({place.reviewCount})
          </Typography>
        </Box>

        {location && (
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mb: 1 }}>
            <LocationOn fontSize="small" color="action" />
            <Typography variant="body2" color="text.secondary" noWrap>
              {location}
            </Typography>
          </Box>
        )}

        <Typography
          variant="body2"
          color="text.secondary"
          sx={{
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            display: '-webkit-box',
            WebkitLineClamp: 2,
            WebkitBoxOrient: 'vertical',
          }}
        >
          {place.description}
        </Typography>
      </CardContent>

      <CardActions sx={{ px: 2, pb: 2 }}>
        <Button
          size="small"
          variant="contained"
          onClick={handleViewDetails}
          fullWidth
        >
          View Details
        </Button>
      </CardActions>
    </Card>
  );
};

export default PlaceCard;
