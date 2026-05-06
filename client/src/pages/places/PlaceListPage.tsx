import { useEffect, useState, useCallback } from 'react';
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
import { FilterList, Close } from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../store/hooks';
import {
  fetchPlaces,
  selectPlaces,
  selectPlaceLoading,
  selectPlaceError,
  selectPagination,
} from '../../store/slices/placeSlice';
import PlaceCard from '../../components/places/PlaceCard';
import SearchBar from '../../components/search/SearchBar';
import AdvancedFilters from '../../components/search/AdvancedFilters';
import searchService from '../../services/api/searchService';
import type { SearchFilters, AdvancedSearchResult } from '../../types/search';

// Sort options for places
const SORT_OPTIONS = [
  { value: 'relevance', label: 'Relevance' },
  { value: 'rating', label: 'Rating' },
  { value: 'distance', label: 'Distance' },
  { value: 'popularity', label: 'Popularity' },
  { value: 'newest', label: 'Newest' },
  { value: 'name', label: 'Name' },
] as const;

const PlaceListPage = () => {
  // Redux state
  const dispatch = useAppDispatch();
  const places = useAppSelector(selectPlaces);
  const loading = useAppSelector(selectPlaceLoading);
  const error = useAppSelector(selectPlaceError);
  const pagination = useAppSelector(selectPagination);

  // Local state for advanced search
  const [searchResults, setSearchResults] = useState<AdvancedSearchResult[] | null>(null);
  const [searchLoading, setSearchLoading] = useState(false);
  const [searchError, setSearchError] = useState<string | null>(null);
  const [totalResults, setTotalResults] = useState(0);
  const [totalPages, setTotalPages] = useState(0);

  // Filter state
  const [filters, setFilters] = useState<SearchFilters>({
    page: 1,
    pageSize: 20,
    sortBy: 'relevance',
    sortDirection: 'desc',
  });
  const [filtersOpen, setFiltersOpen] = useState(false);

  const hasActiveFilters = useCallback((): boolean => {
    return !!(
      filters.category ||
      filters.city ||
      filters.country ||
      filters.minRating ||
      filters.maxRating ||
      filters.minPriceLevel ||
      filters.maxPriceLevel ||
      filters.isVerified ||
      filters.amenities?.length ||
      filters.isOpenNow ||
      filters.is24Hours ||
      filters.location
    );
  }, [filters]);

  const loadPlacesFromStore = useCallback(() => {
    dispatch(
      fetchPlaces({
        page: filters.page || 1,
        pageSize: filters.pageSize || 20,
      })
    );
    setSearchResults(null);
  }, [dispatch, filters.page, filters.pageSize]);

  const performAdvancedSearch = useCallback(async () => {
    try {
      setSearchLoading(true);
      setSearchError(null);
      const result = await searchService.advancedSearch(filters);
      setSearchResults(result.items);
      setTotalResults(result.totalCount);
      setTotalPages(result.totalPages);
    } catch (err: any) {
      console.error('Advanced search error:', err);
      setSearchError(err.response?.data?.message || 'Search failed. Please try again.');
    } finally {
      setSearchLoading(false);
    }
  }, [filters]);

  useEffect(() => {
    if (filters.searchTerm || hasActiveFilters()) {
      performAdvancedSearch();
    } else {
      loadPlacesFromStore();
    }
  }, [filters, hasActiveFilters, performAdvancedSearch, loadPlacesFromStore]);

  // ==================== Event Handlers ====================

  const handleSearch = (searchTerm: string) => {
    setFilters({
      ...filters,
      searchTerm: searchTerm || undefined,
      page: 1,
    });
  };

  const handleFiltersApply = (newFilters: SearchFilters) => {
    setFilters({
      ...newFilters,
      page: 1,
    });
  };

  const handlePageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
    setFilters({
      ...filters,
      page: value,
    });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleSortChange = (event: any) => {
    setFilters({
      ...filters,
      sortBy: event.target.value,
      page: 1,
    });
  };

  const handleClearFilters = () => {
    setFilters({
      page: 1,
      pageSize: 20,
      sortBy: 'relevance',
      sortDirection: 'desc',
    });
  };

  const removeFilter = (filterKey: keyof SearchFilters) => {
    setFilters({
      ...filters,
      [filterKey]: undefined,
      page: 1,
    });
  };

  // ==================== Computed Values ====================

  const displayPlaces = searchResults || places;
  const displayLoading = searchLoading || loading;
  const displayError = searchError || error;
  const displayPagination = searchResults
    ? { page: filters.page || 1, totalPages, totalCount: totalResults }
    : pagination;

  const activeFilterCount = [
    filters.category,
    filters.city,
    filters.country,
    filters.minRating && filters.minRating > 0,
    filters.maxRating && filters.maxRating < 5,
    filters.minPriceLevel && filters.minPriceLevel > 1,
    filters.maxPriceLevel && filters.maxPriceLevel < 4,
    filters.isVerified,
    filters.amenities?.length,
    filters.isOpenNow,
    filters.is24Hours,
  ].filter(Boolean).length;

  const isUsingAdvancedSearch = !!(filters.searchTerm || hasActiveFilters());

  // ==================== Render ====================

  return (
    <Container maxWidth="xl" sx={{ py: 4 }}>
      {/* Page Header */}
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom fontWeight="bold">
          Discover Places
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Explore amazing destinations and experiences around the world
        </Typography>
      </Box>

      {/* Search and Filters Toolbar */}
      <Box
        sx={{
          mb: 3,
          display: 'flex',
          gap: 2,
          alignItems: 'center',
          flexWrap: 'wrap',
        }}
      >
        {/* Search Bar */}
        <Box sx={{ flexGrow: 1, minWidth: { xs: '100%', sm: 300 } }}>
          <SearchBar onSearch={handleSearch} autoFocus={false} />
        </Box>

        {/* Filters Button */}
        <Button
          variant="outlined"
          startIcon={<FilterList />}
          onClick={() => setFiltersOpen(true)}
          sx={{ minWidth: 120 }}
        >
          Filters
          {activeFilterCount > 0 && (
            <Chip
              label={activeFilterCount}
              size="small"
              color="primary"
              sx={{ ml: 1, height: 20 }}
            />
          )}
        </Button>

        {/* Sort Dropdown */}
        <FormControl sx={{ minWidth: 150 }}>
          <InputLabel id="sort-select-label">Sort By</InputLabel>
          <Select
            labelId="sort-select-label"
            value={filters.sortBy || 'relevance'}
            onChange={handleSortChange}
            label="Sort By"
            size="medium"
          >
            {SORT_OPTIONS.map((option) => (
              <MenuItem key={option.value} value={option.value}>
                {option.label}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        {/* Clear All Filters */}
        {activeFilterCount > 0 && (
          <Button
            variant="text"
            startIcon={<Close />}
            onClick={handleClearFilters}
            color="error"
          >
            Clear All
          </Button>
        )}
      </Box>

      {/* Active Filters Display */}
      {isUsingAdvancedSearch && (
        <Box
          sx={{
            mb: 2,
            display: 'flex',
            gap: 1,
            flexWrap: 'wrap',
            alignItems: 'center',
          }}
        >
          <Typography variant="body2" color="text.secondary" fontWeight="medium">
            Active filters:
          </Typography>
          {filters.searchTerm && (
            <Chip
              label={`Search: "${filters.searchTerm}"`}
              onDelete={() => handleSearch('')}
              size="small"
              color="primary"
            />
          )}
          {filters.category && (
            <Chip
              label={`Category: ${filters.category}`}
              onDelete={() => removeFilter('category')}
              size="small"
            />
          )}
          {filters.city && (
            <Chip
              label={`City: ${filters.city}`}
              onDelete={() => removeFilter('city')}
              size="small"
            />
          )}
          {filters.country && (
            <Chip
              label={`Country: ${filters.country}`}
              onDelete={() => removeFilter('country')}
              size="small"
            />
          )}
          {filters.isVerified && (
            <Chip
              label="Verified Only"
              onDelete={() => removeFilter('isVerified')}
              size="small"
            />
          )}
          {filters.isOpenNow && (
            <Chip
              label="Open Now"
              onDelete={() => removeFilter('isOpenNow')}
              size="small"
            />
          )}
          {filters.is24Hours && (
            <Chip
              label="24 Hours"
              onDelete={() => removeFilter('is24Hours')}
              size="small"
            />
          )}
          {filters.amenities && filters.amenities.length > 0 && (
            <Chip
              label={`${filters.amenities.length} Amenities`}
              onDelete={() => removeFilter('amenities')}
              size="small"
            />
          )}
        </Box>
      )}

      {/* Advanced Filters Drawer */}
      <AdvancedFilters
        open={filtersOpen}
        onClose={() => setFiltersOpen(false)}
        filters={filters}
        onApply={handleFiltersApply}
      />

      {/* Main Content Area */}
      <Box>
        {/* Loading State */}
        {displayLoading && (
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
              {isUsingAdvancedSearch ? 'Searching places...' : 'Loading places...'}
            </Typography>
          </Box>
        )}

        {/* Error State */}
        {displayError && (
          <Alert severity="error" sx={{ mb: 3 }}>
            {displayError}
          </Alert>
        )}

        {/* Empty State */}
        {!displayLoading && !displayError && displayPlaces.length === 0 && (
          <Box
            sx={{
              textAlign: 'center',
              py: 8,
              px: 2,
            }}
          >
            <Typography variant="h6" color="text.secondary" gutterBottom>
              No places found
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
              {isUsingAdvancedSearch
                ? 'Try adjusting your search terms or filters to see more results.'
                : 'No places available at the moment. Please check back later.'}
            </Typography>
            {isUsingAdvancedSearch && (
              <Button variant="outlined" onClick={handleClearFilters} sx={{ mt: 2 }}>
                Clear All Filters
              </Button>
            )}
          </Box>
        )}

        {/* Places Grid */}
        {!displayLoading && !displayError && displayPlaces.length > 0 && (
          <>
            {/* Results Summary */}
            <Box sx={{ mb: 3, display: 'flex', alignItems: 'center', gap: 1 }}>
              <Typography variant="body2" color="text.secondary">
                Showing {displayPlaces.length} of {displayPagination.totalCount.toLocaleString()} places
              </Typography>
              {isUsingAdvancedSearch && (
                <Chip
                  label="Advanced Search"
                  size="small"
                  color="primary"
                  variant="outlined"
                />
              )}
            </Box>

            {/* Places Grid */}
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
                mb: 4,
              }}
            >
              {displayPlaces.map((place: any) => (
                <PlaceCard key={place.id} place={place} />
              ))}
            </Box>

            {/* Pagination */}
            {displayPagination.totalPages > 1 && (
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
                  count={displayPagination.totalPages}
                  page={displayPagination.page}
                  onChange={handlePageChange}
                  color="primary"
                  size="large"
                  showFirstButton
                  showLastButton
                  siblingCount={1}
                  boundaryCount={1}
                />
                <Typography variant="caption" color="text.secondary">
                  Page {displayPagination.page} of {displayPagination.totalPages}
                </Typography>
              </Box>
            )}
          </>
        )}
      </Box>
    </Container>
  );
};

export default PlaceListPage;
