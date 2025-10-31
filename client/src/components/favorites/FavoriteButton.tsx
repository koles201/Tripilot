import React, { useState } from 'react';
import { IconButton, Tooltip, CircularProgress } from '@mui/material';
import { Favorite, FavoriteBorder } from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import {
  addFavorite,
  removeFavorite,
  optimisticAddFavorite,
  optimisticRemoveFavorite,
  selectIsFavorite,
} from '../../store/slices/favoriteSlice';
import { selectIsAuthenticated } from '../../store/slices/authSlice';
import { useNavigate } from 'react-router-dom';

interface FavoriteButtonProps {
  /** ID of the place */
  placeId: string;
  /** Button size */
  size?: 'small' | 'medium' | 'large';
  /** Show tooltip */
  showTooltip?: boolean;
  /** Custom styling */
  sx?: any;
  /** Callback after successful toggle */
  onToggle?: (isFavorite: boolean) => void;
}

/**
 * FavoriteButton Component
 * 
 * Reusable button for adding/removing places from favorites.
 * Features optimistic updates for instant UI feedback.
 * 
 * @component
 * @example
 * ```tsx
 * <FavoriteButton
 *   placeId="place-123"
 *   size="medium"
 *   showTooltip
 *   onToggle={(isFavorite) => console.log('Favorited:', isFavorite)}
 * />
 * ```
 */
const FavoriteButton: React.FC<FavoriteButtonProps> = ({
  placeId,
  size = 'medium',
  showTooltip = true,
  sx,
  onToggle,
}) => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const isAuthenticated = useAppSelector(selectIsAuthenticated);
  const isFavorite = useAppSelector(selectIsFavorite(placeId));
  const [loading, setLoading] = useState(false);

  /**
   * Handle favorite toggle
   */
  const handleToggle = async (e: React.MouseEvent) => {
    // Prevent event bubbling (e.g., when button is inside a card)
    e.stopPropagation();
    e.preventDefault();

    // Redirect to login if not authenticated
    if (!isAuthenticated) {
      navigate('/login', { state: { from: window.location.pathname } });
      return;
    }

    setLoading(true);

    try {
      if (isFavorite) {
        // Optimistic update
        dispatch(optimisticRemoveFavorite({ placeId }));
        // Actual API call
        await dispatch(removeFavorite(placeId)).unwrap();
        onToggle?.(false);
      } else {
        // Optimistic update
        dispatch(optimisticAddFavorite({ placeId }));
        // Actual API call
        await dispatch(addFavorite(placeId)).unwrap();
        onToggle?.(true);
      }
    } catch (error) {
      console.error('Error toggling favorite:', error);
      // Optimistic update will be reverted by Redux on error
    } finally {
      setLoading(false);
    }
  };

  const button = (
    <IconButton
      onClick={handleToggle}
      disabled={loading}
      size={size}
      color={isFavorite ? 'error' : 'default'}
      aria-label={isFavorite ? 'Remove from favorites' : 'Add to favorites'}
      sx={{
        transition: 'all 0.2s ease-in-out',
        '&:hover': {
          transform: 'scale(1.1)',
        },
        ...sx,
      }}
    >
      {loading ? (
        <CircularProgress size={size === 'small' ? 16 : size === 'large' ? 28 : 20} />
      ) : isFavorite ? (
        <Favorite />
      ) : (
        <FavoriteBorder />
      )}
    </IconButton>
  );

  if (showTooltip) {
    return (
      <Tooltip
        title={
          !isAuthenticated
            ? 'Login to save favorites'
            : isFavorite
            ? 'Remove from favorites'
            : 'Add to favorites'
        }
        arrow
      >
        {button}
      </Tooltip>
    );
  }

  return button;
};

export default FavoriteButton;
