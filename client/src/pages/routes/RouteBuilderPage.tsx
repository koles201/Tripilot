import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Container,
  Typography,
  Button,
  Paper,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  InputAdornment,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Avatar,
  Checkbox,
  CircularProgress,
  Alert,
  Breadcrumbs,
  Link,
} from '@mui/material';
import {
  Save as SaveIcon,
  Search as SearchIcon,
  ArrowBack as ArrowBackIcon,
} from '@mui/icons-material';
import { useAppDispatch, useAppSelector } from '../../hooks/useRedux';
import {
  createRoute,
  updateRoute,
  fetchRouteById,
  selectRouteBuilder,
  selectRouteLoading,
  selectRouteError,
  addPlaceToBuilder,
  clearBuilder,
  initBuilderFromRoute,
} from '../../store/slices/routeSlice';
import {
  fetchPlaces,
  selectPlaces,
  selectPlaceLoading,
} from '../../store/slices/placeSlice';
import { RouteBuilder } from '../../components/routes/RouteBuilder';
import { RouteMap } from '../../components/routes/RouteMap';
import { ListItemButton } from '@mui/material';

/**
 * Route builder/edit page
 */
export const RouteBuilderPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const isEditMode = Boolean(id);

  const builder = useAppSelector(selectRouteBuilder);
  const loading = useAppSelector(selectRouteLoading);
  const error = useAppSelector(selectRouteError);
  const places = useAppSelector(selectPlaces);
  const placesLoading = useAppSelector(selectPlaceLoading);

  const [placePickerOpen, setPlacePickerOpen] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');

  // Load route for editing
  useEffect(() => {
    if (isEditMode && id) {
      dispatch(fetchRouteById(id)).unwrap().then((route) => {
        dispatch(initBuilderFromRoute(route));
      });
    } else {
      dispatch(clearBuilder());
    }
  }, [id, isEditMode, dispatch]);

  // Load places for selection
  useEffect(() => {
    dispatch(fetchPlaces({ page: 1, pageSize: 50 }));
  }, [dispatch]);

  const handleSave = async () => {
    if (!builder.name.trim()) {
      alert('Please enter a route name');
      return;
    }

    if (builder.selectedPlaces.length < 2) {
      alert('Please add at least 2 places to your route');
      return;
    }

    try {
      if (isEditMode && id) {
        // Update existing route
        await dispatch(
          updateRoute({
            id,
            data: {
              name: builder.name,
              description: builder.description,
              difficulty: builder.difficulty,
              privacy: builder.privacy,
            },
          })
        ).unwrap();
        
        navigate(`/routes/${id}`);
      } else {
        // Create new route
        const result = await dispatch(
          createRoute({
            name: builder.name,
            description: builder.description,
            difficulty: builder.difficulty,
            privacy: builder.privacy,
            placeIds: builder.selectedPlaces.map(p => p.id),
          })
        ).unwrap();
        
        navigate(`/routes/${result.id}`);
      }
    } catch (error) {
      console.error('Failed to save route:', error);
    }
  };

  const handleCancel = () => {
    if (window.confirm('Are you sure? Any unsaved changes will be lost.')) {
      navigate('/routes');
    }
  };

  const handleOpenPlacePicker = () => {
    setPlacePickerOpen(true);
  };

  const handleClosePlacePicker = () => {
    setPlacePickerOpen(false);
    setSearchTerm('');
  };

  const handleTogglePlace = (place: typeof places[0]) => {
    const isSelected = builder.selectedPlaces.some(p => p.id === place.id);
    if (!isSelected) {
      dispatch(addPlaceToBuilder(place));
    }
    handleClosePlacePicker();
  };

  const filteredPlaces = places.filter(
    place =>
      place.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      (place.description && place.description.toLowerCase().includes(searchTerm.toLowerCase())) ||
      (place.city && place.city.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  return (
    <Container maxWidth="xl" sx={{ py: 4 }}>
      {/* Breadcrumbs */}
      <Breadcrumbs sx={{ mb: 2 }}>
        <Link underline="hover" color="inherit" onClick={() => navigate('/routes')} sx={{ cursor: 'pointer' }}>
          My Routes
        </Link>
        <Typography color="text.primary">
          {isEditMode ? 'Edit Route' : 'Create New Route'}
        </Typography>
      </Breadcrumbs>

      {/* Header */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4">
          {isEditMode ? 'Edit Route' : 'Create New Route'}
        </Typography>
        <Box sx={{ display: 'flex', gap: 2 }}>
          <Button
            variant="outlined"
            startIcon={<ArrowBackIcon />}
            onClick={handleCancel}
          >
            Cancel
          </Button>
          <Button
            variant="contained"
            startIcon={<SaveIcon />}
            onClick={handleSave}
            disabled={loading || !builder.name.trim() || builder.selectedPlaces.length < 2}
          >
            {loading ? 'Saving...' : 'Save Route'}
          </Button>
        </Box>
      </Box>

      {/* Error Message */}
      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}

      {/* Two Column Layout */}
      <Box sx={{ display: 'flex', gap: 3, flexDirection: { xs: 'column', md: 'row' } }}>
        {/* Route Builder */}
        <Box sx={{ flex: 1 }}>
          <RouteBuilder onAddPlace={handleOpenPlacePicker} />
        </Box>

        {/* Map Preview */}
        <Box sx={{ flex: 1 }}>
          <Paper sx={{ p: 2, position: 'sticky', top: 80 }}>
            <Typography variant="h6" gutterBottom>
              Route Preview
            </Typography>
            {builder.selectedPlaces.length === 0 ? (
              <Box
                sx={{
                  height: 400,
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  bgcolor: 'grey.100',
                  borderRadius: 1,
                }}
              >
                <Typography color="text.secondary">
                  Add places to see route preview
                </Typography>
              </Box>
            ) : (
              <RouteMap
                places={builder.selectedPlaces}
                height="600px"
                showRoute
                interactive
              />
            )}
          </Paper>
        </Box>
      </Box>

      {/* Place Picker Dialog */}
      <Dialog open={placePickerOpen} onClose={handleClosePlacePicker} maxWidth="md" fullWidth>
        <DialogTitle>Add Place to Route</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            placeholder="Search places..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            sx={{ mb: 2, mt: 1 }}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon />
                </InputAdornment>
              ),
            }}
          />

          {placesLoading ? (
            <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
              <CircularProgress />
            </Box>
          ) : (
            <List sx={{ maxHeight: 400, overflow: 'auto' }}>
              {filteredPlaces.map((place) => {
                const isSelected = builder.selectedPlaces.some(p => p.id === place.id);
                return (
                  <ListItem key={place.id} disablePadding>
                    <ListItemButton
                      onClick={() => !isSelected && handleTogglePlace(place)}
                      disabled={isSelected}
                      sx={{ display: 'flex', alignItems: 'center' }}
                    >
                      <Checkbox checked={isSelected} disabled={isSelected} />
                      <ListItemAvatar>
                        <Avatar src={place.imageUrl} alt={place.name} variant="rounded" />
                      </ListItemAvatar>
                      <ListItemText
                        primary={place.name}
                        secondary={place.city || place.description}
                      />
                    </ListItemButton>
                  </ListItem>
                );
              })}
            </List>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleClosePlacePicker}>Close</Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};

export default RouteBuilderPage;
