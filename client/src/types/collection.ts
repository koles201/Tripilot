// Route collection types

export interface RouteCollection {
  id: string;
  name: string;
  description?: string;
  userId: string;
  userName: string;
  isPublic: boolean;
  coverImageUrl?: string;
  itemCount: number;
  createdAt: string;
  modifiedAt?: string;
}

export interface CollectionItem {
  id: string;
  routeId: string;
  routeName: string;
  routeImageUrl?: string;
  order: number;
  note?: string;
  addedAt: string;
}

export interface CollectionDetail extends RouteCollection {
  items: CollectionItem[];
}

export interface CreateCollectionRequest {
  name: string;
  description?: string;
  isPublic: boolean;
  coverImageUrl?: string;
}

export interface UpdateCollectionRequest {
  name: string;
  description?: string;
  isPublic: boolean;
  coverImageUrl?: string;
}

export interface AddRouteToCollectionRequest {
  collectionId: string;
  routeId: string;
  note?: string;
}

export interface PaginatedCollections {
  items: RouteCollection[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
