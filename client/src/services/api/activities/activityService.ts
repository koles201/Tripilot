import apiClient from '../apiClient';
import type {
  PaginatedActivities,
  GetActivityFeedRequest,
} from '../../../types/activity';

/**
 * Service for activity feeds
 */
export const activityService = {
  /**
   * Get activity feed for the authenticated user (showing followed users' activities)
   */
  async getActivityFeed(params?: GetActivityFeedRequest): Promise<PaginatedActivities> {
    const response = await apiClient.get<PaginatedActivities>('/activities/feed', {
      params: {
        followedOnly: params?.followedOnly ?? true,
        activityType: params?.activityType,
        pageNumber: params?.pageNumber ?? 1,
        pageSize: params?.pageSize ?? 20,
      },
    });
    return response.data;
  },

  /**
   * Get activity feed for a specific user
   */
  async getUserActivities(
    userId: string,
    params?: GetActivityFeedRequest
  ): Promise<PaginatedActivities> {
    const response = await apiClient.get<PaginatedActivities>(
      `/activities/user/${userId}`,
      {
        params: {
          activityType: params?.activityType,
          pageNumber: params?.pageNumber ?? 1,
          pageSize: params?.pageSize ?? 20,
        },
      }
    );
    return response.data;
  },

  /**
   * Get global activity feed (all public activities)
   */
  async getGlobalFeed(params?: GetActivityFeedRequest): Promise<PaginatedActivities> {
    const response = await apiClient.get<PaginatedActivities>('/activities/global', {
      params: {
        activityType: params?.activityType,
        pageNumber: params?.pageNumber ?? 1,
        pageSize: params?.pageSize ?? 20,
      },
    });
    return response.data;
  },
};

export default activityService;
