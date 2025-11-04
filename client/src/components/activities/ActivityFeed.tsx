import React, { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Avatar,
  Stack,
  CircularProgress,
  Alert,
  Chip,
  Pagination,
} from '@mui/material';
import {
  RouteOutlined as RouteIcon,
  FavoriteOutlined as FavoriteIcon,
  RateReviewOutlined as ReviewIcon,
  CollectionsOutlined as CollectionIcon,
  PersonAddOutlined as FollowIcon,
} from '@mui/icons-material';
import { activityService } from '../../services/api/activities/activityService';
import type { Activity, PaginatedActivities } from '../../types/activity';

interface ActivityFeedProps {
  userId?: string;
  followedOnly?: boolean;
  showGlobal?: boolean;
}

const ActivityFeed: React.FC<ActivityFeedProps> = ({
  userId,
  followedOnly = true,
  showGlobal = false,
}) => {
  const [activities, setActivities] = useState<PaginatedActivities | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);

  useEffect(() => {
    loadActivities();
  }, [userId, followedOnly, showGlobal, page]);

  const loadActivities = async () => {
    setLoading(true);
    setError(null);
    try {
      let data: PaginatedActivities;
      if (showGlobal) {
        data = await activityService.getGlobalFeed({ pageNumber: page });
      } else if (userId) {
        data = await activityService.getUserActivities(userId, { pageNumber: page });
      } else {
        data = await activityService.getActivityFeed({ followedOnly, pageNumber: page });
      }
      setActivities(data);
    } catch (err) {
      console.error('Failed to load activities:', err);
      setError('Failed to load activity feed');
    } finally {
      setLoading(false);
    }
  };

  const getActivityIcon = (activityType: string) => {
    switch (activityType) {
      case 'RouteCreated':
      case 'RouteUpdated':
        return <RouteIcon />;
      case 'RouteFavorited':
        return <FavoriteIcon />;
      case 'ReviewCreated':
        return <ReviewIcon />;
      case 'CollectionCreated':
      case 'RouteAddedToCollection':
        return <CollectionIcon />;
      case 'UserFollowed':
        return <FollowIcon />;
      default:
        return <RouteIcon />;
    }
  };

  const getActivityText = (activity: Activity): string => {
    switch (activity.activityType) {
      case 'RouteCreated':
        return `created a new route "${activity.entityName}"`;
      case 'RouteUpdated':
        return `updated route "${activity.entityName}"`;
      case 'RouteFavorited':
        return `favorited route "${activity.entityName}"`;
      case 'ReviewCreated':
        return `reviewed route "${activity.entityName}"`;
      case 'CollectionCreated':
        return `created a collection "${activity.entityName}"`;
      case 'RouteAddedToCollection':
        return `added route "${activity.entityName}" to a collection`;
      case 'UserFollowed':
        return `started following a user`;
      default:
        return 'performed an activity';
    }
  };

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins} minute${diffMins > 1 ? 's' : ''} ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
    if (diffDays < 7) return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
    return date.toLocaleDateString();
  };

  if (loading && !activities) {
    return (
      <Box display="flex" justifyContent="center" p={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error" sx={{ m: 2 }}>
        {error}
      </Alert>
    );
  }

  if (!activities || activities.items.length === 0) {
    return (
      <Alert severity="info" sx={{ m: 2 }}>
        No activities to display
      </Alert>
    );
  }

  return (
    <Box>
      <Stack spacing={2}>
        {activities.items.map((activity) => (
          <Card key={activity.id} variant="outlined">
            <CardContent>
              <Stack direction="row" spacing={2} alignItems="flex-start">
                <Avatar
                  src={activity.userAvatarUrl}
                  alt={activity.userName}
                  sx={{ width: 40, height: 40 }}
                >
                  {activity.userName[0]}
                </Avatar>
                <Box flex={1}>
                  <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap">
                    <Typography variant="body2">
                      <strong>{activity.userName}</strong> {getActivityText(activity)}
                    </Typography>
                    <Chip
                      icon={getActivityIcon(activity.activityType)}
                      label={activity.activityType.replace(/([A-Z])/g, ' $1').trim()}
                      size="small"
                      variant="outlined"
                    />
                  </Stack>
                  <Typography variant="caption" color="text.secondary">
                    {formatDate(activity.createdAt)}
                  </Typography>
                </Box>
              </Stack>
            </CardContent>
          </Card>
        ))}
      </Stack>

      {activities.totalPages > 1 && (
        <Box display="flex" justifyContent="center" mt={3}>
          <Pagination
            count={activities.totalPages}
            page={page}
            onChange={(_, value) => setPage(value)}
            color="primary"
          />
        </Box>
      )}
    </Box>
  );
};

export default ActivityFeed;
