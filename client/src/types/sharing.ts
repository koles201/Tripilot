// Route sharing types

export interface ShareLinkResponse {
  shareToken: string;
  shareUrl: string;
  embedCode?: string;
}

export interface RouteSocialMetadata {
  title: string;
  description: string;
  imageUrl?: string;
  url: string;
  creatorName: string;
  averageRating: number;
  viewCount: number;
  favoriteCount: number;
}

export interface GenerateShareLinkRequest {
  routeId: string;
}

export interface GetEmbedCodeRequest {
  identifier: string;
  width?: number;
  height?: number;
}
