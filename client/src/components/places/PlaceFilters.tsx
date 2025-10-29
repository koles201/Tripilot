import type { FC } from 'react';
import {
  Box,
  Chip,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Typography,
  Stack,
  Slider,
  FormControlLabel,
  Switch,
} from '@mui/material';
import { PlaceCategory, PlaceCategoryNames } from '../../types/enums';
import type { PlaceFilters as PlaceFiltersType } from '../../types/place';

interface PlaceFiltersProps {
  filters: PlaceFiltersType;
  onChange: (filters: PlaceFiltersType) => void;
}

const PlaceFilters: FC<PlaceFiltersProps> = ({ filters, onChange }) => {
  const categories = Object.entries(PlaceCategory).map(([, value]) => ({
    value,
    label: PlaceCategoryNames[value as keyof typeof PlaceCategoryNames],
  }));

  const handleCategoryClick = (category: number) => {
    onChange({
      ...filters,
      category: filters.category === category ? undefined : (category as any),
    });
  };

  const handleMinRatingChange = (_event: Event, value: number | number[]) => {
    onChange({
      ...filters,
      minRating: value as number,
    });
  };

  const handlePriceLevelChange = (event: any) => {
    const value = event.target.value;
    onChange({
      ...filters,
      priceLevel: value === '' ? undefined : Number(value),
    });
  };

  const handleVerifiedChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    onChange({
      ...filters,
      isVerified: event.target.checked ? true : undefined,
    });
  };

  const handleClearFilters = () => {
    onChange({});
  };

  const hasActiveFilters =
    filters.category !== undefined ||
    filters.minRating !== undefined ||
    filters.priceLevel !== undefined ||
    filters.isVerified !== undefined;

  return (
    <Box sx={{ p: 2, borderRadius: 1, bgcolor: 'background.paper' }}>
      <Stack spacing={3}>
        {/* Categories */}
        <Box>
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
            <Typography variant="subtitle2" fontWeight="bold">
              Categories
            </Typography>
            {hasActiveFilters && (
              <Chip
                label="Clear All"
                size="small"
                onClick={handleClearFilters}
                onDelete={handleClearFilters}
              />
            )}
          </Box>
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
            {categories.map((cat) => (
              <Chip
                key={cat.value}
                label={cat.label}
                onClick={() => handleCategoryClick(cat.value)}
                color={filters.category === cat.value ? 'primary' : 'default'}
                variant={filters.category === cat.value ? 'filled' : 'outlined'}
              />
            ))}
          </Box>
        </Box>

        {/* Minimum Rating */}
        <Box>
          <Typography variant="subtitle2" fontWeight="bold" gutterBottom>
            Minimum Rating: {filters.minRating || 0}
          </Typography>
          <Slider
            value={filters.minRating || 0}
            onChange={handleMinRatingChange}
            min={0}
            max={5}
            step={0.5}
            marks
            valueLabelDisplay="auto"
            sx={{ mt: 2 }}
          />
        </Box>

        {/* Price Level */}
        <FormControl fullWidth size="small">
          <InputLabel>Price Level</InputLabel>
          <Select
            value={filters.priceLevel || ''}
            onChange={handlePriceLevelChange}
            label="Price Level"
          >
            <MenuItem value="">Any</MenuItem>
            <MenuItem value={1}>$ - Budget</MenuItem>
            <MenuItem value={2}>$$ - Moderate</MenuItem>
            <MenuItem value={3}>$$$ - Expensive</MenuItem>
            <MenuItem value={4}>$$$$ - Very Expensive</MenuItem>
          </Select>
        </FormControl>

        {/* Verified Only */}
        <FormControlLabel
          control={
            <Switch
              checked={filters.isVerified || false}
              onChange={handleVerifiedChange}
            />
          }
          label="Verified places only"
        />
      </Stack>
    </Box>
  );
};

export default PlaceFilters;
