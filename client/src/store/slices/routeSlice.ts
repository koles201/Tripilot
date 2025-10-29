import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';
import type { RootState } from '../store';
import routeService from '../../services/api/routeService';
import type {
  Route,
  RouteListItem,
  RoutesListResult,
  RouteQueryParams,
  CreateRouteRequest,
  UpdateRouteRequest,
  UpdateRoutePlacesRequest,
  RouteBuilderState,
  RouteDifficultyType,
  RoutePrivacyType,
} from '../../types/route';
import type { PlaceListItem } from '../../types/place';
import { RouteDifficulty, RoutePrivacy } from '../../types/route';

/**
 * Route state interface
 */
interface RouteState {
  // List view
  routes: RouteListItem[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  
  // Detail view
  currentRoute: Route | null;
  
  // Route builder
  builder: RouteBuilderState;
  
  // Loading states
  loading: boolean;
  listLoading: boolean;
  detailLoading: boolean;
  
  // Error state
  error: string | null;
}

/**
 * Initial state
 */
const initialBuilderState: RouteBuilderState = {
  name: '',
  description: '',
  difficulty: RouteDifficulty.Easy,
  privacy: RoutePrivacy.Public,
  selectedPlaces: [],
  isOptimized: false,
};

const initialState: RouteState = {
  routes: [],
  totalCount: 0,
  page: 1,
  pageSize: 12,
  totalPages: 0,
  currentRoute: null,
  builder: initialBuilderState,
  loading: false,
  listLoading: false,
  detailLoading: false,
  error: null,
};

/**
 * Async thunks
 */

export const fetchRoutes = createAsyncThunk<RoutesListResult, RouteQueryParams | undefined>(
  'routes/fetchRoutes',
  async (params) => {
    return await routeService.getRoutes(params);
  }
);

export const fetchUserRoutes = createAsyncThunk<RoutesListResult, RouteQueryParams | undefined>(
  'routes/fetchUserRoutes',
  async (params) => {
    return await routeService.getUserRoutes(params);
  }
);

export const fetchRouteById = createAsyncThunk<Route, string>(
  'routes/fetchRouteById',
  async (id) => {
    return await routeService.getRouteById(id);
  }
);

export const createRoute = createAsyncThunk<Route, CreateRouteRequest>(
  'routes/createRoute',
  async (data) => {
    return await routeService.createRoute(data);
  }
);

export const updateRoute = createAsyncThunk<Route, { id: string; data: UpdateRouteRequest }>(
  'routes/updateRoute',
  async ({ id, data }) => {
    return await routeService.updateRoute(id, data);
  }
);

export const updateRoutePlaces = createAsyncThunk<Route, { id: string; data: UpdateRoutePlacesRequest }>(
  'routes/updateRoutePlaces',
  async ({ id, data }) => {
    return await routeService.updateRoutePlaces(id, data);
  }
);

export const deleteRoute = createAsyncThunk<string, string>(
  'routes/deleteRoute',
  async (id) => {
    await routeService.deleteRoute(id);
    return id;
  }
);

export const duplicateRoute = createAsyncThunk<Route, string>(
  'routes/duplicateRoute',
  async (id) => {
    return await routeService.duplicateRoute(id);
  }
);

export const togglePublishRoute = createAsyncThunk<Route, { id: string; isPublished: boolean }>(
  'routes/togglePublishRoute',
  async ({ id, isPublished }) => {
    return await routeService.togglePublishRoute(id, isPublished);
  }
);

export const optimizeRoute = createAsyncThunk<string[], string[]>(
  'routes/optimizeRoute',
  async (placeIds) => {
    const result = await routeService.getOptimizedRoute(placeIds);
    return result.optimizedOrder;
  }
);

/**
 * Route slice
 */
const routeSlice = createSlice({
  name: 'routes',
  initialState,
  reducers: {
    // Builder actions
    setBuilderName: (state, action: PayloadAction<string>) => {
      state.builder.name = action.payload;
    },
    setBuilderDescription: (state, action: PayloadAction<string>) => {
      state.builder.description = action.payload;
    },
    setBuilderDifficulty: (state, action: PayloadAction<RouteDifficultyType>) => {
      state.builder.difficulty = action.payload;
    },
    setBuilderPrivacy: (state, action: PayloadAction<RoutePrivacyType>) => {
      state.builder.privacy = action.payload;
    },
    addPlaceToBuilder: (state, action: PayloadAction<PlaceListItem>) => {
      const exists = state.builder.selectedPlaces.some(p => p.id === action.payload.id);
      if (!exists) {
        state.builder.selectedPlaces.push(action.payload);
        state.builder.isOptimized = false;
      }
    },
    removePlaceFromBuilder: (state, action: PayloadAction<string>) => {
      state.builder.selectedPlaces = state.builder.selectedPlaces.filter(
        p => p.id !== action.payload
      );
      state.builder.isOptimized = false;
    },
    reorderBuilderPlaces: (state, action: PayloadAction<PlaceListItem[]>) => {
      state.builder.selectedPlaces = action.payload;
      state.builder.isOptimized = false;
    },
    clearBuilder: (state) => {
      state.builder = initialBuilderState;
    },
    initBuilderFromRoute: (state, action: PayloadAction<Route>) => {
      const route = action.payload;
      state.builder = {
        name: route.name,
        description: route.description,
        difficulty: route.difficulty,
        privacy: route.privacy,
        selectedPlaces: route.routePlaces
          .sort((a, b) => a.order - b.order)
          .map(rp => rp.place),
        isOptimized: false,
      };
    },
    clearError: (state) => {
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    // Fetch routes
    builder.addCase(fetchRoutes.pending, (state) => {
      state.listLoading = true;
      state.error = null;
    });
    builder.addCase(fetchRoutes.fulfilled, (state, action) => {
      state.listLoading = false;
      state.routes = action.payload.items;
      state.totalCount = action.payload.totalCount;
      state.page = action.payload.page;
      state.pageSize = action.payload.pageSize;
      state.totalPages = action.payload.totalPages;
    });
    builder.addCase(fetchRoutes.rejected, (state, action) => {
      state.listLoading = false;
      state.error = action.error.message || 'Failed to fetch routes';
    });

    // Fetch user routes
    builder.addCase(fetchUserRoutes.pending, (state) => {
      state.listLoading = true;
      state.error = null;
    });
    builder.addCase(fetchUserRoutes.fulfilled, (state, action) => {
      state.listLoading = false;
      state.routes = action.payload.items;
      state.totalCount = action.payload.totalCount;
      state.page = action.payload.page;
      state.pageSize = action.payload.pageSize;
      state.totalPages = action.payload.totalPages;
    });
    builder.addCase(fetchUserRoutes.rejected, (state, action) => {
      state.listLoading = false;
      state.error = action.error.message || 'Failed to fetch user routes';
    });

    // Fetch route by ID
    builder.addCase(fetchRouteById.pending, (state) => {
      state.detailLoading = true;
      state.error = null;
    });
    builder.addCase(fetchRouteById.fulfilled, (state, action) => {
      state.detailLoading = false;
      state.currentRoute = action.payload;
    });
    builder.addCase(fetchRouteById.rejected, (state, action) => {
      state.detailLoading = false;
      state.error = action.error.message || 'Failed to fetch route';
    });

    // Create route
    builder.addCase(createRoute.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(createRoute.fulfilled, (state, action) => {
      state.loading = false;
      state.currentRoute = action.payload;
      state.builder = initialBuilderState;
    });
    builder.addCase(createRoute.rejected, (state, action) => {
      state.loading = false;
      state.error = action.error.message || 'Failed to create route';
    });

    // Update route
    builder.addCase(updateRoute.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(updateRoute.fulfilled, (state, action) => {
      state.loading = false;
      state.currentRoute = action.payload;
      // Update in list if present
      const index = state.routes.findIndex(r => r.id === action.payload.id);
      if (index !== -1) {
        state.routes[index] = {
          ...state.routes[index],
          name: action.payload.name,
          description: action.payload.description,
          difficulty: action.payload.difficulty,
          privacy: action.payload.privacy,
        };
      }
    });
    builder.addCase(updateRoute.rejected, (state, action) => {
      state.loading = false;
      state.error = action.error.message || 'Failed to update route';
    });

    // Update route places
    builder.addCase(updateRoutePlaces.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(updateRoutePlaces.fulfilled, (state, action) => {
      state.loading = false;
      state.currentRoute = action.payload;
    });
    builder.addCase(updateRoutePlaces.rejected, (state, action) => {
      state.loading = false;
      state.error = action.error.message || 'Failed to update route places';
    });

    // Delete route
    builder.addCase(deleteRoute.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(deleteRoute.fulfilled, (state, action) => {
      state.loading = false;
      state.routes = state.routes.filter(r => r.id !== action.payload);
      if (state.currentRoute?.id === action.payload) {
        state.currentRoute = null;
      }
    });
    builder.addCase(deleteRoute.rejected, (state, action) => {
      state.loading = false;
      state.error = action.error.message || 'Failed to delete route';
    });

    // Duplicate route
    builder.addCase(duplicateRoute.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(duplicateRoute.fulfilled, (state, action) => {
      state.loading = false;
      state.currentRoute = action.payload;
    });
    builder.addCase(duplicateRoute.rejected, (state, action) => {
      state.loading = false;
      state.error = action.error.message || 'Failed to duplicate route';
    });

    // Toggle publish
    builder.addCase(togglePublishRoute.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(togglePublishRoute.fulfilled, (state, action) => {
      state.loading = false;
      state.currentRoute = action.payload;
      // Update in list if present
      const index = state.routes.findIndex(r => r.id === action.payload.id);
      if (index !== -1) {
        state.routes[index] = {
          ...state.routes[index],
          isPublished: action.payload.isPublished,
        };
      }
    });
    builder.addCase(togglePublishRoute.rejected, (state, action) => {
      state.loading = false;
      state.error = action.error.message || 'Failed to toggle publish status';
    });

    // Optimize route
    builder.addCase(optimizeRoute.pending, (state) => {
      state.loading = true;
      state.error = null;
    });
    builder.addCase(optimizeRoute.fulfilled, (state, action) => {
      state.loading = false;
      const optimizedOrder = action.payload;
      const placeMap = new Map(state.builder.selectedPlaces.map(p => [p.id, p]));
      state.builder.selectedPlaces = optimizedOrder
        .map(id => placeMap.get(id))
        .filter((p): p is PlaceListItem => p !== undefined);
      state.builder.isOptimized = true;
    });
    builder.addCase(optimizeRoute.rejected, (state, action) => {
      state.loading = false;
      state.error = action.error.message || 'Failed to optimize route';
    });
  },
});

/**
 * Actions
 */
export const {
  setBuilderName,
  setBuilderDescription,
  setBuilderDifficulty,
  setBuilderPrivacy,
  addPlaceToBuilder,
  removePlaceFromBuilder,
  reorderBuilderPlaces,
  clearBuilder,
  initBuilderFromRoute,
  clearError,
} = routeSlice.actions;

/**
 * Selectors
 */
export const selectRoutes = (state: RootState) => state.routes.routes;
export const selectCurrentRoute = (state: RootState) => state.routes.currentRoute;
export const selectRouteBuilder = (state: RootState) => state.routes.builder;
export const selectRoutePagination = (state: RootState) => ({
  page: state.routes.page,
  pageSize: state.routes.pageSize,
  totalCount: state.routes.totalCount,
  totalPages: state.routes.totalPages,
});
export const selectRouteLoading = (state: RootState) => state.routes.loading;
export const selectRouteListLoading = (state: RootState) => state.routes.listLoading;
export const selectRouteDetailLoading = (state: RootState) => state.routes.detailLoading;
export const selectRouteError = (state: RootState) => state.routes.error;

/**
 * Export reducer
 */
export default routeSlice.reducer;
