import { createSlice } from '@reduxjs/toolkit';
import type { PayloadAction } from '@reduxjs/toolkit';

interface UIState {
  sidebarOpen: boolean;
  theme: 'light' | 'dark';
  loading: boolean;
  notification: {
    open: boolean;
    message: string;
    severity: 'success' | 'error' | 'warning' | 'info';
  } | null;
}

const initialState: UIState = {
  sidebarOpen: true,
  theme: 'light',
  loading: false,
  notification: null,
};

const uiSlice = createSlice({
  name: 'ui',
  initialState,
  reducers: {
    toggleSidebar: (state) => {
      state.sidebarOpen = !state.sidebarOpen;
    },
    setTheme: (state, action: PayloadAction<'light' | 'dark'>) => {
      state.theme = action.payload;
    },
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
    },
    showNotification: (state, action: PayloadAction<Omit<NonNullable<UIState['notification']>, 'open'>>) => {
      state.notification = {
        ...action.payload,
        open: true,
      };
    },
    hideNotification: (state) => {
      if (state.notification) {
        state.notification.open = false;
      }
    },
  },
});

export const { toggleSidebar, setTheme, setLoading, showNotification, hideNotification } = uiSlice.actions;
export default uiSlice.reducer;
