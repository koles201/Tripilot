import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ThemeProvider, CssBaseline, Box } from '@mui/material';
import { useAppSelector } from './hooks/useRedux';
import { lightTheme, darkTheme } from './styles/theme';
import Header from './components/layout/Header';
import HomePage from './pages/home/HomePage';
import { LoginPage, RegisterPage, ForgotPasswordPage, ProfilePage } from './pages/auth';
import { PlaceListPage, PlaceDetailPage } from './pages/places';
import { RouteListPage, RouteDetailPage, RouteBuilderPage } from './pages/routes';
import ProtectedRoute from './components/auth/ProtectedRoute';

function App() {
  const theme = useAppSelector((state) => state.ui.theme);
  const currentTheme = theme === 'light' ? lightTheme : darkTheme;

  return (
    <ThemeProvider theme={currentTheme}>
      <CssBaseline />
      <Router>
        <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh', width: '100%' }}>
          <Header />
          <Box component="main" sx={{ flexGrow: 1, width: '100%' }}>
            <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/forgot-password" element={<ForgotPasswordPage />} />
              
              {/* Place Routes */}
              <Route path="/places" element={<PlaceListPage />} />
              <Route path="/places/:id" element={<PlaceDetailPage />} />
              
              {/* Protected Routes */}
              <Route element={<ProtectedRoute />}>
                <Route path="/profile" element={<ProfilePage />} />
                
                {/* Route Planning Routes */}
                <Route path="/routes" element={<RouteListPage />} />
                <Route path="/routes/new" element={<RouteBuilderPage />} />
                <Route path="/routes/:id" element={<RouteDetailPage />} />
                <Route path="/routes/:id/edit" element={<RouteBuilderPage />} />
              </Route>
            </Routes>
          </Box>
        </Box>
      </Router>
    </ThemeProvider>
  );
}

export default App;

