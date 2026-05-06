// Social features types

export interface UserFollow {
  id: string;
  followerId: string;
  followingId: string;
  createdAt: string;
}

export interface UserProfile {
  id: string;
  fullName: string;
  email: string;
  bio?: string;
  avatarUrl?: string;
  followerCount: number;
  followingCount: number;
  isFollowing: boolean;
  createdAt: string;
}

export interface UserListItem {
  id: string;
  fullName: string;
  bio?: string;
  avatarUrl?: string;
  isFollowing: boolean;
  followedAt: string;
}

export interface FollowUserRequest {
  userId: string;
}

export interface PaginatedUserList {
  items: UserListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
