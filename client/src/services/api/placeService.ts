import apiClient from './apiClient';
import type {
  Place,
  PlaceListItem,
  PlacesListResult,
  PlaceQueryParams,
  NearbyPlacesParams,
} from '../../types/place';

/**
 * Place API Service
 * Handles all place-related API calls
 */
class PlaceService {
  /**
   * Get places list with optional filters and pagination
   */
  async getPlaces(params?: PlaceQueryParams): Promise<PlacesListResult> {
    const response = await apiClient.get<PlacesListResult>('/place', { params });
    return response.data;
  }

  /**
   * Search places by text and optional location
   */
  async searchPlaces(params: PlaceQueryParams): Promise<PlacesListResult> {
    const response = await apiClient.get<PlacesListResult>('/place/search', { params });
    return response.data;
  }

  /**
   * Get nearby places based on coordinates
   */
  async getNearbyPlaces(params: NearbyPlacesParams): Promise<PlaceListItem[]> {
    const response = await apiClient.get<PlaceListItem[]>('/place/nearby', { params });
    return response.data;
  }

  /**
   * Get single place by ID
   */
  async getPlaceById(id: string): Promise<Place> {
    const response = await apiClient.get<Place>(`/place/${id}`);
    return response.data;
  }

  /**
   * Get all categories (if endpoint exists)
   */
  async getCategories(): Promise<{ id: number; name: string }[]> {
    const response = await apiClient.get<{ id: number; name: string }[]>('/categories');
    return response.data;
  }
}

export default new PlaceService();
