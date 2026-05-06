/**
 * Favorite (bookmark) type definitions
 */

/**
 * Favorite entity
 */
export interface Favorite {
  /** Unique identifier */
  id: string;
  /** User ID who favorited */
  userId: string;
  /** Place ID that was favorited */
  placeId: string;
  /** Place name */
  placeName: string;
  /** Place category */
  category: string;
  /** Place city */
  city?: string;
  /** Place country */
  country?: string;
  /** Place image URL */
  imageUrl?: string;
  /** Place average rating */
  averageRating?: number;
  /** When the favorite was created */
  createdAt: string;
}

/**
 * Result of checking if a place is favorited
 */
export interface IsFavoriteResponse {
  /** Whether the place is favorited */
  isFavorite: boolean;
  /** Favorite ID if it exists */
  favoriteId?: string;
}

/**
 * Request to add a favorite
 */
export interface AddFavoriteRequest {
  /** ID of the place to favorite */
  placeId: string;
}

/**
 * Paginated list of favorites
 */
export interface FavoriteListResult {
  /** List of favorites */
  favorites: Favorite[];
  /** Total count of favorites */
  totalCount: number;
  /** Current page number */
  page: number;
  /** Page size */
  pageSize: number;
  /** Total number of pages */
  totalPages: number;
}

/**
 * Query parameters for fetching favorites
 */
export interface FavoriteQueryParams {
  /** Page number (1-based) */
  page?: number;
  /** Page size */
  pageSize?: number;
  /** Filter by category */
  category?: string;
  /** Sort by: createdAt, name, rating */
  sortBy?: 'createdAt' | 'name' | 'rating';
  /** Sort direction */
  sortDirection?: 'asc' | 'desc';
}

/**
 * Favorite status for a place
 */
export interface FavoriteStatus {
  /** Place ID */
  placeId: string;
  /** Whether it's favorited */
  isFavorite: boolean;
  /** Favorite ID if exists */
  favoriteId?: string;
}
