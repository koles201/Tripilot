import React, { useState, useCallback, useEffect } from 'react';
import {
  Autocomplete,
  TextField,
  Box,
  Typography,
  IconButton,
  Chip,
  CircularProgress,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Avatar,
} from '@mui/material';
import {
  Search as SearchIcon,
  Clear as ClearIcon,
  History as HistoryIcon,
  Place as PlaceIcon,
  Category as CategoryIcon,
  LocationOn as LocationIcon,
  Star,
} from '@mui/icons-material';
import searchService from '../../services/api/searchService';
import type { AutocompleteSuggestion } from '../../types/search';

/**
 * Debounce utility function to delay function execution
 * @param func - Function to debounce
 * @param wait - Delay in milliseconds
 * @returns Debounced function
 */
const debounce = <T extends (...args: any[]) => any>(
  func: T,
  wait: number
): ((...args: Parameters<T>) => void) => {
  let timeout: ReturnType<typeof setTimeout>;
  return (...args: Parameters<T>) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
};

// Constants
const MIN_SEARCH_LENGTH = 2;
const DEBOUNCE_DELAY = 300;
const MAX_RECENT_SEARCHES = 5;

interface SearchBarProps {
  /** Callback function when a search is performed */
  onSearch: (searchTerm: string) => void;
  /** Placeholder text for the search input */
  placeholder?: string;
  /** Whether to auto-focus the input on mount */
  autoFocus?: boolean;
}

/**
 * SearchBar Component with Autocomplete
 * 
 * Provides an intelligent search bar with autocomplete suggestions for places,
 * categories, and cities. Features debounced search, recent search history,
 * and rich suggestion rendering.
 * 
 * @component
 * @example
 * ```tsx
 * <SearchBar
 *   onSearch={(term) => console.log('Searching for:', term)}
 *   placeholder="Find amazing places..."
 *   autoFocus
 * />
 * ```
 */
const SearchBar: React.FC<SearchBarProps> = ({
  onSearch,
  placeholder = 'Search places, categories, or cities...',
  autoFocus = false,
}) => {
  // State management
  const [inputValue, setInputValue] = useState('');
  const [suggestions, setSuggestions] = useState<AutocompleteSuggestion[]>([]);
  const [recentSearches, setRecentSearches] = useState<string[]>([]);
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);

  /**
   * Load recent search history on component mount
   */
  useEffect(() => {
    const history = searchService.getHistory();
    setRecentSearches(history.map((item) => item.term).slice(0, MAX_RECENT_SEARCHES));
  }, []);

  /**
   * Fetch autocomplete suggestions from the API
   * Debounced to avoid excessive API calls
   */
  const fetchSuggestions = useCallback(
    debounce(async (searchTerm: string) => {
      if (!searchTerm || searchTerm.length < MIN_SEARCH_LENGTH) {
        setSuggestions([]);
        setLoading(false);
        return;
      }

      try {
        setLoading(true);
        const result = await searchService.getAutocomplete(searchTerm);
        setSuggestions(result.suggestions);
        
        // Update recent searches if provided
        if (result.recentSearches) {
          setRecentSearches(result.recentSearches);
        }
      } catch (error) {
        console.error('Error fetching suggestions:', error);
        setSuggestions([]);
      } finally {
        setLoading(false);
      }
    }, DEBOUNCE_DELAY),
    []
  );

  /**
   * Handle input value changes
   */
  const handleInputChange = (_event: React.SyntheticEvent, value: string) => {
    setInputValue(value);
    
    if (value.length >= MIN_SEARCH_LENGTH) {
      setLoading(true);
      fetchSuggestions(value);
    } else {
      setSuggestions([]);
      setLoading(false);
    }
  };

  /**
   * Handle suggestion selection
   */
  const handleSelect = (
    _event: React.SyntheticEvent,
    value: AutocompleteSuggestion | string | null
  ) => {
    if (!value) return;

    const searchTerm = typeof value === 'string' ? value : value.text;
    setInputValue(searchTerm);
    onSearch(searchTerm);
    setOpen(false);
  };

  /**
   * Clear the search input
   */
  const handleClear = () => {
    setInputValue('');
    setSuggestions([]);
  };

  /**
   * Handle Enter key press for search
   */
  const handleKeyPress = (event: React.KeyboardEvent) => {
    if (event.key === 'Enter' && inputValue.trim()) {
      onSearch(inputValue.trim());
      setOpen(false);
    }
  };

  /**
   * Get icon for suggestion type
   */
  /**
   * Get icon for suggestion type
   */
  const getSuggestionIcon = (type: string): React.ReactElement => {
    const iconMap: Record<string, React.ReactElement> = {
      place: <PlaceIcon />,
      category: <CategoryIcon />,
      city: <LocationIcon />,
      recent: <HistoryIcon />,
    };
    return iconMap[type] || <SearchIcon />;
  };

  /**
   * Render autocomplete option with rich content
   */
  const renderOption = (props: any, option: AutocompleteSuggestion | string) => {
    // Render recent search
    if (typeof option === 'string') {
      return (
        <ListItem {...props} key={`recent-${option}`}>
          <ListItemAvatar>
            <Avatar sx={{ bgcolor: 'action.selected' }}>
              <HistoryIcon />
            </Avatar>
          </ListItemAvatar>
          <ListItemText primary={option} secondary="Recent search" />
        </ListItem>
      );
    }

    // Render suggestion with full details
    return (
      <ListItem {...props} key={`suggestion-${option.type}-${option.text}`}>
        <ListItemAvatar>
          {option.imageUrl ? (
            <Avatar src={option.imageUrl} alt={option.text} />
          ) : (
            <Avatar sx={{ bgcolor: 'primary.main' }}>
              {getSuggestionIcon(option.type)}
            </Avatar>
          )}
        </ListItemAvatar>
        <ListItemText
          primary={
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
              <Typography variant="body1">{option.text}</Typography>
              {option.rating && (
                <Chip
                  icon={<Star fontSize="small" />}
                  label={option.rating.toFixed(1)}
                  size="small"
                  color="warning"
                  sx={{ height: 20 }}
                />
              )}
            </Box>
          }
          secondary={
            <Box component="span">
              {option.category && (
                <Typography variant="caption" color="text.secondary" component="span">
                  {option.category}
                </Typography>
              )}
              {option.location && (
                <Typography variant="caption" color="text.secondary" component="span" sx={{ ml: 1 }}>
                  • {option.location}
                </Typography>
              )}
              {option.reviewCount && (
                <Typography variant="caption" color="text.secondary" component="span" sx={{ ml: 1 }}>
                  • {option.reviewCount} reviews
                </Typography>
              )}
            </Box>
          }
        />
      </ListItem>
    );
  };

  /**
   * Extract label from option
   */
  const getOptionLabel = (option: AutocompleteSuggestion | string): string => {
    return typeof option === 'string' ? option : option.text;
  };

  // Combine suggestions with recent searches when input is short
  const allOptions =
    inputValue.length < MIN_SEARCH_LENGTH && recentSearches.length > 0
      ? recentSearches
      : suggestions;

  return (
    <Box sx={{ width: '100%' }}>
      <Autocomplete
        freeSolo
        open={open}
        onOpen={() => setOpen(true)}
        onClose={() => setOpen(false)}
        options={allOptions}
        getOptionLabel={getOptionLabel}
        inputValue={inputValue}
        onInputChange={handleInputChange}
        onChange={handleSelect}
        loading={loading}
        filterOptions={(options) => options} // Disable built-in filtering (we handle it server-side)
        renderOption={renderOption}
        noOptionsText={
          inputValue.length < MIN_SEARCH_LENGTH
            ? `Type at least ${MIN_SEARCH_LENGTH} characters to search`
            : 'No results found'
        }
        renderInput={(params) => (
          <TextField
            {...params}
            placeholder={placeholder}
            autoFocus={autoFocus}
            onKeyPress={handleKeyPress}
            InputProps={{
              ...params.InputProps,
              startAdornment: (
                <SearchIcon sx={{ color: 'action.active', mr: 1, ml: 0.5 }} />
              ),
              endAdornment: (
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                  {loading && <CircularProgress size={20} sx={{ mr: 1 }} />}
                  {inputValue && (
                    <IconButton
                      size="small"
                      onClick={handleClear}
                      edge="end"
                      aria-label="Clear search"
                      sx={{ mr: 0.5 }}
                    >
                      <ClearIcon fontSize="small" />
                    </IconButton>
                  )}
                  {params.InputProps.endAdornment}
                </Box>
              ),
            }}
            sx={{
              '& .MuiOutlinedInput-root': {
                paddingLeft: 1.5,
                '&:hover': {
                  '& .MuiOutlinedInput-notchedOutline': {
                    borderColor: 'primary.main',
                  },
                },
              },
            }}
          />
        )}
      />
    </Box>
  );
};

export default SearchBar;