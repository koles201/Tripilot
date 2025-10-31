import apiClient from './apiClient';
import type {
  SearchFilters,
  AdvancedSearchResponse,
  AutocompleteResult,
  SearchHistoryItem,
} from '../../types/search';

// Constants
const SEARCH_HISTORY_KEY = 'tripilot_search_history';
const MAX_HISTORY_ITEMS = 10;
const MAX_RECENT_SUGGESTIONS = 3;

/**
 * Advanced Search API Service
 * 
 * Provides comprehensive search functionality including:
 * - Advanced search with 20+ filter parameters
 * - Autocomplete suggestions for places, categories, and cities
 * - Search history management with localStorage
 * 
 * @class SearchService
 */
class SearchService {
  /**
   * Perform advanced search with comprehensive filtering
   * 
   * @param filters - Search filters including search term, category, location, rating, etc.
   * @returns Promise resolving to search results with metadata
   * @throws Error if API request fails
   * 
   * @example
   * ```typescript
   * const results = await searchService.advancedSearch({
   *   searchTerm: 'coffee',
   *   category: 'Restaurant',
   *   minRating: 4.0,
   *   city: 'Paris'
   * });
   * ```
   */
  async advancedSearch(filters: SearchFilters): Promise<AdvancedSearchResponse> {
    const params = this.buildSearchParams(filters);

    try {
      const response = await apiClient.get<AdvancedSearchResponse>(
        '/place/advanced-search',
        { params }
      );

      // Save successful search to history
      if (filters.searchTerm) {
        this.addToHistory(filters.searchTerm, filters);
      }

      return response.data;
    } catch (error) {
      console.error('Advanced search failed:', error);
      throw new Error('Failed to perform search. Please try again.');
    }
  }

  /**
   * Build query parameters from search filters
   * Converts SearchFilters object to API query parameters
   * 
   * @param filters - Search filters
   * @returns Record of query parameters
   * @private
   */
  private buildSearchParams(filters: SearchFilters): Record<string, any> {
    const params: Record<string, any> = {
      page: filters.page || 1,
      pageSize: filters.pageSize || 20,
    };

    // Add optional parameters only if they exist
    const paramMapping: Array<[keyof SearchFilters, string]> = [
      ['searchTerm', 'searchTerm'],
      ['category', 'category'],
      ['city', 'city'],
      ['country', 'country'],
      ['minRating', 'minRating'],
      ['maxRating', 'maxRating'],
      ['minPriceLevel', 'minPriceLevel'],
      ['maxPriceLevel', 'maxPriceLevel'],
      ['isVerified', 'isVerified'],
      ['isOpenNow', 'isOpenNow'],
      ['is24Hours', 'is24Hours'],
      ['sortBy', 'sortBy'],
      ['sortDirection', 'sortDirection'],
    ];

    // Map filter values to params
    paramMapping.forEach(([filterKey, paramKey]) => {
      const value = filters[filterKey];
      if (value !== undefined && value !== null && value !== '') {
        params[paramKey] = value;
      }
    });

    // Handle array parameters
    if (filters.amenities && filters.amenities.length > 0) {
      params.amenities = filters.amenities.join(',');
    }

    // Handle location parameters
    if (filters.location) {
      params.latitude = filters.location.latitude;
      params.longitude = filters.location.longitude;
      params.radiusKm = filters.location.radiusKm;
    }

    return params;
  }

  /**
   * Get autocomplete suggestions for a search term
   * Combines API suggestions with recent search history
   * 
   * @param searchTerm - Partial search term
   * @param maxSuggestions - Maximum number of suggestions to return (default: 10)
   * @returns Promise resolving to autocomplete results
   * @throws Error if API request fails
   * 
   * @example
   * ```typescript
   * const suggestions = await searchService.getAutocomplete('par', 10);
   * ```
   */
  async getAutocomplete(
    searchTerm: string,
    maxSuggestions: number = 10
  ): Promise<AutocompleteResult> {
    try {
      const response = await apiClient.get<AutocompleteResult>('/place/suggestions', {
        params: { searchTerm, maxSuggestions },
      });

      // Merge with relevant recent searches
      const history = this.getHistory();
      const recentSearches = history
        .filter((item) =>
          item.term.toLowerCase().includes(searchTerm.toLowerCase())
        )
        .slice(0, MAX_RECENT_SUGGESTIONS)
        .map((item) => item.term);

      return {
        suggestions: response.data.suggestions,
        recentSearches,
      };
    } catch (error) {
      console.error('Autocomplete request failed:', error);
      // Return empty results on error
      return {
        suggestions: [],
        recentSearches: [],
      };
    }
  }

  // ==================== Search History Management ====================

  /**
   * Get search history from localStorage
   * 
   * @returns Array of search history items, most recent first
   */
  getHistory(): SearchHistoryItem[] {
    try {
      const history = localStorage.getItem(SEARCH_HISTORY_KEY);
      return history ? JSON.parse(history) : [];
    } catch (error) {
      console.error('Error reading search history:', error);
      return [];
    }
  }

  /**
   * Add search term to history
   * Removes duplicates and maintains max history size
   * 
   * @param term - Search term to add
   * @param filters - Optional associated filters
   */
  addToHistory(term: string, filters?: Partial<SearchFilters>): void {
    if (!term.trim()) return;

    try {
      const history = this.getHistory();

      // Remove existing entry with same term (case-insensitive)
      const filtered = history.filter(
        (item) => item.term.toLowerCase() !== term.toLowerCase()
      );

      // Add new item at the beginning
      const newHistory: SearchHistoryItem[] = [
        {
          term: term.trim(),
          timestamp: Date.now(),
          filters,
        },
        ...filtered,
      ].slice(0, MAX_HISTORY_ITEMS); // Limit to max items

      localStorage.setItem(SEARCH_HISTORY_KEY, JSON.stringify(newHistory));
    } catch (error) {
      console.error('Error saving search history:', error);
    }
  }

  /**
   * Clear all search history
   */
  clearHistory(): void {
    try {
      localStorage.removeItem(SEARCH_HISTORY_KEY);
    } catch (error) {
      console.error('Error clearing search history:', error);
    }
  }

  /**
   * Remove a specific item from history
   * 
   * @param term - Search term to remove
   */
  removeFromHistory(term: string): void {
    try {
      const history = this.getHistory();
      const filtered = history.filter((item) => item.term !== term);
      localStorage.setItem(SEARCH_HISTORY_KEY, JSON.stringify(filtered));
    } catch (error) {
      console.error('Error removing from search history:', error);
    }
  }
}

export default new SearchService();
