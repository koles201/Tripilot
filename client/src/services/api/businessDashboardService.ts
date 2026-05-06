import apiClient from './apiClient';
import type {
  DashboardData,
  BusinessAnalytics,
  AnalyticsQueryParams,
} from '../../types/businessDashboard';

/**
 * Business Dashboard API Service
 * Handles all business dashboard-related API calls
 */
class BusinessDashboardService {
  /**
   * Get complete business dashboard data
   * Includes business profile, analytics, claimed places, and pending claims
   */
  async getDashboard(): Promise<DashboardData> {
    const response = await apiClient.get<DashboardData>('/business-dashboard');
    return response.data;
  }

  /**
   * Get detailed business analytics with optional date range filtering
   * @param params Optional query parameters for date range filtering
   */
  async getAnalytics(params?: AnalyticsQueryParams): Promise<BusinessAnalytics> {
    const response = await apiClient.get<BusinessAnalytics>('/business-dashboard/analytics', {
      params,
    });
    return response.data;
  }
}

export default new BusinessDashboardService();
