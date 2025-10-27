import apiClient from './apiClient';

export interface HealthStatus {
  status: string;
  timestamp: string;
  service: string;
  version: string;
}

export const healthService = {
  checkHealth: async (): Promise<HealthStatus> => {
    const response = await apiClient.get('/health');
    return response.data;
  },

  checkDetailedHealth: async (): Promise<any> => {
    const response = await apiClient.get('/health/detailed');
    return response.data;
  },
};
