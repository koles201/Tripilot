import apiClient from '../apiClient';
import type {
  ShareLinkResponse,
  RouteSocialMetadata,
} from '../../../types/sharing';

/**
 * Service for route sharing features
 */
export const sharingService = {
  /**
   * Generate a shareable link for a route
   */
  async generateShareLink(routeId: string): Promise<ShareLinkResponse> {
    const response = await apiClient.post<ShareLinkResponse>(
      `/routes/sharing/${routeId}/generate-link`
    );
    return response.data;
  },

  /**
   * Get route metadata by share token (for social media previews)
   */
  async getRouteByShareToken(shareToken: string): Promise<RouteSocialMetadata> {
    const response = await apiClient.get<RouteSocialMetadata>(
      `/routes/sharing/token/${shareToken}`
    );
    return response.data;
  },

  /**
   * Copy share link to clipboard
   */
  async copyShareLink(shareUrl: string): Promise<boolean> {
    try {
      await navigator.clipboard.writeText(shareUrl);
      return true;
    } catch (error) {
      console.error('Failed to copy to clipboard:', error);
      return false;
    }
  },

  /**
   * Share route on social media
   */
  shareOnSocialMedia(platform: 'facebook' | 'twitter' | 'linkedin', shareUrl: string, title: string) {
    const encodedUrl = encodeURIComponent(shareUrl);
    const encodedTitle = encodeURIComponent(title);

    let shareLink = '';
    switch (platform) {
      case 'facebook':
        shareLink = `https://www.facebook.com/sharer/sharer.php?u=${encodedUrl}`;
        break;
      case 'twitter':
        shareLink = `https://twitter.com/intent/tweet?url=${encodedUrl}&text=${encodedTitle}`;
        break;
      case 'linkedin':
        shareLink = `https://www.linkedin.com/sharing/share-offsite/?url=${encodedUrl}`;
        break;
    }

    if (shareLink) {
      window.open(shareLink, '_blank', 'width=600,height=400');
    }
  },
};

export default sharingService;
