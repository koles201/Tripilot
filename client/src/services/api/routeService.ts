import apiClient from './apiClient';
import type {
  Route,
  RoutesListResult,
  RouteQueryParams,
  CreateRouteRequest,
  UpdateRouteRequest,
  UpdateRoutePlacesRequest,
} from '../../types/route';

/**
 * Service for managing routes
 */
export const routeService = {
  /**
   * Get paginated list of routes with filters
   */
  async getRoutes(params?: RouteQueryParams): Promise<RoutesListResult> {
    const response = await apiClient.get<RoutesListResult>('/routes', { params });
    return response.data;
  },

  /**
   * Get a specific route by ID
   */
  async getRouteById(id: string): Promise<Route> {
    const response = await apiClient.get<Route>(`/routes/${id}`);
    return response.data;
  },

  /**
   * Get routes created by current user
   */
  async getUserRoutes(params?: RouteQueryParams): Promise<RoutesListResult> {
    const response = await apiClient.get<RoutesListResult>('/routes/my-routes', { params });
    return response.data;
  },

  /**
   * Create a new route
   */
  async createRoute(data: CreateRouteRequest): Promise<Route> {
    const response = await apiClient.post<Route>('/routes', data);
    return response.data;
  },

  /**
   * Update an existing route
   */
  async updateRoute(id: string, data: UpdateRouteRequest): Promise<Route> {
    const response = await apiClient.put<Route>(`/routes/${id}`, data);
    return response.data;
  },

  /**
   * Update places in a route (order, duration, notes)
   */
  async updateRoutePlaces(id: string, data: UpdateRoutePlacesRequest): Promise<Route> {
    const response = await apiClient.put<Route>(`/routes/${id}/places`, data);
    return response.data;
  },

  /**
   * Delete a route
   */
  async deleteRoute(id: string): Promise<void> {
    await apiClient.delete(`/routes/${id}`);
  },

  /**
   * Duplicate a route
   */
  async duplicateRoute(id: string): Promise<Route> {
    const response = await apiClient.post<Route>(`/routes/${id}/duplicate`);
    return response.data;
  },

  /**
   * Publish or unpublish a route
   */
  async togglePublishRoute(id: string, isPublished: boolean): Promise<Route> {
    const response = await apiClient.patch<Route>(`/routes/${id}/publish`, { isPublished });
    return response.data;
  },

  /**
   * Get optimized route order
   */
  async getOptimizedRoute(placeIds: string[]): Promise<{ optimizedOrder: string[] }> {
    const response = await apiClient.post<{ optimizedOrder: string[] }>(
      '/routes/optimize',
      { placeIds }
    );
    return response.data;
  },
};

export default routeService;
