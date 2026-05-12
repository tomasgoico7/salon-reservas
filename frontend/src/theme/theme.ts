import { createTheme, alpha } from '@mui/material/styles';

const PURPLE = '#5C35BF';

export const theme = createTheme({
  palette: {
    mode: 'light',
    primary: {
      main: PURPLE,
      light: '#7C5CBF',
      dark: '#3D1F8F',
      contrastText: '#ffffff',
    },
    secondary: {
      main: '#E8562A',
      light: '#FF7D54',
      dark: '#B53D1A',
    },
    success: {
      main: '#2A7A4B',
      light: '#3DAB6A',
    },
    background: {
      default: '#EEEAF6',
      paper: '#FFFFFF',
    },
    text: {
      primary: '#18112E',
      secondary: '#5A5070',
    },
    divider: alpha(PURPLE, 0.1),
  },
  typography: {
    fontFamily: '"Inter","Segoe UI","Roboto","Helvetica","Arial",sans-serif',
    h4: { fontWeight: 700, letterSpacing: '-0.5px' },
    h5: { fontWeight: 600, letterSpacing: '-0.3px' },
    h6: { fontWeight: 600 },
    subtitle2: { fontWeight: 600, fontSize: '0.75rem', letterSpacing: '0.06em', textTransform: 'uppercase' },
    button: { textTransform: 'none', fontWeight: 600 },
  },
  shape: { borderRadius: 14 },
  components: {
    MuiCssBaseline: {
      styleOverrides: {
        body: {
          backgroundImage:
            'radial-gradient(ellipse 80% 50% at 75% -10%, rgba(92,53,191,0.10) 0%, transparent 65%)',
          backgroundAttachment: 'fixed',
          minHeight: '100vh',
        },
      },
    },
    MuiPaper: {
      defaultProps: { elevation: 0 },
      styleOverrides: {
        root: {
          backgroundImage: 'none',
          border: `1px solid ${alpha(PURPLE, 0.1)}`,
        },
      },
    },
    MuiButton: {
      defaultProps: { disableElevation: true },
      styleOverrides: {
        root: { borderRadius: 10, padding: '9px 22px' },
        containedPrimary: {
          background: `linear-gradient(135deg, #7048D4 0%, #4220A8 100%)`,
          '&:hover': { background: `linear-gradient(135deg, #7F58E5 0%, #5230B9 100%)` },
          '&:disabled': { background: alpha(PURPLE, 0.3), color: '#fff' },
        },
        outlinedPrimary: {
          borderColor: alpha(PURPLE, 0.35),
          '&:hover': { borderColor: PURPLE, background: alpha(PURPLE, 0.04) },
        },
      },
    },
    MuiTextField: {
      defaultProps: { variant: 'outlined', size: 'small' },
      styleOverrides: {
        root: {
          '& .MuiOutlinedInput-root': {
            borderRadius: 10,
            '& fieldset': { borderColor: alpha(PURPLE, 0.2) },
            '&:hover fieldset': { borderColor: alpha(PURPLE, 0.5) },
            '&.Mui-focused fieldset': { borderColor: PURPLE },
          },
          '& .MuiInputLabel-root.Mui-focused': { color: PURPLE },
        },
      },
    },
    MuiChip: {
      styleOverrides: {
        root: { borderRadius: 8, fontWeight: 500, fontSize: '0.78rem' },
      },
    },
    MuiAlert: {
      styleOverrides: {
        root: { borderRadius: 10, fontSize: '0.875rem' },
        standardInfo: {
          backgroundColor: alpha(PURPLE, 0.07),
          color: '#3D1F8F',
          '& .MuiAlert-icon': { color: PURPLE },
        },
      },
    },
    MuiDivider: {
      styleOverrides: { root: { borderColor: alpha(PURPLE, 0.1) } },
    },
    MuiAppBar: {
      styleOverrides: {
        root: { backgroundImage: 'none' },
      },
    },
  },
});
