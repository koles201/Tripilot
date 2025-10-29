import type { PlaceCategoryType } from './enums';

/**
 * Place entity type matching backend PlaceResponse
 */
export interface Place {
  id: string;
  name: string;
  description: string;
  category: PlaceCategoryType;

  // Location
  latitude: number;
  longitude: number;
  address?: string;
  city?: string;
  country?: string;
  postalCode?: string;

  // Contact Info
  phone?: string;
  email?: string;
  website?: string;

  // Operating Hours
  openTime?: string;
  closeTime?: string;
  daysOfWeek?: string;
  is24Hours: boolean;
  specialNotes?: string;

  // Ratings and Reviews
  averageRating: number;
  reviewCount: number;

  // Additional Info
  priceLevel?: number;
  amenities?: string;
  imageUrl?: string;
  galleryImages?: string;

  // Metadata
  isVerified: boolean;
  isActive: boolean;
  viewCount: number;
  ownerId?: string;
  ownerName?: string;

  // Audit
  createdAt: string;
  modifiedAt?: string;
}

/**
 * Simplified place type for list views
 */
export interface PlaceListItem {
  id: string;
  name: string;
  description: string;
  category: PlaceCategoryType;
  
  // Location
  latitude?: number;
  longitude?: number;
  address?: string;
  city?: string;
  country?: string;
  
  // Ratings
  averageRating: number;
  reviewCount: number;
  priceLevel?: number;
  
  // Media
  imageUrl?: string;
  photoUrl?: string;
  coverImageUrl?: string;
  
  // Metadata
  isVerified: boolean;
  viewCount: number;
}

/**
 * Paginated places response
 */
export interface PlacesListResult {
  places: PlaceListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * Place search and filter parameters
 */
export interface PlaceFilters {
  searchTerm?: string;
  category?: PlaceCategoryType;
  city?: string;
  country?: string;
  minRating?: number;
  priceLevel?: number;
  isVerified?: boolean;
  
  // Location-based search
  latitude?: number;
  longitude?: number;
  radiusKm?: number;
}

/**
 * Place query parameters
 */
export interface PlaceQueryParams extends PlaceFilters {
  page?: number;
  pageSize?: number;
  sortBy?: 'rating' | 'distance' | 'name' | 'newest';
}

/**
 * Nearby places query parameters
 */
export interface NearbyPlacesParams {
  latitude: number;
  longitude: number;
  radiusKm: number;
  category?: PlaceCategoryType;
  minRating?: number;
  maxResults?: number;
}
