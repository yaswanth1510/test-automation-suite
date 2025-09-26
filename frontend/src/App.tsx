import React from 'react';
import { Provider } from 'react-redux';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { Box } from '@mui/material';
import { store } from './store/store';
import NavigationBar from './components/Layout/NavigationBar';
import Dashboard from './components/Dashboard/Dashboard';
import { SignalRProvider } from './services/SignalRContext';

const theme = createTheme({
  palette: {
    mode: 'dark',
    primary: {
      main: '#1976d2',
    },
    secondary: {
      main: '#f50057',
    },
    background: {
      default: '#121212',
      paper: '#1e1e1e',
    },
  },
  typography: {
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
    h4: {
      fontWeight: 600,
    },
    h6: {
      fontWeight: 500,
    },
  },
});

function App() {
  return (
    <Provider store={store}>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <SignalRProvider>
          <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
            <NavigationBar />
            <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
              <Dashboard />
            </Box>
          </Box>
        </SignalRProvider>
      </ThemeProvider>
    </Provider>
  );
}

export default App;
