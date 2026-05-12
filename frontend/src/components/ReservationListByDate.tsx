import { useEffect, useMemo, useState } from 'react';
import type { Theme } from '@mui/material/styles';
import {
  Alert,
  Box,
  Chip,
  CircularProgress,
  Divider,
  Paper,
  Stack,
  TextField,
  Typography,
  alpha,
} from '@mui/material';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import GroupIcon from '@mui/icons-material/Group';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import MeetingRoomIcon from '@mui/icons-material/MeetingRoom';
import PersonIcon from '@mui/icons-material/Person';
import EventBusyIcon from '@mui/icons-material/EventBusy';
import { reservationsApi } from '../api/reservations';
import type { ReservationResponse } from '../types/api';
import { formatTime } from '../utils/time';
import { roomColor } from '../utils/roomColor';

interface Props {
  refreshKey: number;
}

function formatDate(iso: string): string {
  const [y, m, d] = iso.split('-');
  return `${d}/${m}/${y}`;
}

export default function ReservationListByDate({ refreshKey }: Props) {
  const today = new Date().toISOString().substring(0, 10);
  const [date, setDate] = useState<string>(today);
  const [reservations, setReservations] = useState<ReservationResponse[]>([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [selectedRoom, setSelectedRoom] = useState<number | null>(null);

  useEffect(() => {
    if (!date) return;
    let cancelled = false;
    setLoading(true);
    setError(null);
    reservationsApi
      .getByDate(date)
      .then((data) => {
        if (!cancelled) {
          setReservations(data);
          setSelectedRoom(null); // reset filter when date changes
        }
      })
      .catch(() => {
        if (!cancelled) setError('No se pudieron cargar las reservas');
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });
    return () => { cancelled = true; };
  }, [date, refreshKey]);

  // Unique rooms present in the current day's reservations
  const rooms = useMemo(() => {
    const seen = new Map<number, string>();
    for (const r of reservations) seen.set(r.roomId, r.roomName);
    return Array.from(seen.entries()).map(([id, name]) => ({ id, name }));
  }, [reservations]);

  const visible = selectedRoom === null
    ? reservations
    : reservations.filter((r) => r.roomId === selectedRoom);

  const isToday = date === today;

  return (
    <Paper sx={{ overflow: 'hidden' }}>
      {/* Header */}
      <Box
        sx={{
          px: { xs: 2.5, md: 3.5 },
          py: 2.5,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          flexWrap: 'wrap',
          gap: 2,
          borderBottom: '1px solid',
          borderColor: 'divider',
          background: (t) =>
            `linear-gradient(135deg, ${alpha(t.palette.primary.main, 0.05)} 0%, transparent 100%)`,
        }}
      >
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
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
            <CalendarTodayIcon sx={{ fontSize: 20 }} />
          </Box>
          <Box>
            <Typography variant="h5" sx={{ lineHeight: 1.2 }}>
              Agenda
            </Typography>
            {!loading && (
              <Typography variant="caption" color="text.secondary">
                {visible.length > 0
                  ? `${visible.length} reserva${visible.length > 1 ? 's' : ''}`
                  : 'Sin reservas'}{' '}
                · {formatDate(date)}{isToday ? ' (hoy)' : ''}
              </Typography>
            )}
          </Box>
        </Box>

        <TextField
          type="date"
          label="Fecha"
          value={date}
          onChange={(e) => setDate(e.target.value)}
          InputLabelProps={{ shrink: true }}
          sx={{ width: 170 }}
        />
      </Box>

      {/* Room filter chips — only when there are 2+ rooms */}
      {!loading && rooms.length >= 2 && (
        <Box
          sx={{
            px: { xs: 2.5, md: 3.5 },
            py: 1.5,
            display: 'flex',
            gap: 1,
            flexWrap: 'wrap',
            alignItems: 'center',
            borderBottom: '1px solid',
            borderColor: 'divider',
            background: (t) => alpha(t.palette.primary.main, 0.03),
          }}
        >
          <Typography
            variant="caption"
            sx={{ color: 'text.secondary', fontWeight: 600, mr: 0.5, flexShrink: 0 }}
          >
            Filtrar:
          </Typography>

          <Chip
            label="Todos"
            size="small"
            onClick={() => setSelectedRoom(null)}
            sx={{
              fontWeight: 600,
              ...(selectedRoom === null
                ? {
                    background: (t: Theme) => t.palette.primary.main,
                    color: '#fff',
                    '&:hover': { background: (t: Theme) => t.palette.primary.dark },
                  }
                : {
                    background: 'transparent',
                    border: '1px solid',
                    borderColor: 'divider',
                  }),
            }}
          />

          {rooms.map(({ id, name }) => {
            const color = roomColor(name);
            const active = selectedRoom === id;
            return (
              <Chip
                key={id}
                icon={<MeetingRoomIcon />}
                label={name}
                size="small"
                onClick={() => setSelectedRoom(active ? null : id)}
                sx={{
                  fontWeight: 600,
                  transition: 'all 0.15s',
                  ...(active
                    ? {
                        backgroundColor: color,
                        color: '#fff',
                        border: '1px solid transparent',
                        '& .MuiChip-icon': { color: '#fff' },
                        '&:hover': { backgroundColor: color, filter: 'brightness(1.1)' },
                      }
                    : {
                        backgroundColor: alpha(color, 0.08),
                        color: color,
                        border: `1px solid ${alpha(color, 0.25)}`,
                        '& .MuiChip-icon': { color: color },
                        '&:hover': { backgroundColor: alpha(color, 0.16) },
                      }),
                }}
              />
            );
          })}
        </Box>
      )}

      {/* Body */}
      <Box sx={{ px: { xs: 2.5, md: 3.5 }, py: 2.5 }}>
        {loading && (
          <Box sx={{ display: 'flex', justifyContent: 'center', py: 6 }}>
            <CircularProgress size={32} />
          </Box>
        )}

        {error && <Alert severity="error">{error}</Alert>}

        {!loading && !error && visible.length === 0 && (
          <Box
            sx={{
              display: 'flex',
              flexDirection: 'column',
              alignItems: 'center',
              py: 6,
              gap: 1.5,
              color: 'text.secondary',
            }}
          >
            <EventBusyIcon sx={{ fontSize: 44, opacity: 0.25 }} />
            <Typography variant="body2" sx={{ fontWeight: 500 }}>
              {selectedRoom !== null
                ? 'Sin reservas para este salón'
                : 'Sin reservas para esta fecha'}
            </Typography>
            <Typography variant="caption" sx={{ opacity: 0.7 }}>
              {selectedRoom !== null
                ? 'Probá seleccionando otro salón o "Todos"'
                : 'Seleccioná otra fecha o creá una nueva reserva'}
            </Typography>
          </Box>
        )}

        <Stack spacing={0} divider={<Divider />}>
          {visible.map((r) => {
            const color = roomColor(r.roomName);
            return (
              <Box
                key={r.id}
                sx={{
                  py: 2,
                  display: 'flex',
                  gap: 2,
                  alignItems: 'flex-start',
                  '&:first-of-type': { pt: 0 },
                  '&:last-of-type': { pb: 0 },
                }}
              >
                {/* Time column */}
                <Box
                  sx={{
                    minWidth: 64,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    pt: 0.25,
                    flexShrink: 0,
                  }}
                >
                  <Typography
                    variant="caption"
                    sx={{ fontWeight: 700, color: 'text.primary', fontSize: '0.8rem', lineHeight: 1.2 }}
                  >
                    {formatTime(r.startTime)}
                  </Typography>
                  <Box
                    sx={{
                      width: 1.5,
                      height: 18,
                      backgroundColor: alpha(color, 0.35),
                      my: 0.25,
                    }}
                  />
                  <Typography
                    variant="caption"
                    sx={{ fontWeight: 500, color: 'text.secondary', fontSize: '0.75rem', lineHeight: 1.2 }}
                  >
                    {formatTime(r.endTime)}
                  </Typography>
                </Box>

                {/* Accent strip */}
                <Box
                  sx={{
                    width: 3,
                    alignSelf: 'stretch',
                    borderRadius: 4,
                    backgroundColor: color,
                    flexShrink: 0,
                  }}
                />

                {/* Content */}
                <Box sx={{ flexGrow: 1, minWidth: 0 }}>
                  <Typography
                    variant="subtitle1"
                    sx={{ fontWeight: 600, lineHeight: 1.3, mb: 0.5, color: 'text.primary' }}
                    noWrap
                  >
                    {r.eventName}
                  </Typography>

                  <Box
                    sx={{
                      display: 'flex',
                      alignItems: 'center',
                      gap: 0.5,
                      mb: 1,
                      color: 'text.secondary',
                    }}
                  >
                    <PersonIcon sx={{ fontSize: 13 }} />
                    <Typography variant="caption">{r.customerName}</Typography>
                  </Box>

                  <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.75 }}>
                    <Chip
                      icon={<MeetingRoomIcon />}
                      label={r.roomName}
                      size="small"
                      sx={{
                        backgroundColor: alpha(color, 0.1),
                        color,
                        borderColor: alpha(color, 0.25),
                        border: '1px solid',
                        '& .MuiChip-icon': { color },
                      }}
                    />
                    <Chip
                      icon={<AccessTimeIcon />}
                      label={`${formatTime(r.startTime)} – ${formatTime(r.endTime)}`}
                      size="small"
                      variant="outlined"
                    />
                    <Chip
                      icon={<GroupIcon />}
                      label={`${r.guestCount} inv.`}
                      size="small"
                      variant="outlined"
                    />
                  </Box>
                </Box>
              </Box>
            );
          })}
        </Stack>
      </Box>
    </Paper>
  );
}
