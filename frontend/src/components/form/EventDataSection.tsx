import { Controller } from 'react-hook-form';
import type { Control, FieldErrors } from 'react-hook-form';
import { Box, TextField } from '@mui/material';
import PersonOutlineIcon from '@mui/icons-material/PersonOutline';
import type { ReservationFormData } from '../../schemas/reservationSchema';
import SectionLabel from './SectionLabel';

interface Props {
  control: Control<ReservationFormData>;
  errors: FieldErrors<ReservationFormData>;
}

export default function EventDataSection({ control, errors }: Props) {
  return (
    <>
      <SectionLabel icon={<PersonOutlineIcon sx={{ fontSize: 15 }} />} label="DATOS DEL EVENTO" />
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' },
          gap: 2,
          mb: 2,
        }}
      >
        <Controller
          name="customerName"
          control={control}
          render={({ field }) => (
            <TextField
              {...field}
              label="Nombre del cliente"
              fullWidth
              error={!!errors.customerName}
              helperText={errors.customerName?.message}
            />
          )}
        />
        <Controller
          name="eventName"
          control={control}
          render={({ field }) => (
            <TextField
              {...field}
              label="Nombre del evento"
              fullWidth
              error={!!errors.eventName}
              helperText={errors.eventName?.message}
            />
          )}
        />
        <Controller
          name="guestCount"
          control={control}
          render={({ field }) => (
            <TextField
              {...field}
              label="Cantidad de invitados"
              fullWidth
              inputProps={{ inputMode: 'numeric', pattern: '[0-9]*' }}
              error={!!errors.guestCount}
              helperText={errors.guestCount?.message}
              value={field.value === 0 ? '' : field.value}
              onChange={(e) => {
                const raw = e.target.value.replace(/\D/g, '');
                field.onChange(raw === '' ? 0 : Number(raw));
              }}
            />
          )}
        />
      </Box>
    </>
  );
}
