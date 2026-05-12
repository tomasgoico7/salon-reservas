import { z } from 'zod';

export const timeRegex = /^([01][0-9]|2[0-3]):[0-5][0-9]$/;

export const reservationBaseSchema = z.object({
  roomId: z.number().int().positive('Seleccione un salon'),
  customerName: z.string().min(1, 'El nombre del cliente es obligatorio').max(120),
  eventName: z.string().min(1, 'El nombre del evento es obligatorio').max(150),
  guestCount: z.number().int().min(1, 'Debe ser mayor a cero').max(500),
  date: z.string().min(1, 'La fecha es obligatoria'),
  startTime: z.string().regex(timeRegex, 'Formato HH:mm'),
  endTime: z.string().regex(timeRegex, 'Formato HH:mm'),
});
