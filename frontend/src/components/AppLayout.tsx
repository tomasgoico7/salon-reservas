import { ReactNode } from 'react';
import { AppBar, Box, Container, Toolbar, Tooltip, Typography, alpha } from '@mui/material';
import CelebrationIcon from '@mui/icons-material/Celebration';
import { useHealthCheck } from './useHealthCheck';

interface Props {
  children: ReactNode;
}

const STATUS_CONFIG = {
  checking: { color: '#FACC15', glow: '#FACC15', label: 'Verificando...' },
  online:   { color: '#4ADE80', glow: '#4ADE80', label: 'Sistema activo' },
  offline:  { color: '#F87171', glow: '#F87171', label: 'Sin conexión' },
};

export default function AppLayout({ children }: Props) {
  const health = useHealthCheck();
  const { color, glow, label } = STATUS_CONFIG[health];

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <AppBar
        position="sticky"
        elevation={0}
        sx={{
          background: 'linear-gradient(135deg, #3D1F8F 0%, #5C35BF 60%, #6B42D4 100%)',
          borderBottom: `1px solid ${alpha('#fff', 0.1)}`,
        }}
      >
        <Toolbar sx={{ gap: 1.5 }}>
          <Box
            sx={{
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              width: 34,
              height: 34,
              borderRadius: '10px',
              background: alpha('#fff', 0.15),
              flexShrink: 0,
            }}
          >
            <CelebrationIcon sx={{ fontSize: 20 }} />
          </Box>

          <Box sx={{ flexGrow: 1 }}>
            <Typography
              variant="h6"
              sx={{ fontWeight: 700, lineHeight: 1.25, letterSpacing: '-0.3px' }}
            >
              SalonReservas
            </Typography>
            <Typography
              variant="caption"
              sx={{ opacity: 0.65, lineHeight: 1, display: 'block' }}
            >
              Gestión de eventos
            </Typography>
          </Box>

          <Tooltip title={`Backend: ${label}`} arrow>
            <Box
              sx={{
                display: { xs: 'none', sm: 'flex' },
                alignItems: 'center',
                gap: 0.75,
                px: 1.5,
                py: 0.6,
                borderRadius: '8px',
                background: alpha('#fff', 0.12),
                cursor: 'default',
              }}
            >
              <Box
                sx={{
                  width: 7,
                  height: 7,
                  borderRadius: '50%',
                  backgroundColor: color,
                  boxShadow: `0 0 6px ${glow}`,
                  transition: 'background-color 0.4s, box-shadow 0.4s',
                }}
              />
              <Typography variant="caption" sx={{ fontWeight: 500, opacity: 0.9, transition: 'opacity 0.4s' }}>
                {label}
              </Typography>
            </Box>
          </Tooltip>
        </Toolbar>
      </AppBar>

      <Container maxWidth="xl" sx={{ flexGrow: 1, py: { xs: 3, md: 5 } }}>
        {children}
      </Container>

      <Box
        component="footer"
        sx={{
          py: 2,
          px: { xs: 2, md: 4 },
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          borderTop: '1px solid',
          borderColor: 'divider',
          flexWrap: 'wrap',
          gap: 1,
        }}
      >
        <Typography variant="caption" color="text.secondary">
          © {new Date().getFullYear()} SalonReservas · Gestión de eventos
        </Typography>
        <Typography variant="caption" color="text.secondary">
          Horario de reservas: 09:00 – 18:00 hs.
        </Typography>
      </Box>
    </Box>
  );
}
