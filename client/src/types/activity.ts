// Activity feed types

export interface Activity {
  id: string;
  userId: string;
  userName: string;
  userAvatarUrl?: string;
  activityType: string;
  entityId?: string;
  entityType?: string;
  entityName?: string;
  metadata?: string;
  createdAt: string;
}

export interface PaginatedActivities {
  items: Activity[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface GetActivityFeedRequest {
  followedOnly?: boolean;
  activityType?: string;
  pageNumber?: number;
  pageSize?: number;
}
