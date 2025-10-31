import { AppBar, Toolbar, Typography, Button, IconButton, Box, Menu, MenuItem } from '@mui/material';
import { Brightness4, Brightness7, Menu as MenuIcon, AccountCircle, Explore, Timeline, BusinessCenter, Favorite } from '@mui/icons-material';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import { useState } from 'react';
import { useAppDispatch, useAppSelector } from '../../hooks/useRedux';
import { toggleSidebar, setTheme } from '../../store/slices/uiSlice';
import { logout } from '../../store/slices/authSlice';

const Header = () => {
  const navigate = useNavigate();
  const dispatch = useAppDispatch();
  const { theme } = useAppSelector((state) => state.ui);
  const { isAuthenticated, user } = useAppSelector((state) => state.auth);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [mobileMenuAnchor, setMobileMenuAnchor] = useState<null | HTMLElement>(null);

  const handleThemeToggle = () => {
    dispatch(setTheme(theme === 'light' ? 'dark' : 'light'));
  };

  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
  };

  const handleMobileMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
    setMobileMenuAnchor(event.currentTarget);
  };

  const handleMobileMenuClose = () => {
    setMobileMenuAnchor(null);
  };

  const handleProfileClick = () => {
    navigate('/profile');
    handleMenuClose();
  };

  const handleMobileNavigation = (path: string) => {
    navigate(path);
    handleMobileMenuClose();
  };

  return (
    <AppBar position="static">
      <Toolbar>
        {/* Mobile menu toggle - only show on mobile when authenticated */}
        {isAuthenticated && (
          <IconButton
            edge="start"
            color="inherit"
            aria-label="navigation menu"
            onClick={handleMobileMenuOpen}
            sx={{ mr: 2, display: { xs: 'flex', md: 'none' } }}
          >
            <MenuIcon />
          </IconButton>
        )}
        
        {/* Sidebar toggle - only show on desktop */}
        <IconButton
          edge="start"
          color="inherit"
          aria-label="toggle sidebar"
          onClick={() => dispatch(toggleSidebar())}
          sx={{ mr: 2, display: { xs: 'none', md: 'flex' } }}
        >
          <MenuIcon />
        </IconButton>
        
        <Typography
          variant="h6"
          component={RouterLink}
          to="/"
          sx={{
            textDecoration: 'none',
            color: 'inherit',
            cursor: 'pointer',
            mr: 4
          }}
        >
          Tripilot
        </Typography>

        {/* Desktop Navigation Links */}
        <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' }, gap: 2 }}>
          <Button
            color="inherit"
            component={RouterLink}
            to="/places"
            startIcon={<Explore />}
          >
            Discover
          </Button>
          {isAuthenticated && (
            <>
              <Button
                color="inherit"
                component={RouterLink}
                to="/favorites"
                startIcon={<Favorite />}
              >
                Favorites
              </Button>
              <Button
                color="inherit"
                component={RouterLink}
                to="/routes"
                startIcon={<Timeline />}
              >
                My Routes
              </Button>
              <Button
                color="inherit"
                component={RouterLink}
                to="/business-dashboard"
                startIcon={<BusinessCenter />}
              >
                Business
              </Button>
            </>
          )}
        </Box>

        {/* Mobile Navigation Menu */}
        <Menu
          id="mobile-menu"
          anchorEl={mobileMenuAnchor}
          anchorOrigin={{
            vertical: 'bottom',
            horizontal: 'left',
          }}
          keepMounted
          transformOrigin={{
            vertical: 'top',
            horizontal: 'left',
          }}
          open={Boolean(mobileMenuAnchor)}
          onClose={handleMobileMenuClose}
        >
          <MenuItem onClick={() => handleMobileNavigation('/places')}>
            <Explore sx={{ mr: 2 }} />
            Discover
          </MenuItem>
          {isAuthenticated && (
            <>
              <MenuItem onClick={() => handleMobileNavigation('/favorites')}>
                <Favorite sx={{ mr: 2 }} />
                Favorites
              </MenuItem>
              <MenuItem onClick={() => handleMobileNavigation('/routes')}>
                <Timeline sx={{ mr: 2 }} />
                My Routes
              </MenuItem>
              <MenuItem onClick={() => handleMobileNavigation('/business-dashboard')}>
                <BusinessCenter sx={{ mr: 2 }} />
                Business Dashboard
              </MenuItem>
            </>
          )}
        </Menu>

        {/* Spacer for mobile */}
        <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none' } }} />

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <IconButton color="inherit" onClick={handleThemeToggle}>
            {theme === 'dark' ? <Brightness7 /> : <Brightness4 />}
          </IconButton>
          
          {isAuthenticated ? (
            <>
              <IconButton
                color="inherit"
                onClick={handleMenuOpen}
                aria-label="account"
                aria-controls="menu-appbar"
                aria-haspopup="true"
              >
                <AccountCircle />
              </IconButton>
              <Menu
                id="menu-appbar"
                anchorEl={anchorEl}
                anchorOrigin={{
                  vertical: 'bottom',
                  horizontal: 'right',
                }}
                keepMounted
                transformOrigin={{
                  vertical: 'top',
                  horizontal: 'right',
                }}
                open={Boolean(anchorEl)}
                onClose={handleMenuClose}
              >
                <MenuItem disabled>
                  <Typography variant="body2">{user?.email}</Typography>
                </MenuItem>
                <MenuItem onClick={handleProfileClick}>Profile</MenuItem>
                <MenuItem onClick={() => { navigate('/business-dashboard'); handleMenuClose(); }}>
                  Business Dashboard
                </MenuItem>
                <MenuItem onClick={handleLogout}>Logout</MenuItem>
              </Menu>
            </>
          ) : (
            <>
              <Button color="inherit" component={RouterLink} to="/login">
                Login
              </Button>
              <Button color="inherit" component={RouterLink} to="/register" variant="outlined">
                Sign Up
              </Button>
            </>
          )}
        </Box>
      </Toolbar>
    </AppBar>
  );
};

export default Header;

