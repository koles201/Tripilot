/**
 * Advanced Search Types
 * Type definitions for advanced search functionality
 */

export interface SearchFilters {
  searchTerm?: string;
  category?: string;
  city?: string;
  country?: string;
  minRating?: number;
  maxRating?: number;
  minPriceLevel?: number;
  maxPriceLevel?: number;
  isVerified?: boolean;
  amenities?: string[];
  isOpenNow?: boolean;
  is24Hours?: boolean;
  location?: LocationFilter;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

export interface LocationFilter {
  latitude: number;
  longitude: number;
  radiusKm: number;
}

export interface AdvancedSearchResult {
  id: string;
  name: string;
  description: string;
  category: string;
  city?: string;
  country?: string;
  address?: string;
  averageRating: number;
  reviewCount: number;
  priceLevel?: number;
  imageUrl?: string;
  isVerified: boolean;
  viewCount: number;
  distanceKm?: number;
  relevanceScore?: number;
  highlightedName?: string;
  highlightedDescription?: string;
  amenities?: string[];
  isOpenNow?: boolean;
  todayHours?: string;
}

export interface AdvancedSearchResponse {
  items: AdvancedSearchResult[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  appliedFilters?: SearchFilters;
  executionTimeMs: number;
}

export interface AutocompleteSuggestion {
  id?: string;
  text: string;
  type: 'place' | 'category' | 'city' | 'recent';
  category?: string;
  location?: string;
  imageUrl?: string;
  rating?: number;
  reviewCount?: number;
}

export interface AutocompleteResult {
  suggestions: AutocompleteSuggestion[];
  recentSearches?: string[];
}

export interface SearchHistoryItem {
  term: string;
  timestamp: number;
  filters?: Partial<SearchFilters>;
}
