import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Container,
  Typography,
  Card,
  CardContent,
  CardActions,
  CardMedia,
  Button,
  IconButton,
  Chip,
  Menu,
  MenuItem,
  TextField,
  Select,
  FormControl,
  InputLabel,
  Paper,
  Skeleton,
  Alert,
  Pagination,
  InputAdornment,
} from '@mui/material';
// Grid2 import unavailable; using responsive flex Box layout instead.
import {
  Add as AddIcon,
  MoreVert as MoreVertIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  ContentCopy as ContentCopyIcon,
  Search as SearchIcon,
  FilterList as FilterListIcon,
  Public as PublicIcon,
  Lock as LockIcon,
  Terrain as TerrainIcon,
  Place as PlaceIcon,
  Timeline as TimelineIcon,
} from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../hooks/useRedux';
import {
  fetchUserRoutes,
  deleteRoute,
  duplicateRoute,
  selectRoutes,
  selectRouteListLoading,
  selectRouteError,
  selectRoutePagination,
} from '../../store/slices/routeSlice';
import {
  RouteDifficultyNames,
  RoutePrivacyNames,
  RouteDifficulty,
  RoutePrivacy,
  type RouteListItem,
  type RouteDifficultyType,
  type RoutePrivacyType,
} from '../../types/route';

/**
 * Route card component
 */
interface RouteCardProps {
  route: RouteListItem;
  onEdit: (id: string) => void;
  onDelete: (id: string) => void;
  onDuplicate: (id: string) => void;
  onView: (id: string) => void;
}

const RouteCard: React.FC<RouteCardProps> = ({ route, onEdit, onDelete, onDuplicate, onView }) => {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    event.stopPropagation();
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
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

  return (
    <Card
      sx={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        cursor: 'pointer',
        transition: 'transform 0.2s, box-shadow 0.2s',
        '&:hover': {
          transform: 'translateY(-4px)',
          boxShadow: 4,
        },
      }}
      onClick={() => onView(route.id)}
    >
      <CardMedia
        component="div"
        sx={{
          height: 160,
          bgcolor: 'grey.300',
          position: 'relative',
          backgroundImage: route.thumbnailUrl ? `url(${route.thumbnailUrl})` : 'none',
          backgroundSize: 'cover',
          backgroundPosition: 'center',
        }}
      >
        {!route.thumbnailUrl && (
          <Box
            sx={{
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              height: '100%',
            }}
          >
            <TimelineIcon sx={{ fontSize: 60, color: 'grey.500' }} />
          </Box>
        )}
        <Box sx={{ position: 'absolute', top: 8, right: 8, display: 'flex', gap: 0.5 }}>
          <Chip
            icon={route.privacy === RoutePrivacy.Public ? <PublicIcon /> : <LockIcon />}
            label={RoutePrivacyNames[route.privacy]}
            size="small"
            sx={{ bgcolor: 'rgba(255, 255, 255, 0.9)' }}
          />
        </Box>
        <Box sx={{ position: 'absolute', bottom: 8, left: 8 }}>
          <Chip
            icon={<TerrainIcon />}
            label={RouteDifficultyNames[route.difficulty]}
            size="small"
            color={getDifficultyColor(route.difficulty)}
            sx={{ fontWeight: 'bold' }}
          />
        </Box>
      </CardMedia>

      <CardContent sx={{ flexGrow: 1, pb: 1 }}>
        <Typography variant="h6" gutterBottom noWrap>
          {route.name}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2, height: 40, overflow: 'hidden' }}>
          {route.description || 'No description'}
        </Typography>

        <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
            <PlaceIcon fontSize="small" color="action" />
            <Typography variant="caption">{route.placeCount} places</Typography>
          </Box>
          {route.totalDistance && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <TimelineIcon fontSize="small" color="action" />
              <Typography variant="caption">{route.totalDistance.toFixed(1)} km</Typography>
            </Box>
          )}
          {route.rating && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
              <Typography variant="caption">⭐ {route.rating.toFixed(1)}</Typography>
              <Typography variant="caption" color="text.secondary">
                ({route.reviewCount})
              </Typography>
            </Box>
          )}
        </Box>
      </CardContent>

      <CardActions sx={{ justifyContent: 'space-between', pt: 0 }}>
        <Button size="small" onClick={(e) => { e.stopPropagation(); onView(route.id); }}>
          View Details
        </Button>
        <IconButton size="small" onClick={handleMenuOpen}>
          <MoreVertIcon />
        </IconButton>
        <Menu anchorEl={anchorEl} open={Boolean(anchorEl)} onClose={handleMenuClose}>
          <MenuItem
            onClick={(e) => {
              e.stopPropagation();
              handleMenuClose();
              onEdit(route.id);
            }}
          >
            <EditIcon fontSize="small" sx={{ mr: 1 }} />
            Edit
          </MenuItem>
          <MenuItem
            onClick={(e) => {
              e.stopPropagation();
              handleMenuClose();
              onDuplicate(route.id);
            }}
          >
            <ContentCopyIcon fontSize="small" sx={{ mr: 1 }} />
            Duplicate
          </MenuItem>
          <MenuItem
            onClick={(e) => {
              e.stopPropagation();
              handleMenuClose();
              onDelete(route.id);
            }}
            sx={{ color: 'error.main' }}
          >
            <DeleteIcon fontSize="small" sx={{ mr: 1 }} />
            Delete
          </MenuItem>
        </Menu>
      </CardActions>
    </Card>
  );
};

/**
 * Route list page
 */
export const RouteListPage: React.FC = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const routes = useAppSelector(selectRoutes);
  const loading = useAppSelector(selectRouteListLoading);
  const error = useAppSelector(selectRouteError);
  const pagination = useAppSelector(selectRoutePagination);

  const [searchTerm, setSearchTerm] = useState('');
  const [difficulty, setDifficulty] = useState<RouteDifficultyType | ''>('');
  const [privacy, setPrivacy] = useState<RoutePrivacyType | ''>('');
  const [sortBy, setSortBy] = useState<'rating' | 'distance' | 'duration' | 'newest'>('newest');

  useEffect(() => {
    loadRoutes();
  }, [pagination.page, difficulty, privacy, sortBy]);

  const loadRoutes = () => {
    dispatch(
      fetchUserRoutes({
        page: pagination.page,
        pageSize: pagination.pageSize,
        difficulty: difficulty || undefined,
        privacy: privacy || undefined,
        sortBy,
      })
    );
  };

  const handleSearch = () => {
    // TODO: Implement search when backend supports it
    loadRoutes();
  };

  const handlePageChange = (_event: React.ChangeEvent<unknown>, value: number) => {
    dispatch(fetchUserRoutes({ page: value, pageSize: pagination.pageSize }));
  };

  const handleEdit = (id: string) => {
    navigate(`/routes/${id}/edit`);
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('Are you sure you want to delete this route?')) {
      try {
        await dispatch(deleteRoute(id)).unwrap();
        loadRoutes();
      } catch (error) {
        console.error('Failed to delete route:', error);
      }
    }
  };

  const handleDuplicate = async (id: string) => {
    try {
      const duplicated = await dispatch(duplicateRoute(id)).unwrap();
      navigate(`/routes/${duplicated.id}`);
    } catch (error) {
      console.error('Failed to duplicate route:', error);
    }
  };

  const handleView = (id: string) => {
    navigate(`/routes/${id}`);
  };

  const handleCreateNew = () => {
    navigate('/routes/new');
  };

  return (
    <Container maxWidth="xl" sx={{ py: 4 }}>
      {/* Header */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 4 }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            My Routes
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Manage and explore your custom travel routes
          </Typography>
        </Box>
        <Button
          variant="contained"
          size="large"
          startIcon={<AddIcon />}
          onClick={handleCreateNew}
        >
          Create New Route
        </Button>
      </Box>

      {/* Filters */}
      <Paper sx={{ p: 2, mb: 3 }}>
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
          <Box sx={{ flexBasis: { xs: '100%', md: '32%' }, flexGrow: 1 }}>
            <TextField
              fullWidth
              placeholder="Search routes..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                ),
              }}
            />
          </Box>
          <Box sx={{ flexBasis: { xs: '100%', sm: '48%', md: '15%' }, flexGrow: 1 }}>
            <FormControl fullWidth>
              <InputLabel>Difficulty</InputLabel>
              <Select
                value={difficulty}
                onChange={(e) => setDifficulty(e.target.value as RouteDifficultyType | '')}
                label="Difficulty"
              >
                <MenuItem value="">All</MenuItem>
                {Object.values(RouteDifficulty).map((value) => (
                  <MenuItem key={value} value={value}>
                    {RouteDifficultyNames[value as RouteDifficultyType]}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Box>
          <Box sx={{ flexBasis: { xs: '100%', sm: '48%', md: '15%' }, flexGrow: 1 }}>
            <FormControl fullWidth>
              <InputLabel>Privacy</InputLabel>
              <Select
                value={privacy}
                onChange={(e) => setPrivacy(e.target.value as RoutePrivacyType | '')}
                label="Privacy"
              >
                <MenuItem value="">All</MenuItem>
                {Object.values(RoutePrivacy).map((value) => (
                  <MenuItem key={value} value={value}>
                    {RoutePrivacyNames[value as RoutePrivacyType]}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Box>
          <Box sx={{ flexBasis: { xs: '100%', sm: '48%', md: '15%' }, flexGrow: 1 }}>
            <FormControl fullWidth>
              <InputLabel>Sort By</InputLabel>
              <Select
                value={sortBy}
                onChange={(e) => setSortBy(e.target.value as typeof sortBy)}
                label="Sort By"
              >
                <MenuItem value="newest">Newest</MenuItem>
                <MenuItem value="rating">Highest Rated</MenuItem>
                <MenuItem value="distance">Distance</MenuItem>
                <MenuItem value="duration">Duration</MenuItem>
              </Select>
            </FormControl>
          </Box>
          <Box sx={{ flexBasis: { xs: '100%', sm: '48%', md: '15%' }, flexGrow: 1 }}>
            <Button
              fullWidth
              variant="outlined"
              startIcon={<FilterListIcon />}
              onClick={loadRoutes}
            >
              Apply Filters
            </Button>
          </Box>
        </Box>
      </Paper>

      {/* Error Message */}
      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      {/* Routes Grid */}
      {loading ? (
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 3 }}>
          {[...Array(6)].map((_, index) => (
            <Box key={index} sx={{ flexBasis: { xs: '100%', sm: '48%', md: '31%' }, flexGrow: 1 }}>
              <Card>
                <Skeleton variant="rectangular" height={160} />
                <CardContent>
                  <Skeleton variant="text" height={32} />
                  <Skeleton variant="text" />
                  <Skeleton variant="text" width="60%" />
                </CardContent>
              </Card>
            </Box>
          ))}
        </Box>
      ) : routes.length === 0 ? (
        <Paper
          sx={{
            p: 8,
            textAlign: 'center',
            bgcolor: 'grey.50',
          }}
        >
          <TimelineIcon sx={{ fontSize: 80, color: 'grey.400', mb: 2 }} />
          <Typography variant="h5" gutterBottom>
            No routes found
          </Typography>
          <Typography variant="body1" color="text.secondary" paragraph>
            Start creating your custom travel routes to explore new places
          </Typography>
          <Button variant="contained" size="large" startIcon={<AddIcon />} onClick={handleCreateNew}>
            Create Your First Route
          </Button>
        </Paper>
      ) : (
        <>
    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 3 }}>
            {routes.map((route) => (
              <Box key={route.id} sx={{ flexBasis: { xs: '100%', sm: '48%', md: '31%' }, flexGrow: 1 }}>
                <RouteCard
                  route={route}
                  onEdit={handleEdit}
                  onDelete={handleDelete}
                  onDuplicate={handleDuplicate}
                  onView={handleView}
                />
              </Box>
            ))}
          </Box>

          {/* Pagination */}
          {pagination.totalPages > 1 && (
            <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
              <Pagination
                count={pagination.totalPages}
                page={pagination.page}
                onChange={handlePageChange}
                color="primary"
                size="large"
              />
            </Box>
          )}
        </>
      )}
    </Container>
  );
};

export default RouteListPage;
