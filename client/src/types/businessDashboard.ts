/**
 * Business Dashboard Types
 * Type definitions for business dashboard data structures
 */

export interface BusinessProfile {
  id: string;
  businessName: string;
  verificationStatus: 'Pending' | 'Approved' | 'Rejected' | 'NotSubmitted';
  claimedPlacesCount: number;
  totalViews: number;
}

export interface TopPlace {
  placeId: string;
  placeName: string;
  reviews: number;
  averageRating: number | null;
}

export interface BusinessAnalytics {
  totalPlaces: number;
  totalReviews: number;
  averageRating: number;
  pendingClaims: number;
  approvedClaims: number;
  rejectedClaims: number;
  topPlaces: TopPlace[];
}

export interface PlaceOverview {
  id: string;
  name: string;
  category: string;
  isVerified: boolean;
  isActive: boolean;
  reviewCount: number;
  averageRating: number | null;
  claimedAt: string;
}

export interface PlaceClaim {
  id: string;
  placeName: string;
  status: string;
  submittedAt: string;
}

export interface DashboardData {
  businessProfile: BusinessProfile | null;
  analytics: BusinessAnalytics;
  claimedPlaces: PlaceOverview[];
  pendingClaims: PlaceClaim[];
}

export interface AnalyticsQueryParams {
  startDate?: string;
  endDate?: string;
}
