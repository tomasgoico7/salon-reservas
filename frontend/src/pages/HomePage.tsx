import { useState } from 'react';
import { Box, Typography, alpha } from '@mui/material';
import CalendarMonthIcon from '@mui/icons-material/CalendarMonth';
import ReservationForm from '../components/ReservationForm';
import ReservationListByDate from '../components/ReservationListByDate';

export default function HomePage() {
  const [refreshKey, setRefreshKey] = useState(0);

  return (
    <Box>
      {/* Two-column layout on desktop */}
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', lg: '480px 1fr' },
          gap: 3,
          alignItems: 'start',
        }}
      >
        {/* Left: form */}
        <Box
          sx={{
            position: { lg: 'sticky' },
            top: { lg: 88 },
          }}
        >
          <ReservationForm onReservationCreated={() => setRefreshKey((k) => k + 1)} />
        </Box>

        {/* Right: list */}
        <Box>
          <ReservationListByDate refreshKey={refreshKey} />
        </Box>
      </Box>

      {/* Decorative background blob */}
      <Box
        aria-hidden
        sx={{
          position: 'fixed',
          bottom: '-10%',
          left: '-5%',
          width: 400,
          height: 400,
          borderRadius: '50%',
          background: (t) => alpha(t.palette.primary.light, 0.06),
          filter: 'blur(60px)',
          pointerEvents: 'none',
          zIndex: -1,
        }}
      />
    </Box>
  );
}
