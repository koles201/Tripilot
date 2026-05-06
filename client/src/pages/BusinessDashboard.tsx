import React, { useEffect, useState } from 'react';
import {
  Box,
  Container,
  Card,
  CardContent,
  Typography,
  Button,
  Chip,
  Alert,
  CircularProgress,
  TableContainer,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  IconButton,
  Tooltip,
} from '@mui/material';
import {
  BusinessCenter,
  Place,
  TrendingUp,
  Star,
  Visibility,
  RateReview,
  CheckCircle,
  Pending,
  Cancel,
  Edit,
  Verified,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import businessDashboardService from '../services/api/businessDashboardService';
import type { DashboardData } from '../types/businessDashboard';

const BusinessDashboard: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [dashboardData, setDashboardData] = useState<DashboardData | null>(null);

  useEffect(() => {
    fetchDashboardData();
  }, []);

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      const data = await businessDashboardService.getDashboard();
      setDashboardData(data);
      setError(null);
    } catch (err: any) {
      console.error('Error fetching dashboard data:', err);
      setError(err.response?.data?.message || 'Failed to load dashboard data');
    } finally {
      setLoading(false);
    }
  };

  const getVerificationStatusChip = (status: string) => {
    const statusConfig: Record<string, { color: any; icon: React.ReactElement; label: string }> = {
      Approved: { color: 'success', icon: <CheckCircle />, label: 'Verified' },
      Pending: { color: 'warning', icon: <Pending />, label: 'Pending Review' },
      Rejected: { color: 'error', icon: <Cancel />, label: 'Rejected' },
      NotSubmitted: { color: 'default', icon: <BusinessCenter />, label: 'Not Submitted' },
    };

    const config = statusConfig[status] || statusConfig.NotSubmitted;
    return <Chip icon={config.icon} label={config.label} color={config.color} size="small" />;
  };

  if (loading) {
    return (
      <Container sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '60vh' }}>
        <CircularProgress />
      </Container>
    );
  }

  if (error) {
    return (
      <Container sx={{ mt: 4 }}>
        <Alert severity="error" onClose={() => setError(null)}>
          {error}
        </Alert>
      </Container>
    );
  }

  if (!dashboardData) {
    return (
      <Container sx={{ mt: 4 }}>
        <Alert severity="info">No dashboard data available</Alert>
      </Container>
    );
  }

  const { businessProfile, analytics, claimedPlaces, pendingClaims } = dashboardData;

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      {/* Header */}
      <Box sx={{ mb: 4, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Box>
          <Typography variant="h4" gutterBottom>
            Business Dashboard
          </Typography>
          {businessProfile && (
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
              <Typography variant="h6" color="text.secondary">
                {businessProfile.businessName}
              </Typography>
              {getVerificationStatusChip(businessProfile.verificationStatus)}
              <Chip
                icon={<Visibility />}
                label={`${businessProfile.totalViews} Views`}
                color="info"
                size="small"
                sx={{ ml: 1 }}
              />
            </Box>
          )}
        </Box>
        <Button
          variant="contained"
          startIcon={<Edit />}
          onClick={() => navigate('/business-profile')}
        >
          Edit Profile
        </Button>
      </Box>

      {/* Alert for unverified profiles */}
      {businessProfile?.verificationStatus !== 'Approved' && (
        <Alert severity="warning" sx={{ mb: 3 }}>
          Your business profile needs verification to claim places.{' '}
          <Button size="small" onClick={() => navigate('/business-profile')}>
            Complete Verification
          </Button>
        </Alert>
      )}

      {/* Stats Cards */}
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, 1fr)', md: 'repeat(4, 1fr)' },
          gap: 3,
          mb: 4,
        }}
      >
        <Card>
          <CardContent>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
              <Box>
                <Typography color="text.secondary" gutterBottom variant="overline">
                  Total Places
                </Typography>
                <Typography variant="h4">{analytics.totalPlaces}</Typography>
              </Box>
              <Place color="primary" sx={{ fontSize: 40, opacity: 0.3 }} />
            </Box>
          </CardContent>
        </Card>

        <Card>
          <CardContent>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
              <Box>
                <Typography color="text.secondary" gutterBottom variant="overline">
                  Total Reviews
                </Typography>
                <Typography variant="h4">{analytics.totalReviews}</Typography>
              </Box>
              <RateReview color="secondary" sx={{ fontSize: 40, opacity: 0.3 }} />
            </Box>
          </CardContent>
        </Card>

        <Card>
          <CardContent>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
              <Box>
                <Typography color="text.secondary" gutterBottom variant="overline">
                  Average Rating
                </Typography>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                  <Typography variant="h4">{analytics.averageRating.toFixed(1)}</Typography>
                  <Star color="warning" />
                </Box>
              </Box>
              <TrendingUp color="success" sx={{ fontSize: 40, opacity: 0.3 }} />
            </Box>
          </CardContent>
        </Card>

        <Card>
          <CardContent>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
              <Box>
                <Typography color="text.secondary" gutterBottom variant="overline">
                  Pending Claims
                </Typography>
                <Typography variant="h4">{analytics.pendingClaims}</Typography>
              </Box>
              <Pending color="warning" sx={{ fontSize: 40, opacity: 0.3 }} />
            </Box>
          </CardContent>
        </Card>
      </Box>

      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', lg: '2fr 1fr' },
          gap: 3,
        }}
      >
        {/* Claimed Places */}
        <Card>
            <CardContent>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                <Typography variant="h6">My Places</Typography>
                <Button size="small" onClick={() => navigate('/places')}>
                  View All
                </Button>
              </Box>

              {claimedPlaces.length === 0 ? (
                <Alert severity="info">
                  No claimed places yet.{' '}
                  <Button size="small" onClick={() => navigate('/places')}>
                    Browse Places
                  </Button>
                </Alert>
              ) : (
                <TableContainer>
                  <Table>
                    <TableHead>
                      <TableRow>
                        <TableCell>Place Name</TableCell>
                        <TableCell>Category</TableCell>
                        <TableCell align="center">Status</TableCell>
                        <TableCell align="center">Reviews</TableCell>
                        <TableCell align="center">Rating</TableCell>
                        <TableCell align="right">Actions</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {claimedPlaces.slice(0, 5).map((place) => (
                        <TableRow key={place.id} hover>
                          <TableCell>
                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                              {place.name}
                              {place.isVerified && (
                                <Tooltip title="Verified">
                                  <Verified color="primary" fontSize="small" />
                                </Tooltip>
                              )}
                            </Box>
                          </TableCell>
                          <TableCell>{place.category}</TableCell>
                          <TableCell align="center">
                            <Chip
                              label={place.isActive ? 'Active' : 'Inactive'}
                              color={place.isActive ? 'success' : 'default'}
                              size="small"
                            />
                          </TableCell>
                          <TableCell align="center">{place.reviewCount}</TableCell>
                          <TableCell align="center">
                            {place.averageRating ? (
                              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 0.5 }}>
                                {place.averageRating.toFixed(1)}
                                <Star fontSize="small" color="warning" />
                              </Box>
                            ) : (
                              '-'
                            )}
                          </TableCell>
                          <TableCell align="right">
                            <IconButton
                              size="small"
                              onClick={() => navigate(`/places/${place.id}`)}
                              title="View Details"
                            >
                              <Visibility />
                            </IconButton>
                            <IconButton
                              size="small"
                              onClick={() => navigate(`/places/${place.id}/edit`)}
                              title="Edit"
                            >
                              <Edit />
                            </IconButton>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              )}
            </CardContent>
          </Card>

        {/* Top Places & Pending Claims */}
        <Box>
          {/* Top Places */}
          <Card sx={{ mb: 3 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Top Performing Places
              </Typography>
              {analytics.topPlaces.length === 0 ? (
                <Typography color="text.secondary" variant="body2">
                  No data available yet
                </Typography>
              ) : (
                <Box>
                  {analytics.topPlaces.slice(0, 5).map((place, index) => (
                    <Box
                      key={place.placeId}
                      sx={{
                        display: 'flex',
                        justifyContent: 'space-between',
                        alignItems: 'center',
                        py: 1.5,
                        borderBottom: index < analytics.topPlaces.length - 1 ? '1px solid' : 'none',
                        borderColor: 'divider',
                      }}
                    >
                      <Box>
                        <Typography variant="body2" fontWeight="medium">
                          {place.placeName}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          {place.reviews} reviews
                        </Typography>
                      </Box>
                      {place.averageRating && (
                        <Chip
                          icon={<Star />}
                          label={place.averageRating.toFixed(1)}
                          size="small"
                          color="warning"
                        />
                      )}
                    </Box>
                  ))}
                </Box>
              )}
            </CardContent>
          </Card>

          {/* Pending Claims */}
          {pendingClaims.length > 0 && (
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Pending Claims
                </Typography>
                {pendingClaims.map((claim) => (
                  <Box
                    key={claim.id}
                    sx={{
                      py: 1.5,
                      borderBottom: '1px solid',
                      borderColor: 'divider',
                      '&:last-child': { borderBottom: 'none' },
                    }}
                  >
                    <Typography variant="body2" fontWeight="medium">
                      {claim.placeName}
                    </Typography>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mt: 0.5 }}>
                      <Typography variant="caption" color="text.secondary">
                        Submitted: {new Date(claim.submittedAt).toLocaleDateString()}
                      </Typography>
                      <Chip label={claim.status} size="small" color="warning" />
                    </Box>
                  </Box>
                ))}
              </CardContent>
            </Card>
          )}
        </Box>
      </Box>

      {/* Claims Summary */}
      {(analytics.approvedClaims > 0 || analytics.rejectedClaims > 0) && (
        <Card sx={{ mt: 3 }}>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Claims Summary
            </Typography>
            <Box
              sx={{
                display: 'grid',
                gridTemplateColumns: { xs: '1fr', sm: 'repeat(3, 1fr)' },
                gap: 2,
              }}
            >
              <Box sx={{ textAlign: 'center', p: 2 }}>
                <Typography variant="h4" color="success.main">
                  {analytics.approvedClaims}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Approved
                </Typography>
              </Box>
              <Box sx={{ textAlign: 'center', p: 2 }}>
                <Typography variant="h4" color="warning.main">
                  {analytics.pendingClaims}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Pending
                </Typography>
              </Box>
              <Box sx={{ textAlign: 'center', p: 2 }}>
                <Typography variant="h4" color="error.main">
                  {analytics.rejectedClaims}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Rejected
                </Typography>
              </Box>
            </Box>
          </CardContent>
        </Card>
      )}
    </Container>
  );
};

export default BusinessDashboard;
