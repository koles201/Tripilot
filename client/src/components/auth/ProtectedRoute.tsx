import { Navigate, Outlet } from 'react-router-dom';
import { useAppSelector } from '../../hooks/useRedux';
import { Box, CircularProgress } from '@mui/material';

interface ProtectedRouteProps {
  requiredRole?: string;
  redirectPath?: string;
}

export default function ProtectedRoute({
  requiredRole,
  redirectPath = '/login',
}: ProtectedRouteProps) {
  const { isAuthenticated, loading, user } = useAppSelector((state) => state.auth);

  if (loading) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          minHeight: '100vh',
        }}
      >
        <CircularProgress />
      </Box>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to={redirectPath} replace />;
  }

  if (requiredRole && user?.role !== requiredRole) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}
