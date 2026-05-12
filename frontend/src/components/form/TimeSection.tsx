import { useMemo } from 'react';
import { Controller } from 'react-hook-form';
import type { Control, FieldErrors } from 'react-hook-form';
import { Alert, Box, Divider, MenuItem, TextField } from '@mui/material';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import InfoOutlinedIcon from '@mui/icons-material/InfoOutlined';
import type { ReservationFormData } from '../../schemas/reservationSchema';
import { generateSlots, START_SLOTS } from '../../utils/time';
import SectionLabel from './SectionLabel';

interface Props {
  control: Control<ReservationFormData>;
  errors: FieldErrors<ReservationFormData>;
  startTime: string;
  onStartTimeChange: (value: string) => void;
}

export default function TimeSection({ control, errors, startTime, onStartTimeChange }: Props) {
  const endSlots = useMemo(() => {
    if (!startTime) return [];
    const [h, m] = startTime.split(':').map(Number);
    return generateSlots(h * 60 + m + 30, 18 * 60);
  }, [startTime]);

  return (
    <>
      <SectionLabel icon={<AccessTimeIcon sx={{ fontSize: 15 }} />} label="HORARIO" />
      <Box sx={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 2, mb: 0.5 }}>
        <Controller
          name="startTime"
          control={control}
          render={({ field }) => (
            <TextField
              {...field}
              select
              label="Desde"
              error={!!errors.startTime}
              helperText={errors.startTime?.message ?? '09:00 – 17:30'}
              onChange={(e) => onStartTimeChange(e.target.value)}
            >
              <MenuItem value="" disabled>
                Seleccioná hora de inicio
              </MenuItem>
              {START_SLOTS.map((slot) => (
                <MenuItem key={slot} value={slot}>
                  {slot}
                </MenuItem>
              ))}
            </TextField>
          )}
        />
        <Controller
          name="endTime"
          control={control}
          render={({ field }) => (
            <TextField
              {...field}
              select
              label="Hasta"
              disabled={!startTime}
              error={!!errors.endTime}
              helperText={
                !startTime
                  ? 'Elegí primero la hora de inicio'
                  : (errors.endTime?.message ?? `${startTime} – 18:00`)
              }
            >
              <MenuItem value="" disabled>
                Seleccioná hora de fin
              </MenuItem>
              {endSlots.map((slot) => (
                <MenuItem key={slot} value={slot}>
                  {slot}
                </MenuItem>
              ))}
            </TextField>
          )}
        />
      </Box>
      <Alert severity="info" icon={<InfoOutlinedIcon fontSize="inherit" />} sx={{ mb: 2, mt: 1, py: 0.5 }}>
        Horario permitido <strong>09:00 – 18:00</strong>. Se requieren{' '}
        <strong>30 min de intervalo</strong> entre reservas.
      </Alert>
      <Divider sx={{ mb: 2 }} />
    </>
  );
}
