import { createTheme } from '@mui/material/styles';
import type {} from '@mui/x-data-grid/themeAugmentation';

export const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: '#1E3A5F',
      light: '#3D5A80',
      dark: '#122840',
      contrastText: '#FFFFFF',
    },
    secondary: {
      main: '#C77B2C',
      light: '#DA9A54',
      dark: '#9C5E1C',
      contrastText: '#FFFFFF',
    },
    background: {
      default: '#F3F1EC',
      paper: '#FFFFFF',
    },
    text: {
      primary: '#1A2130',
      secondary: '#5B6472',
    },
    divider: 'rgba(26,33,48,0.09)',
  },
  shape: { borderRadius: 10 },
  typography: {
    fontFamily: '"Inter", "Segoe UI", Roboto, sans-serif',
    h1: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 700 },
    h2: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 700 },
    h3: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 700 },
    h4: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 700, letterSpacing: '-0.02em' },
    h5: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 700, letterSpacing: '-0.01em' },
    h6: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 700 },
    subtitle1: { fontWeight: 600 },
    subtitle2: { fontWeight: 600 },
    button: { textTransform: 'none', fontWeight: 600 },
  },
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        body: { backgroundColor: '#F3F1EC' },
      },
    },
    MuiAppBar: {
      styleOverrides: {
        root: { boxShadow: 'none' },
        colorPrimary: { backgroundColor: '#1E3A5F' },
      },
    },
    MuiDrawer: {
      styleOverrides: {
        paper: { borderRight: '1px solid rgba(26,33,48,0.08)', backgroundColor: '#FFFFFF' },
      },
    },
    MuiCard: {
      styleOverrides: {
        root: {
          borderRadius: 14,
          border: '1px solid rgba(26,33,48,0.08)',
        },
      },
    },
    MuiButton: {
      styleOverrides: {
        root: { borderRadius: 8, paddingInline: 18 },
        contained: {
          boxShadow: 'none',
          '&:hover': { boxShadow: '0 6px 16px rgba(30,58,95,0.28)' },
        },
      },
    },
    MuiChip: {
      styleOverrides: {
        root: { fontWeight: 600, borderRadius: 6 },
      },
    },
    MuiPaper: {
      styleOverrides: {
        root: { backgroundImage: 'none' },
        outlined: { borderColor: 'rgba(26,33,48,0.08)' },
      },
    },
    MuiDataGrid: {
      styleOverrides: {
        root: {
          border: 'none',
          borderRadius: 12,
          backgroundColor: '#FFFFFF',
        },
        columnHeaders: {
          backgroundColor: '#F1EEE7',
        },
        columnHeaderTitle: {
          fontWeight: 700,
        },
      },
    },
  },
});
