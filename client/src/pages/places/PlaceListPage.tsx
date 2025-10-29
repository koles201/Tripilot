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
  Drawer,
  IconButton,
  useMediaQuery,
  useTheme,
} from '@mui/material';
import { FilterList } from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import {
  fetchPlaces,
  searchPlaces,
  selectPlaces,
  selectPlaceLoading,
  selectPlaceError,
  selectPagination,
  selectPlaceFilters,
  setFilters,
  setPage,
} from '../../store/slices/placeSlice';
import PlaceCard from '../../components/places/PlaceCard';
import PlaceSearch from '../../components/places/PlaceSearch';
import PlaceFilters from '../../components/places/PlaceFilters';
import type { PlaceQueryParams } from '../../types/place';

const PlaceListPage = () => {
  const dispatch = useAppDispatch();
  const places = useAppSelector(selectPlaces);
  const loading = useAppSelector(selectPlaceLoading);
  const error = useAppSelector(selectPlaceError);
  const pagination = useAppSelector(selectPagination);
  const filters = useAppSelector(selectPlaceFilters);

  const [searchTerm, setSearchTerm] = useState(filters.searchTerm || '');
  const [sortBy, setSortBy] = useState<'rating' | 'distance' | 'name' | 'newest'>('rating');
  const [mobileFiltersOpen, setMobileFiltersOpen] = useState(false);

  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));

  useEffect(() => {
    const params: PlaceQueryParams = {
      ...filters,
      page: pagination.page,
      pageSize: pagination.pageSize,
      sortBy,
    };

    if (searchTerm) {
      dispatch(searchPlaces({ ...params, searchTerm }));
    } else {
      dispatch(fetchPlaces(params));
    }
  }, [dispatch, filters, pagination.page, pagination.pageSize, sortBy, searchTerm]);

  const handleSearchChange = (value: string) => {
    setSearchTerm(value);
    dispatch(setPage(1));
  };

  const handleFiltersChange = (newFilters: PlaceQueryParams) => {
    dispatch(setFilters(newFilters));
    dispatch(setPage(1));
  };

  const handlePageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
    dispatch(setPage(value));
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleSortChange = (event: any) => {
    setSortBy(event.target.value);
    dispatch(setPage(1));
  };

  return (
    <Container maxWidth="xl" sx={{ py: 4 }}>
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom fontWeight="bold">
          Discover Places
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Explore amazing destinations and experiences
        </Typography>
      </Box>

      {/* Search and Sort Bar */}
      <Box sx={{ mb: 3, display: 'flex', gap: 2, alignItems: 'center' }}>
        <Box sx={{ flexGrow: 1 }}>
          <PlaceSearch value={searchTerm} onChange={handleSearchChange} />
        </Box>

        {isMobile && (
          <IconButton
            onClick={() => setMobileFiltersOpen(true)}
            sx={{ bgcolor: 'background.paper' }}
          >
            <FilterList />
          </IconButton>
        )}

        <FormControl sx={{ minWidth: 150 }}>
          <InputLabel>Sort By</InputLabel>
          <Select value={sortBy} onChange={handleSortChange} label="Sort By" size="medium">
            <MenuItem value="rating">Rating</MenuItem>
            <MenuItem value="distance">Distance</MenuItem>
            <MenuItem value="name">Name</MenuItem>
            <MenuItem value="newest">Newest</MenuItem>
          </Select>
        </FormControl>
      </Box>

      {/* Main Content */}
      <Box sx={{ display: 'flex', gap: 3 }}>
        {/* Filters Sidebar - Desktop */}
        {!isMobile && (
          <Box sx={{ width: 280, flexShrink: 0 }}>
            <PlaceFilters filters={filters} onChange={handleFiltersChange} />
          </Box>
        )}

        {/* Filters Drawer - Mobile */}
        {isMobile && (
          <Drawer
            anchor="right"
            open={mobileFiltersOpen}
            onClose={() => setMobileFiltersOpen(false)}
          >
            <Box sx={{ width: 300, p: 2 }}>
              <Typography variant="h6" gutterBottom>
                Filters
              </Typography>
              <PlaceFilters filters={filters} onChange={handleFiltersChange} />
            </Box>
          </Drawer>
        )}

        {/* Places Grid */}
        <Box sx={{ flexGrow: 1 }}>
          {loading && (
            <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
              <CircularProgress />
            </Box>
          )}

          {error && (
            <Alert severity="error" sx={{ mb: 3 }}>
              {error}
            </Alert>
          )}

          {!loading && !error && places.length === 0 && (
            <Box sx={{ textAlign: 'center', py: 8 }}>
              <Typography variant="h6" color="text.secondary">
                No places found
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Try adjusting your search or filters
              </Typography>
            </Box>
          )}

          {!loading && !error && places.length > 0 && (
            <>
              <Box
                sx={{
                  display: 'grid',
                  gridTemplateColumns: {
                    xs: '1fr',
                    sm: 'repeat(2, 1fr)',
                    lg: 'repeat(3, 1fr)',
                  },
                  gap: 3,
                }}
              >
                {places.map((place) => (
                  <PlaceCard key={place.id} place={place} />
                ))}
              </Box>

              {/* Pagination */}
              {pagination.totalPages > 1 && (
                <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
                  <Pagination
                    count={pagination.totalPages}
                    page={pagination.page}
                    onChange={handlePageChange}
                    color="primary"
                    size="large"
                    showFirstButton
                    showLastButton
                  />
                </Box>
              )}

              {/* Results Info */}
              <Box sx={{ mt: 2, textAlign: 'center' }}>
                <Typography variant="body2" color="text.secondary">
                  Showing {places.length} of {pagination.totalCount} places
                </Typography>
              </Box>
            </>
          )}
        </Box>
      </Box>
    </Container>
  );
};

export default PlaceListPage;
