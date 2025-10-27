# Tripilot Client Application

React + TypeScript frontend for the Tripilot tourist route planning platform.

## Technology Stack

- **React 18** - UI library
- **TypeScript** - Type-safe JavaScript
- **Vite** - Fast build tool and dev server
- **Material-UI (MUI)** - Component library and design system
- **Redux Toolkit** - State management
- **React Router** - Client-side routing
- **Axios** - HTTP client for API requests
- **Emotion** - CSS-in-JS styling

## Project Structure

```
client/
 src/
    components/          # Reusable UI components
       common/          # Generic components
       layout/          # Layout components (Header, Footer, etc.)
       auth/            # Authentication components
    pages/               # Page components
       home/            # Home page
       auth/            # Auth pages (Login, Register)
    services/            # API and service layers
       api/             # API client and service functions
    store/               # Redux store configuration
       slices/          # Redux slices
    hooks/               # Custom React hooks
    utils/               # Utility functions
    types/               # TypeScript type definitions
    styles/              # Theme and global styles
 public/                  # Static assets
 package.json
```

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- Backend API running on http://localhost:5139

### Install Dependencies

```bash
npm install
```

### Development Server

```bash
npm run dev
```

The application will start on http://localhost:5173

### Build for Production

```bash
npm run build
```

### Preview Production Build

```bash
npm run preview
```

## Features Implemented

###  Project Setup
- React + TypeScript with Vite
- Material-UI theme configuration (light/dark modes)
- Redux Toolkit store setup
- React Router navigation
- Axios API client with interceptors

###  Components
- **Header** - Navigation bar with theme toggle and auth status
- **HomePage** - Landing page with API health check

###  State Management
- **Auth Slice** - User authentication state
- **UI Slice** - Theme, sidebar, and notification state
- Typed Redux hooks for TypeScript

###  Services
- **API Client** - Axios instance with auth interceptors
- **Health Service** - API health check methods

## Environment Variables

Create a `.env` file in the root directory:

```env
VITE_API_URL=http://localhost:5139/api
```

## API Integration

The client is configured to connect to the backend API at:
- Base URL: `http://localhost:5139/api`
- Health check: `GET /health`

The Axios client automatically:
- Adds JWT token to requests from localStorage
- Redirects to login on 401 unauthorized
- Handles error responses

## State Management

### Redux Store Structure

```typescript
{
  auth: {
    user: User | null,
    token: string | null,
    isAuthenticated: boolean,
    loading: boolean,
    error: string | null
  },
  ui: {
    sidebarOpen: boolean,
    theme: 'light' | 'dark',
    loading: boolean,
    notification: {...} | null
  }
}
```

### Using Redux in Components

```typescript
import { useAppDispatch, useAppSelector } from '../hooks/useRedux';
import { logout } from '../store/slices/authSlice';

const MyComponent = () => {
  const dispatch = useAppDispatch();
  const { isAuthenticated } = useAppSelector((state) => state.auth);
  
  const handleLogout = () => {
    dispatch(logout());
  };
};
```

## Styling

Material-UI themes are configured in `src/styles/theme.ts`:
- Light theme with blue primary color
- Dark theme with appropriate contrasts
- Custom button styles (no text transform, rounded corners)

## Next Steps

- [ ] Implement Login page
- [ ] Implement Registration page
- [ ] Add form validation
- [ ] Create protected routes
- [ ] Add loading indicators and error handling
- [ ] Implement place listing components
- [ ] Create route planning interface
- [ ] Add map integration
- [ ] Implement review system UI

## Development Guidelines

### Component Structure

```typescript
import { FC } from 'react';

interface MyComponentProps {
  title: string;
  onAction: () => void;
}

const MyComponent: FC<MyComponentProps> = ({ title, onAction }) => {
  return (
    // Component JSX
  );
};

export default MyComponent;
```

### API Service Pattern

```typescript
import apiClient from './apiClient';

export const myService = {
  getData: async () => {
    const response = await apiClient.get('/endpoint');
    return response.data;
  },
  
  postData: async (data: MyType) => {
    const response = await apiClient.post('/endpoint', data);
    return response.data;
  },
};
```

## Code Style

- Use TypeScript for all new files
- Follow React functional component patterns
- Use hooks instead of class components
- Apply proper typing for props and state
- Use async/await for asynchronous operations
- Follow Material-UI styling conventions

## Support

For issues or questions, contact the development team.

## License

Proprietary - All rights reserved
