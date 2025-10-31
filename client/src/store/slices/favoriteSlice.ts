import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import type { RootState } from '../store';
import type {
  Favorite,
  FavoriteQueryParams,
  FavoriteStatus,
} from '../../types/favorite';
import favoriteService from '../../services/api/favoriteService';

interface FavoriteState {
  favorites: Favorite[];
  favoriteStatuses: Record<string, FavoriteStatus>; // placeId -> status
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  loading: boolean;
  error: string | null;
}

const initialState: FavoriteState = {
  favorites: [],
  favoriteStatuses: {},
  totalCount: 0,
  page: 1,
  pageSize: 20,
  totalPages: 0,
  loading: false,
  error: null,
};

// Async thunks
export const fetchFavorites = createAsyncThunk(
  'favorite/fetchFavorites',
  async (params: FavoriteQueryParams | undefined, { rejectWithValue }) => {
    try {
      return await favoriteService.getFavorites(params);
    } catch (error: unknown) {
      if (error instanceof Error) {
        return rejectWithValue(error.message);
      }
      return rejectWithValue('An error occurred while fetching favorites');
    }
  }
);

export const addFavorite = createAsyncThunk(
  'favorite/addFavorite',
  async (placeId: string, { rejectWithValue }) => {
    try {
      return await favoriteService.addFavorite(placeId);
    } catch (error: unknown) {
      if (error instanceof Error) {
        return rejectWithValue(error.message);
      }
      return rejectWithValue('An error occurred while adding favorite');
    }
  }
);

export const removeFavorite = createAsyncThunk(
  'favorite/removeFavorite',
  async (placeId: string, { rejectWithValue }) => {
    try {
      await favoriteService.removeFavorite(placeId);
      return placeId;
    } catch (error: unknown) {
      if (error instanceof Error) {
        return rejectWithValue(error.message);
      }
      return rejectWithValue('An error occurred while removing favorite');
    }
  }
);

export const checkFavoriteStatus = createAsyncThunk(
  'favorite/checkStatus',
  async (placeId: string, { rejectWithValue }) => {
    try {
      const result = await favoriteService.isFavorite(placeId);
      return { placeId, ...result };
    } catch (error: unknown) {
      if (error instanceof Error) {
        return rejectWithValue(error.message);
      }
      return rejectWithValue('An error occurred while checking favorite status');
    }
  }
);

export const getFavoritesCount = createAsyncThunk(
  'favorite/getCount',
  async (_, { rejectWithValue }) => {
    try {
      return await favoriteService.getFavoritesCount();
    } catch (error: unknown) {
      if (error instanceof Error) {
        return rejectWithValue(error.message);
      }
      return rejectWithValue('An error occurred while fetching favorites count');
    }
  }
);

const favoriteSlice = createSlice({
  name: 'favorite',
  initialState,
  reducers: {
    // Optimistic update for adding favorite
    optimisticAddFavorite: (state, action: PayloadAction<{ placeId: string }>) => {
      state.favoriteStatuses[action.payload.placeId] = {
        placeId: action.payload.placeId,
        isFavorite: true,
      };
    },
    // Optimistic update for removing favorite
    optimisticRemoveFavorite: (state, action: PayloadAction<{ placeId: string }>) => {
      state.favoriteStatuses[action.payload.placeId] = {
        placeId: action.payload.placeId,
        isFavorite: false,
      };
      // Remove from favorites list
      state.favorites = state.favorites.filter(f => f.placeId !== action.payload.placeId);
    },
    // Set favorite status for a place
    setFavoriteStatus: (state, action: PayloadAction<FavoriteStatus>) => {
      state.favoriteStatuses[action.payload.placeId] = action.payload;
    },
    // Clear error
    clearError: (state) => {
      state.error = null;
    },
    // Reset favorites state
    resetFavorites: (state) => {
      state.favorites = [];
      state.totalCount = 0;
      state.page = 1;
      state.totalPages = 0;
    },
  },
  extraReducers: (builder) => {
    // Fetch favorites
    builder
      .addCase(fetchFavorites.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchFavorites.fulfilled, (state, action) => {
        state.loading = false;
        state.favorites = action.payload.favorites;
        state.totalCount = action.payload.totalCount;
        state.page = action.payload.page;
        state.pageSize = action.payload.pageSize;
        state.totalPages = action.payload.totalPages;
        
        // Update favorite statuses
        action.payload.favorites.forEach(fav => {
          state.favoriteStatuses[fav.placeId] = {
            placeId: fav.placeId,
            isFavorite: true,
            favoriteId: fav.id,
          };
        });
      })
      .addCase(fetchFavorites.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });

    // Add favorite
    builder
      .addCase(addFavorite.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(addFavorite.fulfilled, (state, action) => {
        state.loading = false;
        // Add to favorites list
        state.favorites.unshift(action.payload);
        state.totalCount += 1;
        // Update status
        state.favoriteStatuses[action.payload.placeId] = {
          placeId: action.payload.placeId,
          isFavorite: true,
          favoriteId: action.payload.id,
        };
      })
      .addCase(addFavorite.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
        // Revert optimistic update on error
        const errorPayload = action.meta.arg;
        if (state.favoriteStatuses[errorPayload]) {
          state.favoriteStatuses[errorPayload].isFavorite = false;
        }
      });

    // Remove favorite
    builder
      .addCase(removeFavorite.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(removeFavorite.fulfilled, (state, action) => {
        state.loading = false;
        const placeId = action.payload;
        // Remove from favorites list
        state.favorites = state.favorites.filter(f => f.placeId !== placeId);
        state.totalCount = Math.max(0, state.totalCount - 1);
        // Update status
        state.favoriteStatuses[placeId] = {
          placeId,
          isFavorite: false,
        };
      })
      .addCase(removeFavorite.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
        // Revert optimistic update on error
        const errorPayload = action.meta.arg;
        if (state.favoriteStatuses[errorPayload]) {
          state.favoriteStatuses[errorPayload].isFavorite = true;
        }
      });

    // Check favorite status
    builder
      .addCase(checkFavoriteStatus.fulfilled, (state, action) => {
        state.favoriteStatuses[action.payload.placeId] = {
          placeId: action.payload.placeId,
          isFavorite: action.payload.isFavorite,
          favoriteId: action.payload.favoriteId,
        };
      });

    // Get favorites count
    builder
      .addCase(getFavoritesCount.fulfilled, (state, action) => {
        state.totalCount = action.payload;
      });
  },
});

export const {
  optimisticAddFavorite,
  optimisticRemoveFavorite,
  setFavoriteStatus,
  clearError,
  resetFavorites,
} = favoriteSlice.actions;

// Selectors
export const selectFavorites = (state: RootState) => state.favorite.favorites;
export const selectFavoriteLoading = (state: RootState) => state.favorite.loading;
export const selectFavoriteError = (state: RootState) => state.favorite.error;
export const selectFavoritePagination = (state: RootState) => ({
  page: state.favorite.page,
  pageSize: state.favorite.pageSize,
  totalCount: state.favorite.totalCount,
  totalPages: state.favorite.totalPages,
});
export const selectFavoriteStatuses = (state: RootState) => state.favorite.favoriteStatuses;
export const selectIsFavorite = (placeId: string) => (state: RootState) =>
  state.favorite.favoriteStatuses[placeId]?.isFavorite || false;
export const selectFavoritesCount = (state: RootState) => state.favorite.totalCount;

export default favoriteSlice.reducer;
