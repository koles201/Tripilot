import apiClient from '../apiClient';
import type {
  RouteCollection,
  CollectionDetail,
  CreateCollectionRequest,
  AddRouteToCollectionRequest,
  PaginatedCollections,
} from '../../../types/collection';

/**
 * Service for route collections
 */
export const collectionService = {
  /**
   * Create a new collection
   */
  async createCollection(data: CreateCollectionRequest): Promise<RouteCollection> {
    const response = await apiClient.post<RouteCollection>('/collections', data);
    return response.data;
  },

  /**
   * Add a route to a collection
   */
  async addRouteToCollection(
    collectionId: string,
    data: AddRouteToCollectionRequest
  ): Promise<void> {
    await apiClient.post(`/collections/${collectionId}/routes`, data);
  },

  /**
   * Get user's collections
   */
  async getUserCollections(
    userId: string,
    pageNumber: number = 1,
    pageSize: number = 20
  ): Promise<PaginatedCollections> {
    const response = await apiClient.get<PaginatedCollections>(
      `/collections/user/${userId}`,
      { params: { pageNumber, pageSize } }
    );
    return response.data;
  },

  /**
   * Get collection by ID with all routes
   */
  async getCollectionById(id: string): Promise<CollectionDetail> {
    const response = await apiClient.get<CollectionDetail>(`/collections/${id}`);
    return response.data;
  },
};

export default collectionService;
