import { createSlice, createAsyncThunk, type PayloadAction } from '@reduxjs/toolkit';
import type { RootState } from '../store';
import type {
  Place,
  PlaceListItem,
  PlaceQueryParams,
  NearbyPlacesParams,
} from '../../types/place';
import placeService from '../../services/api/placeService';

interface PlaceState {
  places: PlaceListItem[];
  currentPlace: Place | null;
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  filters: PlaceQueryParams;
  loading: boolean;
  error: string | null;
}

const initialState: PlaceState = {
  places: [],
  currentPlace: null,
  totalCount: 0,
  page: 1,
  pageSize: 12,
  totalPages: 0,
  filters: {},
  loading: false,
  error: null,
};

// Async thunks
export const fetchPlaces = createAsyncThunk(
  'place/fetchPlaces',
  async (params: PlaceQueryParams | undefined, { rejectWithValue }) => {
    try {
      return await placeService.getPlaces(params);
    } catch (error: unknown) {
      if (error instanceof Error) {
        return rejectWithValue(error.message);
      }
      return rejectWithValue('An error occurred while fetching places');
    }
  }
);

export const searchPlaces = createAsyncThunk(
  'place/searchPlaces',
  async (params: PlaceQueryParams, { rejectWithValue }) => {
    try {
      return await placeService.searchPlaces(params);
    } catch (error: unknown) {
      if (error instanceof Error) {
        return rejectWithValue(error.message);
      }
      return rejectWithValue('An error occurred while searching places');
    }
  }
);

export const fetchNearbyPlaces = createAsyncThunk(
  'place/fetchNearbyPlaces',
  async (params: NearbyPlacesParams, { rejectWithValue }) => {
    try {
      const places = await placeService.getNearbyPlaces(params);
      return {
        places,
        totalCount: places.length,
        page: 1,
        pageSize: places.length,
        totalPages: 1,
      };
    } catch (error: unknown) {
      if (error instanceof Error) {
        return rejectWithValue(error.message);
      }
      return rejectWithValue('An error occurred while fetching nearby places');
    }
  }
);

export const fetchPlaceById = createAsyncThunk(
  'place/fetchPlaceById',
  async (id: string, { rejectWithValue }) => {
    try {
      return await placeService.getPlaceById(id);
    } catch (error: unknown) {
      if (error instanceof Error) {
        return rejectWithValue(error.message);
      }
      return rejectWithValue('An error occurred while fetching place details');
    }
  }
);

const placeSlice = createSlice({
  name: 'place',
  initialState,
  reducers: {
    setFilters: (state, action: PayloadAction<PlaceQueryParams>) => {
      state.filters = action.payload;
    },
    clearFilters: (state) => {
      state.filters = {};
    },
    setPage: (state, action: PayloadAction<number>) => {
      state.page = action.payload;
    },
    clearCurrentPlace: (state) => {
      state.currentPlace = null;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    // Fetch places
    builder
      .addCase(fetchPlaces.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchPlaces.fulfilled, (state, action) => {
        state.loading = false;
        state.places = action.payload.places;
        state.totalCount = action.payload.totalCount;
        state.page = action.payload.page;
        state.pageSize = action.payload.pageSize;
        state.totalPages = action.payload.totalPages;
      })
      .addCase(fetchPlaces.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });

    // Search places
    builder
      .addCase(searchPlaces.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(searchPlaces.fulfilled, (state, action) => {
        state.loading = false;
        state.places = action.payload.places;
        state.totalCount = action.payload.totalCount;
        state.page = action.payload.page;
        state.pageSize = action.payload.pageSize;
        state.totalPages = action.payload.totalPages;
      })
      .addCase(searchPlaces.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });

    // Fetch nearby places
    builder
      .addCase(fetchNearbyPlaces.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchNearbyPlaces.fulfilled, (state, action) => {
        state.loading = false;
        state.places = action.payload.places;
        state.totalCount = action.payload.totalCount;
        state.page = action.payload.page;
        state.pageSize = action.payload.pageSize;
        state.totalPages = action.payload.totalPages;
      })
      .addCase(fetchNearbyPlaces.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });

    // Fetch place by ID
    builder
      .addCase(fetchPlaceById.pending, (state) => {
        state.loading = true;
        state.error = null;
      })
      .addCase(fetchPlaceById.fulfilled, (state, action) => {
        state.loading = false;
        state.currentPlace = action.payload;
      })
      .addCase(fetchPlaceById.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload as string;
      });
  },
});

export const { setFilters, clearFilters, setPage, clearCurrentPlace, clearError } =
  placeSlice.actions;

// Selectors
export const selectPlaces = (state: RootState) => state.place.places;
export const selectCurrentPlace = (state: RootState) => state.place.currentPlace;
export const selectPlaceLoading = (state: RootState) => state.place.loading;
export const selectPlaceError = (state: RootState) => state.place.error;
export const selectPlaceFilters = (state: RootState) => state.place.filters;
export const selectPagination = (state: RootState) => ({
  page: state.place.page,
  pageSize: state.place.pageSize,
  totalCount: state.place.totalCount,
  totalPages: state.place.totalPages,
});

export default placeSlice.reducer;
