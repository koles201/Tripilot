import React, { useState, useEffect } from 'react';
import {
  Drawer,
  Box,
  Typography,
  Button,
  IconButton,
  Divider,
  FormControl,
  FormLabel,
  FormGroup,
  FormControlLabel,
  Slider,
  Select,
  MenuItem,
  TextField,
  Chip,
  Switch,
  InputLabel,
} from '@mui/material';
import {
  Close as CloseIcon,
  FilterList as FilterIcon,
  Clear as ClearIcon,
} from '@mui/icons-material';
import type { SearchFilters } from '../../types/search';

// Constants
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
] as const;

const AMENITIES = [
  'WiFi',
  'Parking',
  'Wheelchair Accessible',
  'Pet Friendly',
  'Air Conditioning',
  'Outdoor Seating',
  'Credit Cards Accepted',
  'Reservations',
  'Takeout',
  'Delivery',
] as const;

const RATING_MARKS = [
  { value: 0, label: '0' },
  { value: 5, label: '5' },
];

const PRICE_MARKS = [
  { value: 1, label: '$' },
  { value: 2, label: '$$' },
  { value: 3, label: '$$$' },
  { value: 4, label: '$$$$' },
];

interface AdvancedFiltersProps {
  /** Whether the drawer is open */
  open: boolean;
  /** Callback to close the drawer */
  onClose: () => void;
  /** Current filter values */
  filters: SearchFilters;
  /** Callback when filters are applied */
  onApply: (filters: SearchFilters) => void;
}

/**
 * AdvancedFilters Component
 * 
 * Provides a comprehensive filter drawer for advanced search functionality.
 * Includes filters for category, location, rating, price, status, and amenities.
 * 
 * @component
 * @example
 * ```tsx
 * <AdvancedFilters
 *   open={isOpen}
 *   onClose={() => setIsOpen(false)}
 *   filters={currentFilters}
 *   onApply={(newFilters) => performSearch(newFilters)}
 * />
 * ```
 */
const AdvancedFilters: React.FC<AdvancedFiltersProps> = ({
  open,
  onClose,
  filters,
  onApply,
}) => {
  const [localFilters, setLocalFilters] = useState<SearchFilters>(filters);

  // Sync local filters with prop changes
  useEffect(() => {
    setLocalFilters(filters);
  }, [filters]);

  /**
   * Handle rating range slider change
   */
  const handleRatingChange = (_event: Event, newValue: number | number[]) => {
    if (Array.isArray(newValue)) {
      setLocalFilters({
        ...localFilters,
        minRating: newValue[0],
        maxRating: newValue[1],
      });
    }
  };

  /**
   * Handle price level slider change
   */
  const handlePriceLevelChange = (_event: Event, newValue: number | number[]) => {
    if (Array.isArray(newValue)) {
      setLocalFilters({
        ...localFilters,
        minPriceLevel: newValue[0],
        maxPriceLevel: newValue[1],
      });
    }
  };

  /**
   * Toggle amenity selection
   */
  const handleAmenityToggle = (amenity: string) => {
    const currentAmenities = localFilters.amenities || [];
    const newAmenities = currentAmenities.includes(amenity)
      ? currentAmenities.filter((a) => a !== amenity)
      : [...currentAmenities, amenity];

    setLocalFilters({
      ...localFilters,
      amenities: newAmenities.length > 0 ? newAmenities : undefined,
    });
  };

  /**
   * Apply filters and close drawer
   */
  const handleApply = () => {
    onApply(localFilters);
    onClose();
  };

  /**
   * Clear all filters except search term and pagination
   */
  const handleClear = () => {
    const clearedFilters: SearchFilters = {
      searchTerm: localFilters.searchTerm,
      page: 1,
      pageSize: localFilters.pageSize || 20,
      sortBy: 'relevance',
      sortDirection: 'desc',
    };
    setLocalFilters(clearedFilters);
  };

  /**
   * Calculate number of active filters
   */
  const getActiveFilterCount = (): number => {
    return [
      localFilters.category,
      localFilters.city,
      localFilters.country,
      localFilters.minRating !== undefined && localFilters.minRating > 0,
      localFilters.maxRating !== undefined && localFilters.maxRating < 5,
      localFilters.minPriceLevel !== undefined && localFilters.minPriceLevel > 1,
      localFilters.maxPriceLevel !== undefined && localFilters.maxPriceLevel < 4,
      localFilters.isVerified,
      localFilters.amenities && localFilters.amenities.length > 0,
      localFilters.isOpenNow,
      localFilters.is24Hours,
    ].filter(Boolean).length;
  };

  const activeFilterCount = getActiveFilterCount();

  return (
    <Drawer
      anchor="right"
      open={open}
      onClose={onClose}
      PaperProps={{
        sx: {
          width: { xs: '100%', sm: 400 },
          maxWidth: '100%',
        },
      }}
    >
      <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
        {/* Header */}
        <Box
          sx={{
            p: 3,
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
          }}
        >
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <FilterIcon color="primary" />
            <Typography variant="h6">
              Advanced Filters
            </Typography>
            {activeFilterCount > 0 && (
              <Chip
                label={activeFilterCount}
                size="small"
                color="primary"
                sx={{ fontWeight: 'bold' }}
              />
            )}
          </Box>
          <IconButton onClick={onClose} edge="end" aria-label="Close filters">
            <CloseIcon />
          </IconButton>
        </Box>

        <Divider />

        {/* Filters Content - Scrollable */}
        <Box sx={{ flex: 1, overflow: 'auto', px: 3, py: 2 }}>
          {/* Category Selection */}
          <FormControl fullWidth sx={{ mb: 3 }}>
            <InputLabel id="category-label">Category</InputLabel>
            <Select
              labelId="category-label"
              value={localFilters.category || ''}
              label="Category"
              onChange={(e) =>
                setLocalFilters({
                  ...localFilters,
                  category: e.target.value || undefined,
                })
              }
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

          {/* Location Filters */}
          <Box sx={{ mb: 3 }}>
            <Typography variant="subtitle2" gutterBottom fontWeight="bold">
              Location
            </Typography>
            <TextField
              fullWidth
              label="City"
              placeholder="e.g., Paris"
              value={localFilters.city || ''}
              onChange={(e) =>
                setLocalFilters({
                  ...localFilters,
                  city: e.target.value || undefined,
                })
              }
              sx={{ mb: 2 }}
            />
            <TextField
              fullWidth
              label="Country"
              placeholder="e.g., France"
              value={localFilters.country || ''}
              onChange={(e) =>
                setLocalFilters({
                  ...localFilters,
                  country: e.target.value || undefined,
                })
              }
            />
          </Box>

          <Divider sx={{ my: 2 }} />

          {/* Rating Range Slider */}
          <Box sx={{ mb: 3 }}>
            <Typography variant="subtitle2" gutterBottom fontWeight="bold">
              Rating Range
            </Typography>
            <Typography variant="caption" color="text.secondary" gutterBottom>
              {localFilters.minRating || 0} - {localFilters.maxRating || 5} stars
            </Typography>
            <Slider
              value={[localFilters.minRating || 0, localFilters.maxRating || 5]}
              onChange={handleRatingChange}
              valueLabelDisplay="auto"
              min={0}
              max={5}
              step={0.5}
              marks={RATING_MARKS}
              sx={{ mt: 2 }}
            />
          </Box>

          {/* Price Level Slider */}
          <Box sx={{ mb: 3 }}>
            <Typography variant="subtitle2" gutterBottom fontWeight="bold">
              Price Level
            </Typography>
            <Typography variant="caption" color="text.secondary" gutterBottom>
              {PRICE_MARKS.find((m) => m.value === (localFilters.minPriceLevel || 1))?.label} -{' '}
              {PRICE_MARKS.find((m) => m.value === (localFilters.maxPriceLevel || 4))?.label}
            </Typography>
            <Slider
              value={[
                localFilters.minPriceLevel || 1,
                localFilters.maxPriceLevel || 4,
              ]}
              onChange={handlePriceLevelChange}
              valueLabelDisplay="auto"
              min={1}
              max={4}
              step={1}
              marks={PRICE_MARKS}
              sx={{ mt: 2 }}
            />
          </Box>

          <Divider sx={{ my: 2 }} />

          {/* Status Switches */}
          <FormControl component="fieldset" sx={{ mb: 3 }}>
            <FormLabel component="legend" sx={{ fontWeight: 'bold' }}>
              Status Filters
            </FormLabel>
            <FormGroup sx={{ mt: 1 }}>
              <FormControlLabel
                control={
                  <Switch
                    checked={localFilters.isVerified || false}
                    onChange={(e) =>
                      setLocalFilters({
                        ...localFilters,
                        isVerified: e.target.checked || undefined,
                      })
                    }
                  />
                }
                label="Verified Only"
              />
              <FormControlLabel
                control={
                  <Switch
                    checked={localFilters.isOpenNow || false}
                    onChange={(e) =>
                      setLocalFilters({
                        ...localFilters,
                        isOpenNow: e.target.checked || undefined,
                      })
                    }
                  />
                }
                label="Open Now"
              />
              <FormControlLabel
                control={
                  <Switch
                    checked={localFilters.is24Hours || false}
                    onChange={(e) =>
                      setLocalFilters({
                        ...localFilters,
                        is24Hours: e.target.checked || undefined,
                      })
                    }
                  />
                }
                label="Open 24 Hours"
              />
            </FormGroup>
          </FormControl>

          <Divider sx={{ my: 2 }} />

          {/* Amenities Chips */}
          <Box sx={{ mb: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
              <Typography variant="subtitle2" fontWeight="bold">
                Amenities
              </Typography>
              {localFilters.amenities && localFilters.amenities.length > 0 && (
                <Chip
                  label={`${localFilters.amenities.length} selected`}
                  size="small"
                  color="primary"
                  variant="outlined"
                />
              )}
            </Box>
            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, mt: 2 }}>
              {AMENITIES.map((amenity) => {
                const isSelected = localFilters.amenities?.includes(amenity);
                return (
                  <Chip
                    key={amenity}
                    label={amenity}
                    onClick={() => handleAmenityToggle(amenity)}
                    color={isSelected ? 'primary' : 'default'}
                    variant={isSelected ? 'filled' : 'outlined'}
                    clickable
                    sx={{
                      transition: 'all 0.2s',
                      '&:hover': {
                        transform: 'translateY(-2px)',
                      },
                    }}
                  />
                );
              })}
            </Box>
          </Box>
        </Box>

        {/* Action Buttons */}
        <Box
          sx={{
            p: 2,
            borderTop: 1,
            borderColor: 'divider',
            bgcolor: 'background.paper',
          }}
        >
          <Box sx={{ display: 'flex', gap: 2 }}>
            <Button
              variant="outlined"
              startIcon={<ClearIcon />}
              onClick={handleClear}
              fullWidth
              disabled={activeFilterCount === 0}
            >
              Clear All
            </Button>
            <Button
              variant="contained"
              onClick={handleApply}
              fullWidth
            >
              Apply Filters
            </Button>
          </Box>
        </Box>
      </Box>
    </Drawer>
  );
};

export default AdvancedFilters;