import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ThemeProvider, CssBaseline, Box } from '@mui/material';
import { useAppSelector } from './hooks/useRedux';
import { lightTheme, darkTheme } from './styles/theme';
import Header from './components/layout/Header';
import HomePage from './pages/home/HomePage';

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
              <Route path="/login" element={<div>Login Page (To be implemented)</div>} />
              <Route path="/register" element={<div>Register Page (To be implemented)</div>} />
            </Routes>
          </Box>
        </Box>
      </Router>
    </ThemeProvider>
  );
}

export default App;
