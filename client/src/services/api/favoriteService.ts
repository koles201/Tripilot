import apiClient from './apiClient';
import type {
  Favorite,
  FavoriteListResult,
  FavoriteQueryParams,
  IsFavoriteResponse,
  AddFavoriteRequest,
} from '../../types/favorite';

/**
 * Favorites API Service
 * 
 * Handles all API operations related to user favorites/bookmarks.
 * Provides methods for adding, removing, and fetching favorites.
 * 
 * @class FavoriteService
 */
class FavoriteService {
  /**
   * Add a place to user's favorites
   * 
   * @param placeId - ID of the place to favorite
   * @returns Promise resolving to the created favorite
   * @throws Error if API request fails
   * 
   * @example
   * ```typescript
   * const favorite = await favoriteService.addFavorite('place-123');
   * ```
   */
  async addFavorite(placeId: string): Promise<Favorite> {
    try {
      const request: AddFavoriteRequest = { placeId };
      const response = await apiClient.post<Favorite>('/favorite', request);
      return response.data;
    } catch (error: any) {
      console.error('Add favorite failed:', error);
      throw new Error(
        error.response?.data?.message || 'Failed to add favorite. Please try again.'
      );
    }
  }

  /**
   * Remove a place from user's favorites
   * 
   * @param placeId - ID of the place to unfavorite
   * @returns Promise resolving when favorite is removed
   * @throws Error if API request fails
   * 
   * @example
   * ```typescript
   * await favoriteService.removeFavorite('place-123');
   * ```
   */
  async removeFavorite(placeId: string): Promise<void> {
    try {
      await apiClient.delete(`/favorite/${placeId}`);
    } catch (error: any) {
      console.error('Remove favorite failed:', error);
      throw new Error(
        error.response?.data?.message || 'Failed to remove favorite. Please try again.'
      );
    }
  }

  /**
   * Get all favorites for the current user
   * 
   * @param params - Query parameters for pagination, filtering, and sorting
   * @returns Promise resolving to paginated list of favorites
   * @throws Error if API request fails
   * 
   * @example
   * ```typescript
   * const result = await favoriteService.getFavorites({
   *   page: 1,
   *   pageSize: 20,
   *   category: 'Restaurant',
   *   sortBy: 'createdAt',
   *   sortDirection: 'desc'
   * });
   * ```
   */
  async getFavorites(params?: FavoriteQueryParams): Promise<FavoriteListResult> {
    try {
      const response = await apiClient.get<FavoriteListResult>('/favorite', { params });
      return response.data;
    } catch (error: any) {
      console.error('Get favorites failed:', error);
      throw new Error(
        error.response?.data?.message || 'Failed to fetch favorites. Please try again.'
      );
    }
  }

  /**
   * Check if a place is favorited by the current user
   * 
   * @param placeId - ID of the place to check
   * @returns Promise resolving to favorite status
   * @throws Error if API request fails
   * 
   * @example
   * ```typescript
   * const status = await favoriteService.isFavorite('place-123');
   * if (status.isFavorite) {
   *   console.log('Place is favorited!');
   * }
   * ```
   */
  async isFavorite(placeId: string): Promise<IsFavoriteResponse> {
    try {
      const response = await apiClient.get<IsFavoriteResponse>(`/favorite/check/${placeId}`);
      return response.data;
    } catch (error: any) {
      console.error('Check favorite failed:', error);
      // Return false instead of throwing on error
      return { isFavorite: false };
    }
  }

  /**
   * Get the total count of user's favorites
   * 
   * @returns Promise resolving to the count
   * @throws Error if API request fails
   * 
   * @example
   * ```typescript
   * const count = await favoriteService.getFavoritesCount();
   * console.log(`You have ${count} favorites`);
   * ```
   */
  async getFavoritesCount(): Promise<number> {
    try {
      const response = await apiClient.get<number>('/favorite/count');
      return response.data;
    } catch (error: any) {
      console.error('Get favorites count failed:', error);
      return 0;
    }
  }

  /**
   * Toggle favorite status for a place
   * Convenience method that adds or removes based on current status
   * 
   * @param placeId - ID of the place
   * @param currentlyFavorited - Current favorite status
   * @returns Promise resolving to new favorite or void
   * 
   * @example
   * ```typescript
   * await favoriteService.toggleFavorite('place-123', false); // Adds favorite
   * await favoriteService.toggleFavorite('place-123', true);  // Removes favorite
   * ```
   */
  async toggleFavorite(placeId: string, currentlyFavorited: boolean): Promise<Favorite | void> {
    if (currentlyFavorited) {
      await this.removeFavorite(placeId);
    } else {
      return await this.addFavorite(placeId);
    }
  }
}

export default new FavoriteService();
