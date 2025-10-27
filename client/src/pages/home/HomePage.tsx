import { Container, Typography, Paper, Box, Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { healthService } from '../../services/api/healthService';
import type { HealthStatus } from '../../services/api/healthService';

const HomePage = () => {
  const navigate = useNavigate();
  const [healthStatus, setHealthStatus] = useState<HealthStatus | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const checkHealth = async () => {
      try {
        const status = await healthService.checkHealth();
        setHealthStatus(status);
      } catch (error) {
        console.error('Health check failed:', error);
      } finally {
        setLoading(false);
      }
    };

    checkHealth();
  }, []);

  return (
    <Box sx={{ 
      width: '100%', 
      minHeight: 'calc(100vh - 64px)', // Subtract header height
      display: 'flex', 
      alignItems: 'center',
      py: 4 
    }}>
      <Container maxWidth="lg">
        <Box sx={{ width: '100%' }}>
            <Paper 
              elevation={3} 
              sx={{ 
                p: { xs: 3, md: 6 }, 
                textAlign: 'center',
                borderRadius: 2
              }}
            >
              <Typography 
                variant="h2" 
                component="h1" 
                gutterBottom
                sx={{ 
                  fontSize: { xs: '2rem', sm: '2.5rem', md: '3rem' },
                  fontWeight: 600
                }}
              >
                Welcome to Tripilot
              </Typography>
              
              <Typography 
                variant="h5" 
                color="text.secondary" 
                sx={{ 
                  fontSize: { xs: '1rem', sm: '1.25rem', md: '1.5rem' },
                  mb: 4 
                }}
              >
                Your personal route planning and discovery platform
              </Typography>

              <Box sx={{ mt: 4, mb: 4 }}>
                {loading ? (
                  <Typography>Checking API connection...</Typography>
                ) : healthStatus ? (
                  <Paper 
                    elevation={0}
                    sx={{ 
                      p: 3, 
                      bgcolor: 'success.light', 
                      color: 'success.contrastText',
                      maxWidth: 500,
                      mx: 'auto',
                      borderRadius: 2
                    }}
                  >
                    <Typography variant="h6" gutterBottom>
                      ✓ API Status: {healthStatus.status}
                    </Typography>
                    <Typography variant="body2">Service: {healthStatus.service}</Typography>
                    <Typography variant="body2">Version: {healthStatus.version}</Typography>
                  </Paper>
                ) : (
                  <Paper 
                    elevation={0}
                    sx={{ 
                      p: 3, 
                      bgcolor: 'error.light', 
                      color: 'error.contrastText',
                      maxWidth: 500,
                      mx: 'auto',
                      borderRadius: 2
                    }}
                  >
                    <Typography variant="h6">✗ API Connection Failed</Typography>
                    <Typography variant="body2">Please ensure the backend server is running</Typography>
                  </Paper>
                )}
              </Box>

              <Box 
                sx={{ 
                  mt: 4, 
                  display: 'flex', 
                  gap: 2, 
                  justifyContent: 'center',
                  flexWrap: 'wrap'
                }}
              >
                <Button 
                  variant="contained" 
                  size="large"
                  onClick={() => navigate('/login')}
                  sx={{ minWidth: 150 }}
                >
                  Login
                </Button>
                <Button 
                  variant="outlined" 
                  size="large"
                  onClick={() => navigate('/register')}
                  sx={{ minWidth: 150 }}
                >
                  Register
                </Button>
              </Box>
            </Paper>
        </Box>
      </Container>
    </Box>
  );
};

export default HomePage;
