import type { PlaceListItem } from './place';

/**
 * Route difficulty levels
 */
export const RouteDifficulty = {
  Easy: 1,
  Moderate: 2,
  Challenging: 3,
  Difficult: 4,
} as const;

export type RouteDifficultyType = (typeof RouteDifficulty)[keyof typeof RouteDifficulty];

/**
 * Route difficulty names
 */
export const RouteDifficultyNames: Record<RouteDifficultyType, string> = {
  [RouteDifficulty.Easy]: 'Easy',
  [RouteDifficulty.Moderate]: 'Moderate',
  [RouteDifficulty.Challenging]: 'Challenging',
  [RouteDifficulty.Difficult]: 'Difficult',
};

/**
 * Route privacy settings
 */
export const RoutePrivacy = {
  Public: 1,
  Private: 2,
  Unlisted: 3,
} as const;

export type RoutePrivacyType = (typeof RoutePrivacy)[keyof typeof RoutePrivacy];

/**
 * Route privacy names
 */
export const RoutePrivacyNames: Record<RoutePrivacyType, string> = {
  [RoutePrivacy.Public]: 'Public',
  [RoutePrivacy.Private]: 'Private',
  [RoutePrivacy.Unlisted]: 'Unlisted',
};

/**
 * Place in a route with ordering and timing
 */
export interface RoutePlace {
  id: string;
  routeId: string;
  placeId: string;
  place: PlaceListItem;
  order: number;
  duration?: number; // minutes to spend at place
  notes?: string;
  arrivalTime?: string;
}

/**
 * Full route entity
 */
export interface Route {
  id: string;
  name: string;
  description: string;
  userId: string;
  userName?: string;
  
  // Route properties
  difficulty: RouteDifficultyType;
  privacy: RoutePrivacyType;
  estimatedDuration?: number; // total minutes
  totalDistance?: number; // total kilometers
  
  // Places in route
  routePlaces: RoutePlace[];
  
  // Statistics
  viewCount: number;
  favoriteCount: number;
  rating?: number;
  reviewCount: number;
  
  // Metadata
  isPublished: boolean;
  createdAt: string;
  updatedAt?: string;
}

/**
 * Simplified route for list views
 */
export interface RouteListItem {
  id: string;
  name: string;
  description: string;
  userId: string;
  userName?: string;
  difficulty: RouteDifficultyType;
  privacy: RoutePrivacyType;
  estimatedDuration?: number;
  totalDistance?: number;
  placeCount: number;
  viewCount: number;
  rating?: number;
  reviewCount: number;
  isPublished: boolean;
  thumbnailUrl?: string;
  createdAt: string;
}

/**
 * Paginated routes response
 */
export interface RoutesListResult {
  items: RouteListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * Route query parameters
 */
export interface RouteQueryParams {
  page?: number;
  pageSize?: number;
  difficulty?: RouteDifficultyType;
  privacy?: RoutePrivacyType;
  userId?: string;
  sortBy?: 'rating' | 'distance' | 'duration' | 'newest';
}

/**
 * Create route request
 */
export interface CreateRouteRequest {
  name: string;
  description: string;
  difficulty: RouteDifficultyType;
  privacy: RoutePrivacyType;
  placeIds: string[];
}

/**
 * Update route request
 */
export interface UpdateRouteRequest {
  name?: string;
  description?: string;
  difficulty?: RouteDifficultyType;
  privacy?: RoutePrivacyType;
  isPublished?: boolean;
}

/**
 * Update route places request
 */
export interface UpdateRoutePlacesRequest {
  places: Array<{
    placeId: string;
    order: number;
    duration?: number;
    notes?: string;
  }>;
}

/**
 * Route builder state
 */
export interface RouteBuilderState {
  name: string;
  description: string;
  difficulty: RouteDifficultyType;
  privacy: RoutePrivacyType;
  selectedPlaces: PlaceListItem[];
  isOptimized: boolean;
}
