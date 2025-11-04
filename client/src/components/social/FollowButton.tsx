import React, { useState } from 'react';
import { Button, CircularProgress } from '@mui/material';
import { PersonAdd as FollowIcon, PersonRemove as UnfollowIcon } from '@mui/icons-material';
import { socialService } from '../../services/api/social/socialService';

interface FollowButtonProps {
  userId: string;
  isFollowing: boolean;
  onFollowChange?: (isFollowing: boolean) => void;
  size?: 'small' | 'medium' | 'large';
  variant?: 'text' | 'outlined' | 'contained';
}

const FollowButton: React.FC<FollowButtonProps> = ({
  userId,
  isFollowing: initialIsFollowing,
  onFollowChange,
  size = 'medium',
  variant = 'contained',
}) => {
  const [isFollowing, setIsFollowing] = useState(initialIsFollowing);
  const [loading, setLoading] = useState(false);

  const handleToggleFollow = async () => {
    setLoading(true);
    try {
      if (isFollowing) {
        await socialService.unfollowUser(userId);
        setIsFollowing(false);
        onFollowChange?.(false);
      } else {
        await socialService.followUser(userId);
        setIsFollowing(true);
        onFollowChange?.(true);
      }
    } catch (error) {
      console.error('Failed to toggle follow:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Button
      variant={variant}
      size={size}
      onClick={handleToggleFollow}
      disabled={loading}
      startIcon={
        loading ? (
          <CircularProgress size={16} />
        ) : isFollowing ? (
          <UnfollowIcon />
        ) : (
          <FollowIcon />
        )
      }
      color={isFollowing ? 'secondary' : 'primary'}
    >
      {isFollowing ? 'Unfollow' : 'Follow'}
    </Button>
  );
};

export default FollowButton;
