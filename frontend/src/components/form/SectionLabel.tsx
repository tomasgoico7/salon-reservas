import { Box, Typography, alpha } from '@mui/material';

interface Props {
  icon: React.ReactNode;
  label: string;
}

export default function SectionLabel({ icon, label }: Props) {
  return (
    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 1.25 }}>
      <Box
        sx={{
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          width: 28,
          height: 28,
          borderRadius: '8px',
          background: (t) => alpha(t.palette.primary.main, 0.1),
          color: 'primary.main',
          flexShrink: 0,
        }}
      >
        {icon}
      </Box>
      <Typography variant="subtitle2" color="text.secondary">
        {label}
      </Typography>
    </Box>
  );
}
