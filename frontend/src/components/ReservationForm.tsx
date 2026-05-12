import { useForm, useWatch } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { Alert, Box, Button, CircularProgress, Paper, Typography, alpha } from '@mui/material';
import EventAvailableIcon from '@mui/icons-material/EventAvailable';
import { useState } from 'react';
import { reservationSchema, ReservationFormData } from '../schemas/reservationSchema';
import { reservationsApi } from '../api/reservations';
import { useRooms } from './useRooms';
import { isApiError } from '../types/api';
import RoomDateSection from './form/RoomDateSection';
import TimeSection from './form/TimeSection';
import EventDataSection from './form/EventDataSection';

interface Props {
  onReservationCreated: () => void;
}

export default function ReservationForm({ onReservationCreated }: Props) {
  const { rooms, loading: loadingRooms } = useRooms();
  const [submitting, setSubmitting] = useState(false);
  const [apiError, setApiError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const {
    control,
    handleSubmit,
    reset,
    setValue,
    formState: { errors },
  } = useForm<ReservationFormData>({
    resolver: zodResolver(reservationSchema),
    defaultValues: {
      roomId: 0,
      customerName: '',
      eventName: '',
      guestCount: 1,
      date: '',
      startTime: '',
      endTime: '',
    },
  });

  const startTime = useWatch({ control, name: 'startTime' });

  const onSubmit = async (data: ReservationFormData) => {
    setSubmitting(true);
    setApiError(null);
    setSuccessMessage(null);
    try {
      await reservationsApi.create({
        ...data,
        startTime: data.startTime + ':00',
        endTime: data.endTime + ':00',
      });
      setSuccessMessage('Reserva creada correctamente');
      reset();
      onReservationCreated();
    } catch (err) {
      if (isApiError(err) && err.errors && Object.keys(err.errors).length > 0) {
        const messages = Object.values(err.errors)
          .flat()
          .map((msg) => msg.replace(/\.\s*Severity:\s*\w+$/i, '').trim());
        setApiError(messages.join(' / '));
      } else if (isApiError(err)) {
        setApiError(err.detail || err.title || 'Ocurrió un error al crear la reserva');
      } else {
        setApiError('Ocurrió un error al crear la reserva');
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Paper sx={{ overflow: 'hidden' }}>
      {/* Header */}
      <Box
        sx={{
          px: { xs: 2.5, md: 3.5 },
          py: 1.75,
          display: 'flex',
          alignItems: 'center',
          gap: 1.5,
          borderBottom: '1px solid',
          borderColor: 'divider',
          background: (t) =>
            `linear-gradient(135deg, ${alpha(t.palette.primary.main, 0.05)} 0%, transparent 100%)`,
        }}
      >
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            width: 38,
            height: 38,
            borderRadius: '10px',
            background: (t) =>
              `linear-gradient(135deg, ${t.palette.primary.light} 0%, ${t.palette.primary.dark} 100%)`,
            color: '#fff',
            flexShrink: 0,
          }}
        >
          <EventAvailableIcon sx={{ fontSize: 20 }} />
        </Box>
        <Box>
          <Typography variant="h5" sx={{ lineHeight: 1.2 }}>
            Nueva reserva
          </Typography>
          <Typography variant="caption" color="text.secondary">
            Completá todos los campos para confirmar
          </Typography>
        </Box>
      </Box>

      <Box
        component="form"
        onSubmit={handleSubmit(onSubmit)}
        sx={{ px: { xs: 2.5, md: 3.5 }, py: 1.5 }}
      >
        {apiError && (
          <Alert severity="error" sx={{ mb: 2.5 }} onClose={() => setApiError(null)}>
            {apiError}
          </Alert>
        )}
        {successMessage && (
          <Alert severity="success" sx={{ mb: 2.5 }} onClose={() => setSuccessMessage(null)}>
            {successMessage}
          </Alert>
        )}

        <RoomDateSection
          control={control}
          errors={errors}
          rooms={rooms}
          loadingRooms={loadingRooms}
        />

        <TimeSection
          control={control}
          errors={errors}
          startTime={startTime}
          onStartTimeChange={(value) => {
            setValue('startTime', value);
            setValue('endTime', '');
          }}
        />

        <EventDataSection control={control} errors={errors} />

        <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
          <Button
            type="submit"
            variant="contained"
            size="large"
            disabled={submitting}
            startIcon={submitting ? <CircularProgress size={16} color="inherit" /> : null}
            sx={{ minWidth: 160 }}
          >
            {submitting ? 'Guardando...' : 'Crear reserva'}
          </Button>
        </Box>
      </Box>
    </Paper>
  );
}
