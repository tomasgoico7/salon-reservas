import { Controller } from 'react-hook-form';
import type { Control, FieldErrors } from 'react-hook-form';
import { Box, Divider, MenuItem, TextField } from '@mui/material';
import EventAvailableIcon from '@mui/icons-material/EventAvailable';
import type { Room } from '../../types/api';
import type { ReservationFormData } from '../../schemas/reservationSchema';
import SectionLabel from './SectionLabel';

const TODAY = new Date().toISOString().substring(0, 10);

interface Props {
  control: Control<ReservationFormData>;
  errors: FieldErrors<ReservationFormData>;
  rooms: Room[];
  loadingRooms: boolean;
}

export default function RoomDateSection({ control, errors, rooms, loadingRooms }: Props) {
  return (
    <>
      <SectionLabel icon={<EventAvailableIcon sx={{ fontSize: 15 }} />} label="SALÓN Y FECHA" />
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' },
          gap: 2,
          mb: 2,
        }}
      >
        <Controller
          name="roomId"
          control={control}
          render={({ field }) => (
            <TextField
              {...field}
              select
              label="Salón"
              fullWidth
              disabled={loadingRooms}
              error={!!errors.roomId}
              helperText={errors.roomId?.message}
              onChange={(e) => field.onChange(Number(e.target.value))}
            >
              <MenuItem value={0} disabled>
                Seleccione un salón
              </MenuItem>
              {rooms.map((s) => (
                <MenuItem key={s.id} value={s.id}>
                  {s.name} · cap. {s.maxCapacity}
                </MenuItem>
              ))}
            </TextField>
          )}
        />
        <Controller
          name="date"
          control={control}
          render={({ field }) => (
            <TextField
              {...field}
              type="date"
              label="Fecha"
              fullWidth
              InputLabelProps={{ shrink: true }}
              inputProps={{ min: TODAY }}
              error={!!errors.date}
              helperText={errors.date?.message}
            />
          )}
        />
      </Box>
      <Divider sx={{ mb: 2 }} />
    </>
  );
}
