import React, { useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Container,
  Typography,
  Button,
  IconButton,
  Chip,
  Paper,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Avatar,
  Skeleton,
  Alert,
  Breadcrumbs,
  Link,
  Divider,
  Card,
  CardContent,
} from '@mui/material';
// Note: MUI v7 Grid2 import unavailable in current setup; using responsive Box layouts instead.
import {
  Edit as EditIcon,
  Delete as DeleteIcon,
  ContentCopy as ContentCopyIcon,
  Share as ShareIcon,
  ArrowBack as ArrowBackIcon,
  Public as PublicIcon,
  Lock as LockIcon,
  Terrain as TerrainIcon,
  Person as PersonIcon,
  Visibility as VisibilityIcon,
  Star as StarIcon,
  Place as PlaceIcon,
  Timeline as TimelineIcon,
  Schedule as ScheduleIcon,
} from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../hooks/useRedux';
import {
  fetchRouteById,
  deleteRoute,
  duplicateRoute,
  selectCurrentRoute,
  selectRouteDetailLoading,
  selectRouteError,
} from '../../store/slices/routeSlice';
import {
  RouteDifficultyNames,
  RoutePrivacyNames,
  RouteDifficulty,
  type RouteDifficultyType,
} from '../../types/route';
import { RouteMap } from '../../components/routes/RouteMap';

/**
 * Route detail page
 */
export const RouteDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  
  const route = useAppSelector(selectCurrentRoute);
  const loading = useAppSelector(selectRouteDetailLoading);
  const error = useAppSelector(selectRouteError);

  useEffect(() => {
    if (id) {
      dispatch(fetchRouteById(id));
    }
  }, [id, dispatch]);

  const handleEdit = () => {
    navigate(`/routes/${id}/edit`);
  };

  const handleDelete = async () => {
    if (window.confirm('Are you sure you want to delete this route?')) {
      try {
        await dispatch(deleteRoute(id!)).unwrap();
        navigate('/routes');
      } catch (error) {
        console.error('Failed to delete route:', error);
      }
    }
  };

  const handleDuplicate = async () => {
    try {
      const duplicated = await dispatch(duplicateRoute(id!)).unwrap();
      navigate(`/routes/${duplicated.id}`);
    } catch (error) {
      console.error('Failed to duplicate route:', error);
    }
  };

  const handleShare = () => {
    // TODO: Implement share functionality
    const url = window.location.href;
    navigator.clipboard.writeText(url);
    alert('Route link copied to clipboard!');
  };

  const handleBack = () => {
    navigate('/routes');
  };

  const getDifficultyColor = (difficulty: RouteDifficultyType) => {
    switch (difficulty) {
      case RouteDifficulty.Easy:
        return 'success';
      case RouteDifficulty.Moderate:
        return 'info';
      case RouteDifficulty.Challenging:
        return 'warning';
      case RouteDifficulty.Difficult:
        return 'error';
      default:
        return 'default';
    }
  };

  if (loading) {
    return (
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <Skeleton variant="text" width={200} height={40} sx={{ mb: 2 }} />
        <Skeleton variant="rectangular" height={400} sx={{ mb: 3 }} />
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 3 }}>
          <Box sx={{ flexGrow: 1, flexBasis: { xs: '100%', md: '66%' } }}>
            <Skeleton variant="rectangular" height={300} />
          </Box>
          <Box sx={{ flexGrow: 1, flexBasis: { xs: '100%', md: '32%' } }}>
            <Skeleton variant="rectangular" height={300} />
          </Box>
        </Box>
      </Container>
    );
  }

  if (error || !route) {
    return (
      <Container maxWidth="xl" sx={{ py: 4 }}>
        <Alert severity="error" sx={{ mb: 3 }}>
          {error || 'Route not found'}
        </Alert>
        <Button variant="outlined" startIcon={<ArrowBackIcon />} onClick={handleBack}>
          Back to Routes
        </Button>
      </Container>
    );
  }

  const places = route.routePlaces
    .sort((a, b) => a.order - b.order)
    .map(rp => rp.place);

  return (
    <Container maxWidth="xl" sx={{ py: 4 }}>
      {/* Breadcrumbs */}
      <Breadcrumbs sx={{ mb: 2 }}>
        <Link underline="hover" color="inherit" onClick={handleBack} sx={{ cursor: 'pointer' }}>
          My Routes
        </Link>
        <Typography color="text.primary">{route.name}</Typography>
      </Breadcrumbs>

      {/* Header */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 3 }}>
        <Box sx={{ flex: 1 }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 1 }}>
            <Typography variant="h4">{route.name}</Typography>
            <Chip
              icon={<TerrainIcon />}
              label={RouteDifficultyNames[route.difficulty]}
              color={getDifficultyColor(route.difficulty)}
            />
            <Chip
              icon={route.privacy === 1 ? <PublicIcon /> : <LockIcon />}
              label={RoutePrivacyNames[route.privacy]}
              variant="outlined"
            />
            {!route.isPublished && (
              <Chip label="Draft" color="warning" variant="outlined" />
            )}
          </Box>
          <Typography variant="body1" color="text.secondary" paragraph>
            {route.description}
          </Typography>
        </Box>

        <Box sx={{ display: 'flex', gap: 1 }}>
          <IconButton onClick={handleBack} color="primary">
            <ArrowBackIcon />
          </IconButton>
          <IconButton onClick={handleEdit} color="primary">
            <EditIcon />
          </IconButton>
          <IconButton onClick={handleDuplicate} color="primary">
            <ContentCopyIcon />
          </IconButton>
          <IconButton onClick={handleShare} color="primary">
            <ShareIcon />
          </IconButton>
          <IconButton onClick={handleDelete} color="error">
            <DeleteIcon />
          </IconButton>
        </Box>
      </Box>

      {/* Stats Cards */}
      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2, mb: 3 }}>
        <Box sx={{ flexBasis: { xs: '100%', sm: '48%', md: '23%' }, flexGrow: 1 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                <PlaceIcon color="primary" />
                <Typography variant="h6">{route.routePlaces.length}</Typography>
              </Box>
              <Typography variant="body2" color="text.secondary">
                Places
              </Typography>
            </CardContent>
          </Card>
        </Box>
        {route.totalDistance && (
          <Box sx={{ flexBasis: { xs: '100%', sm: '48%', md: '23%' }, flexGrow: 1 }}>
            <Card>
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                  <TimelineIcon color="primary" />
                  <Typography variant="h6">{route.totalDistance.toFixed(1)} km</Typography>
                </Box>
                <Typography variant="body2" color="text.secondary">
                  Total Distance
                </Typography>
              </CardContent>
            </Card>
          </Box>
        )}
        {route.estimatedDuration && (
          <Box sx={{ flexBasis: { xs: '100%', sm: '48%', md: '23%' }, flexGrow: 1 }}>
            <Card>
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                  <ScheduleIcon color="primary" />
                  <Typography variant="h6">{route.estimatedDuration} min</Typography>
                </Box>
                <Typography variant="body2" color="text.secondary">
                  Estimated Duration
                </Typography>
              </CardContent>
            </Card>
          </Box>
        )}
        <Box sx={{ flexBasis: { xs: '100%', sm: '48%', md: '23%' }, flexGrow: 1 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1 }}>
                <VisibilityIcon color="primary" />
                <Typography variant="h6">{route.viewCount}</Typography>
              </Box>
              <Typography variant="body2" color="text.secondary">
                Views
              </Typography>
            </CardContent>
          </Card>
        </Box>
      </Box>

      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 3 }}>
        {/* Map */}
        <Box sx={{ flexBasis: { xs: '100%', md: '66%' }, flexGrow: 1 }}>
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>
              Route Map
            </Typography>
            <Divider sx={{ mb: 2 }} />
            <RouteMap places={places} height="500px" showRoute interactive />
          </Paper>
        </Box>

        {/* Route Info and Places */}
        <Box sx={{ flexBasis: { xs: '100%', md: '32%' }, flexGrow: 1 }}>
          {/* Author Info */}
          <Paper sx={{ p: 2, mb: 2 }}>
            <Typography variant="h6" gutterBottom>
              Route Information
            </Typography>
            <Divider sx={{ mb: 2 }} />
            
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
              <PersonIcon color="action" />
              <Box>
                <Typography variant="caption" color="text.secondary" display="block">
                  Created by
                </Typography>
                <Typography variant="body2" fontWeight={500}>
                  {route.userName || 'Unknown'}
                </Typography>
              </Box>
            </Box>

            {route.rating && (
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
                <StarIcon color="warning" />
                <Box>
                  <Typography variant="caption" color="text.secondary" display="block">
                    Rating
                  </Typography>
                  <Typography variant="body2" fontWeight={500}>
                    {route.rating.toFixed(1)} ({route.reviewCount} reviews)
                  </Typography>
                </Box>
              </Box>
            )}

            <Box>
              <Typography variant="caption" color="text.secondary" display="block">
                Created on
              </Typography>
              <Typography variant="body2" fontWeight={500}>
                {new Date(route.createdAt).toLocaleDateString()}
              </Typography>
            </Box>
          </Paper>

          {/* Places List */}
          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>
              Places on Route ({route.routePlaces.length})
            </Typography>
            <Divider sx={{ mb: 2 }} />

            <List>
              {route.routePlaces
                .sort((a, b) => a.order - b.order)
                .map((routePlace, index) => (
                  <React.Fragment key={routePlace.id}>
                    {index > 0 && <Divider />}
                    <ListItem
                      sx={{
                        cursor: 'pointer',
                        '&:hover': { bgcolor: 'action.hover' },
                      }}
                      onClick={() => navigate(`/places/${routePlace.placeId}`)}
                    >
                      <Chip
                        label={index + 1}
                        size="small"
                        color="primary"
                        sx={{ mr: 2, fontWeight: 'bold' }}
                      />
                      <ListItemAvatar>
                        <Avatar
                          src={routePlace.place.photoUrl || routePlace.place.coverImageUrl}
                          alt={routePlace.place.name}
                          variant="rounded"
                        />
                      </ListItemAvatar>
                      <ListItemText
                        primary={routePlace.place.name}
                        secondary={
                          <>
                            {routePlace.place.address || routePlace.place.city}
                            {routePlace.duration && (
                              <Typography variant="caption" display="block">
                                Duration: {routePlace.duration} min
                              </Typography>
                            )}
                            {routePlace.notes && (
                              <Typography variant="caption" display="block">
                                {routePlace.notes}
                              </Typography>
                            )}
                          </>
                        }
                        primaryTypographyProps={{ fontWeight: 500 }}
                      />
                    </ListItem>
                  </React.Fragment>
                ))}
            </List>
          </Paper>
        </Box>
      </Box>

      {/* Action Buttons */}
      <Box sx={{ display: 'flex', justifyContent: 'center', gap: 2, mt: 4 }}>
        <Button variant="outlined" size="large" onClick={handleBack}>
          Back to Routes
        </Button>
        <Button variant="contained" size="large" startIcon={<EditIcon />} onClick={handleEdit}>
          Edit Route
        </Button>
      </Box>
    </Container>
  );
};

export default RouteDetailPage;
