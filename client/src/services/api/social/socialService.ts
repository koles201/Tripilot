import apiClient from '../apiClient';
import type {
  UserFollow,
  PaginatedUserList,
} from '../../../types/social';

/**
 * Service for social features (following/followers)
 */
export const socialService = {
  /**
   * Follow a user
   */
  async followUser(userId: string): Promise<UserFollow> {
    const response = await apiClient.post<UserFollow>('/social/follow', { userId });
    return response.data;
  },

  /**
   * Unfollow a user
   */
  async unfollowUser(userId: string): Promise<void> {
    await apiClient.post('/social/unfollow', { userId });
  },

  /**
   * Get followers of a user
   */
  async getFollowers(
    userId: string,
    pageNumber: number = 1,
    pageSize: number = 20
  ): Promise<PaginatedUserList> {
    const response = await apiClient.get<PaginatedUserList>(
      `/social/users/${userId}/followers`,
      { params: { pageNumber, pageSize } }
    );
    return response.data;
  },

  /**
   * Get users that a user is following
   */
  async getFollowing(
    userId: string,
    pageNumber: number = 1,
    pageSize: number = 20
  ): Promise<PaginatedUserList> {
    const response = await apiClient.get<PaginatedUserList>(
      `/social/users/${userId}/following`,
      { params: { pageNumber, pageSize } }
    );
    return response.data;
  },
};

export default socialService;
