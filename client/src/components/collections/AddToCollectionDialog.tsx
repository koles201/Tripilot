import React, { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  TextField,
  CircularProgress,
  Alert,
  Box,
  Divider,
  Typography,
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { collectionService } from '../../services/api/collections/collectionService';
import type { RouteCollection } from '../../types/collection';

interface AddToCollectionDialogProps {
  open: boolean;
  onClose: () => void;
  routeId: string;
  routeName: string;
}

const AddToCollectionDialog: React.FC<AddToCollectionDialogProps> = ({
  open,
  onClose,
  routeId,
  routeName,
}) => {
  const [collections, setCollections] = useState<RouteCollection[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showCreateNew, setShowCreateNew] = useState(false);
  const [newCollectionName, setNewCollectionName] = useState('');
  const [creating, setCreating] = useState(false);

  useEffect(() => {
    if (open) {
      loadCollections();
    }
  }, [open]);

  const loadCollections = async () => {
    setLoading(true);
    setError(null);
    try {
      // This would need to be adjusted to get current user's collections
      // For now, we'll use a placeholder
      const data = await collectionService.getMyCollections();
      setCollections(data.items);
    } catch (err) {
      console.error('Failed to load collections:', err);
      setError('Failed to load collections');
    } finally {
      setLoading(false);
    }
  };

  const handleAddToCollection = async (collectionId: string) => {
    try {
      await collectionService.addRouteToCollection(collectionId, {
        collectionId,
        routeId,
      });
      onClose();
    } catch (err) {
      console.error('Failed to add route to collection:', err);
      setError('Failed to add route to collection');
    }
  };

  const handleCreateCollection = async () => {
    if (!newCollectionName.trim()) return;

    setCreating(true);
    try {
      const collection = await collectionService.createCollection({
        name: newCollectionName,
        isPublic: true,
      });
      await handleAddToCollection(collection.id);
    } catch (err) {
      console.error('Failed to create collection:', err);
      setError('Failed to create collection');
    } finally {
      setCreating(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add "{routeName}" to Collection</DialogTitle>
      <DialogContent>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        {loading ? (
          <Box display="flex" justifyContent="center" p={3}>
            <CircularProgress />
          </Box>
        ) : (
          <>
            <List>
              {collections.map((collection) => (
                <ListItem key={collection.id} disablePadding>
                  <ListItemButton onClick={() => handleAddToCollection(collection.id)}>
                    <ListItemText
                      primary={collection.name}
                      secondary={`${collection.itemCount} routes`}
                    />
                  </ListItemButton>
                </ListItem>
              ))}
            </List>

            <Divider sx={{ my: 2 }} />

            {!showCreateNew ? (
              <Button
                startIcon={<AddIcon />}
                onClick={() => setShowCreateNew(true)}
                fullWidth
                variant="outlined"
              >
                Create New Collection
              </Button>
            ) : (
              <Box>
                <Typography variant="subtitle2" gutterBottom>
                  New Collection
                </Typography>
                <TextField
                  fullWidth
                  size="small"
                  placeholder="Collection name"
                  value={newCollectionName}
                  onChange={(e) => setNewCollectionName(e.target.value)}
                  disabled={creating}
                  autoFocus
                  sx={{ mb: 1 }}
                />
                <Box display="flex" gap={1}>
                  <Button
                    onClick={handleCreateCollection}
                    disabled={!newCollectionName.trim() || creating}
                    variant="contained"
                    size="small"
                  >
                    {creating ? <CircularProgress size={20} /> : 'Create & Add'}
                  </Button>
                  <Button
                    onClick={() => {
                      setShowCreateNew(false);
                      setNewCollectionName('');
                    }}
                    disabled={creating}
                    size="small"
                  >
                    Cancel
                  </Button>
                </Box>
              </Box>
            )}
          </>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
      </DialogActions>
    </Dialog>
  );
};

export default AddToCollectionDialog;
