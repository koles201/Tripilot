import React, { useState } from 'react';
import {
  Box,
  Paper,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Button,
  Typography,
  IconButton,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Avatar,
  Divider,
  Alert,
  Chip,
} from '@mui/material';
import {
  DragHandle as DragHandleIcon,
  Delete as DeleteIcon,
  Add as AddIcon,
  Lightbulb as LightbulbIcon,
} from '@mui/icons-material';
import {
  DndContext,
  closestCenter,
  KeyboardSensor,
  PointerSensor,
  useSensor,
  useSensors,
} from '@dnd-kit/core';
import type { DragEndEvent } from '@dnd-kit/core';
import {
  arrayMove,
  SortableContext,
  sortableKeyboardCoordinates,
  useSortable,
  verticalListSortingStrategy,
} from '@dnd-kit/sortable';
import { CSS } from '@dnd-kit/utilities';
import { useAppDispatch, useAppSelector } from '../../hooks/useRedux';
import {
  selectRouteBuilder,
  setBuilderName,
  setBuilderDescription,
  setBuilderDifficulty,
  setBuilderPrivacy,
  removePlaceFromBuilder,
  reorderBuilderPlaces,
  optimizeRoute,
} from '../../store/slices/routeSlice';
import {
  RouteDifficulty,
  RouteDifficultyNames,
  RoutePrivacy,
  RoutePrivacyNames,
  type RouteDifficultyType,
  type RoutePrivacyType,
} from '../../types/route';
import type { PlaceListItem } from '../../types/place';

interface RouteBuilderProps {
  onAddPlace?: () => void;
  readOnly?: boolean;
}

/**
 * Sortable place item component
 */
interface SortablePlaceItemProps {
  place: PlaceListItem;
  index: number;
  onRemove: (id: string) => void;
  readOnly?: boolean;
}

const SortablePlaceItem: React.FC<SortablePlaceItemProps> = ({
  place,
  index,
  onRemove,
  readOnly,
}) => {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } = useSortable({
    id: place.id,
  });

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.5 : 1,
  };

  return (
    <ListItem
      ref={setNodeRef}
      style={style}
      sx={{
        bgcolor: 'background.paper',
        mb: 1,
        borderRadius: 1,
        border: '1px solid',
        borderColor: 'divider',
      }}
      secondaryAction={
        !readOnly && (
          <IconButton edge="end" onClick={() => onRemove(place.id)} size="small">
            <DeleteIcon />
          </IconButton>
        )
      }
    >
      {!readOnly && (
        <IconButton
          {...attributes}
          {...listeners}
          sx={{ cursor: 'grab', mr: 1 }}
          size="small"
        >
          <DragHandleIcon />
        </IconButton>
      )}
      <Chip
        label={index + 1}
        size="small"
        color="primary"
        sx={{ mr: 2, fontWeight: 'bold' }}
      />
      <ListItemAvatar>
        <Avatar
          src={place.photoUrl || place.coverImageUrl}
          alt={place.name}
          variant="rounded"
        />
      </ListItemAvatar>
      <ListItemText
        primary={place.name}
        secondary={place.address || place.city}
        primaryTypographyProps={{ fontWeight: 500 }}
      />
    </ListItem>
  );
};

/**
 * Route builder component with drag-and-drop
 */
export const RouteBuilder: React.FC<RouteBuilderProps> = ({ onAddPlace, readOnly = false }) => {
  const dispatch = useAppDispatch();
  const builder = useAppSelector(selectRouteBuilder);
  const [isOptimizing, setIsOptimizing] = useState(false);

  const sensors = useSensors(
    useSensor(PointerSensor),
    useSensor(KeyboardSensor, {
      coordinateGetter: sortableKeyboardCoordinates,
    })
  );

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event;

    if (over && active.id !== over.id) {
      const oldIndex = builder.selectedPlaces.findIndex((p) => p.id === active.id);
      const newIndex = builder.selectedPlaces.findIndex((p) => p.id === over.id);

      if (oldIndex !== -1 && newIndex !== -1) {
        const reordered = arrayMove(builder.selectedPlaces, oldIndex, newIndex);
        dispatch(reorderBuilderPlaces(reordered));
      }
    }
  };

  const handleRemovePlace = (placeId: string) => {
    dispatch(removePlaceFromBuilder(placeId));
  };

  const handleOptimizeRoute = async () => {
    if (builder.selectedPlaces.length < 2) return;
    
    setIsOptimizing(true);
    try {
      await dispatch(optimizeRoute(builder.selectedPlaces.map(p => p.id))).unwrap();
    } catch (error) {
      console.error('Failed to optimize route:', error);
    } finally {
      setIsOptimizing(false);
    }
  };

  const getDifficultyOptions = () => {
    return Object.values(RouteDifficulty).map((value) => ({
      value,
      label: RouteDifficultyNames[value as RouteDifficultyType],
    }));
  };

  const getPrivacyOptions = () => {
    return Object.values(RoutePrivacy).map((value) => ({
      value,
      label: RoutePrivacyNames[value as RoutePrivacyType],
    }));
  };

  return (
    <Box>
      {/* Route Information */}
      <Paper sx={{ p: 3, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Route Information
        </Typography>
        <Divider sx={{ mb: 2 }} />

        <TextField
          fullWidth
          label="Route Name"
          value={builder.name}
          onChange={(e) => dispatch(setBuilderName(e.target.value))}
          disabled={readOnly}
          required
          sx={{ mb: 2 }}
        />

        <TextField
          fullWidth
          label="Description"
          value={builder.description}
          onChange={(e) => dispatch(setBuilderDescription(e.target.value))}
          disabled={readOnly}
          multiline
          rows={3}
          sx={{ mb: 2 }}
        />

        <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
          <FormControl fullWidth>
            <InputLabel>Difficulty</InputLabel>
            <Select
              value={builder.difficulty}
              onChange={(e) =>
                dispatch(setBuilderDifficulty(e.target.value as RouteDifficultyType))
              }
              disabled={readOnly}
              label="Difficulty"
            >
              {getDifficultyOptions().map((option) => (
                <MenuItem key={option.value} value={option.value}>
                  {option.label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          <FormControl fullWidth>
            <InputLabel>Privacy</InputLabel>
            <Select
              value={builder.privacy}
              onChange={(e) =>
                dispatch(setBuilderPrivacy(e.target.value as RoutePrivacyType))
              }
              disabled={readOnly}
              label="Privacy"
            >
              {getPrivacyOptions().map((option) => (
                <MenuItem key={option.value} value={option.value}>
                  {option.label}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Box>
      </Paper>

      {/* Places List */}
      <Paper sx={{ p: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6">
            Places ({builder.selectedPlaces.length})
          </Typography>
          {!readOnly && (
            <Box sx={{ display: 'flex', gap: 1 }}>
              {builder.selectedPlaces.length >= 2 && (
                <Button
                  variant="outlined"
                  startIcon={<LightbulbIcon />}
                  onClick={handleOptimizeRoute}
                  disabled={isOptimizing}
                  size="small"
                >
                  {isOptimizing ? 'Optimizing...' : 'Optimize Route'}
                </Button>
              )}
              <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={onAddPlace}
                size="small"
              >
                Add Place
              </Button>
            </Box>
          )}
        </Box>

        <Divider sx={{ mb: 2 }} />

        {builder.isOptimized && (
          <Alert severity="success" sx={{ mb: 2 }}>
            Route has been optimized for minimum distance!
          </Alert>
        )}

        {builder.selectedPlaces.length === 0 ? (
          <Box
            sx={{
              textAlign: 'center',
              py: 6,
              color: 'text.secondary',
            }}
          >
            <Typography variant="body1" gutterBottom>
              No places added yet
            </Typography>
            <Typography variant="body2">
              Click "Add Place" to start building your route
            </Typography>
          </Box>
        ) : (
          <DndContext
            sensors={sensors}
            collisionDetection={closestCenter}
            onDragEnd={handleDragEnd}
          >
            <SortableContext
              items={builder.selectedPlaces.map((p) => p.id)}
              strategy={verticalListSortingStrategy}
            >
              <List>
                {builder.selectedPlaces.map((place, index) => (
                  <SortablePlaceItem
                    key={place.id}
                    place={place}
                    index={index}
                    onRemove={handleRemovePlace}
                    readOnly={readOnly}
                  />
                ))}
              </List>
            </SortableContext>
          </DndContext>
        )}

        {builder.selectedPlaces.length > 0 && (
          <Alert severity="info" sx={{ mt: 2 }}>
            <Typography variant="body2" fontWeight={500}>
              Tips:
            </Typography>
            <Typography variant="body2">
              • Drag places to reorder them
              {builder.selectedPlaces.length >= 2 && ' • Use "Optimize Route" for the best order'}
              {builder.selectedPlaces.length < 2 && ' • Add at least 2 places to optimize'}
            </Typography>
          </Alert>
        )}
      </Paper>
    </Box>
  );
};

export default RouteBuilder;
