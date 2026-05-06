import { useEffect, useState } from 'react';
import {
  Container,
  Box,
  Typography,
  CircularProgress,
  Alert,
  Pagination,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Button,
  Chip,
} from '@mui/material';
import { Favorite as FavoriteIcon, FilterList } from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import {
  fetchFavorites,
  selectFavorites,
  selectFavoriteLoading,
  selectFavoriteError,
  selectFavoritePagination,
} from '../../store/slices/favoriteSlice';
import { selectIsAuthenticated } from '../../store/slices/authSlice';
import PlaceCard from '../../components/places/PlaceCard';
import { useNavigate } from 'react-router-dom';
import type { FavoriteQueryParams } from '../../types/favorite';

// Categories for filtering
const CATEGORIES = [
  'Restaurant',
  'Hotel',
  'Museum',
  'Park',
  'Beach',
  'Shopping',
  'Entertainment',
  'NightLife',
  'Historical',
  'Religious',
  'Nature',
  'Adventure',
  'Cultural',
  'Educational',
];

/**
 * Favorites Page Component
 * 
 * Displays user's favorited places with filtering and sorting options.
 * 
 * @component
 */
const FavoritesPage = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const isAuthenticated = useAppSelector(selectIsAuthenticated);
  const favorites = useAppSelector(selectFavorites);
  const loading = useAppSelector(selectFavoriteLoading);
  const error = useAppSelector(selectFavoriteError);
  const pagination = useAppSelector(selectFavoritePagination);

  const [queryParams, setQueryParams] = useState<FavoriteQueryParams>({
    page: 1,
    pageSize: 20,
    sortBy: 'createdAt',
    sortDirection: 'desc',
  });

  // Redirect to login if not authenticated
  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login', { state: { from: '/favorites' } });
    }
  }, [isAuthenticated, navigate]);

  // Load favorites on mount and when query params change
  useEffect(() => {
    if (isAuthenticated) {
      dispatch(fetchFavorites(queryParams));
    }
  }, [dispatch, isAuthenticated, queryParams]);

  /**
   * Handle page change
   */
  const handlePageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
    setQueryParams({ ...queryParams, page: value });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  /**
   * Handle category filter change
   */
  const handleCategoryChange = (event: any) => {
    setQueryParams({
      ...queryParams,
      category: event.target.value || undefined,
      page: 1,
    });
  };

  /**
   * Handle sort change
   */
  const handleSortChange = (event: any) => {
    const [sortBy, sortDirection] = event.target.value.split('-') as [
      'createdAt' | 'name' | 'rating',
      'asc' | 'desc'
    ];
    setQueryParams({
      ...queryParams,
      sortBy,
      sortDirection,
      page: 1,
    });
  };

  /**
   * Clear category filter
   */
  const handleClearCategory = () => {
    setQueryParams({
      ...queryParams,
      category: undefined,
      page: 1,
    });
  };

  if (!isAuthenticated) {
    return null; // Will redirect
  }

  return (
    <Container maxWidth="xl" sx={{ py: 4 }}>
      {/* Page Header */}
      <Box sx={{ mb: 4 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
          <FavoriteIcon color="error" sx={{ fontSize: 32 }} />
          <Typography variant="h4" component="h1" fontWeight="bold">
            My Favorites
          </Typography>
        </Box>
        <Typography variant="body1" color="text.secondary">
          Places you've saved for your next adventure
        </Typography>
      </Box>

      {/* Filters and Sort Bar */}
      <Box
        sx={{
          mb: 3,
          display: 'flex',
          gap: 2,
          alignItems: 'center',
          flexWrap: 'wrap',
        }}
      >
        {/* Category Filter */}
        <FormControl sx={{ minWidth: 200 }}>
          <InputLabel id="category-filter-label">Category</InputLabel>
          <Select
            labelId="category-filter-label"
            value={queryParams.category || ''}
            onChange={handleCategoryChange}
            label="Category"
          >
            <MenuItem value="">
              <em>All Categories</em>
            </MenuItem>
            {CATEGORIES.map((cat) => (
              <MenuItem key={cat} value={cat}>
                {cat}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        {/* Sort Dropdown */}
        <FormControl sx={{ minWidth: 200 }}>
          <InputLabel id="sort-select-label">Sort By</InputLabel>
          <Select
            labelId="sort-select-label"
            value={`${queryParams.sortBy}-${queryParams.sortDirection}`}
            onChange={handleSortChange}
            label="Sort By"
          >
            <MenuItem value="createdAt-desc">Recently Added</MenuItem>
            <MenuItem value="createdAt-asc">Oldest First</MenuItem>
            <MenuItem value="name-asc">Name (A-Z)</MenuItem>
            <MenuItem value="name-desc">Name (Z-A)</MenuItem>
            <MenuItem value="rating-desc">Highest Rated</MenuItem>
            <MenuItem value="rating-asc">Lowest Rated</MenuItem>
          </Select>
        </FormControl>

        {/* Active Filter Badge */}
        {queryParams.category && (
          <Chip
            icon={<FilterList />}
            label={`Category: ${queryParams.category}`}
            onDelete={handleClearCategory}
            color="primary"
          />
        )}

        {/* Results Count */}
        <Box sx={{ ml: 'auto' }}>
          <Typography variant="body2" color="text.secondary">
            {pagination.totalCount} {pagination.totalCount === 1 ? 'favorite' : 'favorites'}
          </Typography>
        </Box>
      </Box>

      {/* Main Content */}
      <Box>
        {/* Loading State */}
        {loading && (
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              justifyContent: 'center',
              py: 8,
              gap: 2,
            }}
          >
            <CircularProgress size={48} />
            <Typography variant="body2" color="text.secondary">
              Loading your favorites...
            </Typography>
          </Box>
        )}

        {/* Error State */}
        {error && (
          <Alert severity="error" sx={{ mb: 3 }}>
            {error}
          </Alert>
        )}

        {/* Empty State */}
        {!loading && !error && favorites.length === 0 && (
          <Box
            sx={{
              textAlign: 'center',
              py: 8,
              px: 2,
            }}
          >
            <FavoriteIcon sx={{ fontSize: 80, color: 'text.disabled', mb: 2 }} />
            <Typography variant="h6" color="text.secondary" gutterBottom>
              No favorites yet
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
              Start exploring and save places you'd like to visit!
            </Typography>
            <Button
              variant="contained"
              onClick={() => navigate('/places')}
              size="large"
            >
              Explore Places
            </Button>
          </Box>
        )}

        {/* Favorites Grid */}
        {!loading && !error && favorites.length > 0 && (
          <>
            <Box
              sx={{
                display: 'grid',
                gridTemplateColumns: {
                  xs: '1fr',
                  sm: 'repeat(2, 1fr)',
                  lg: 'repeat(3, 1fr)',
                  xl: 'repeat(4, 1fr)',
                },
                gap: 3,
              }}
            >
              {favorites.map((favorite) => (
                <PlaceCard
                  key={favorite.id}
                  place={{
                    id: favorite.placeId,
                    name: favorite.placeName,
                    category: favorite.category as any,
                    city: favorite.city || '',
                    country: favorite.country || '',
                    imageUrl: favorite.imageUrl || '',
                    averageRating: favorite.averageRating || 0,
                    reviewCount: 0,
                    description: '',
                    latitude: 0,
                    longitude: 0,
                    isVerified: false,
                    viewCount: 0,
                  }}
                />
              ))}
            </Box>

            {/* Pagination */}
            {pagination.totalPages > 1 && (
              <Box
                sx={{
                  display: 'flex',
                  justifyContent: 'center',
                  alignItems: 'center',
                  mt: 4,
                  gap: 2,
                  flexDirection: { xs: 'column', sm: 'row' },
                }}
              >
                <Pagination
                  count={pagination.totalPages}
                  page={pagination.page}
                  onChange={handlePageChange}
                  color="primary"
                  size="large"
                  showFirstButton
                  showLastButton
                  siblingCount={1}
                  boundaryCount={1}
                />
                <Typography variant="caption" color="text.secondary">
                  Page {pagination.page} of {pagination.totalPages}
                </Typography>
              </Box>
            )}
          </>
        )}
      </Box>
    </Container>
  );
};

export default FavoritesPage;
