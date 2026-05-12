import { z } from 'zod';
import { reservationBaseSchema } from './reservationBase';
import { timeToMinutes, OPENING_MIN, CLOSING_MIN } from './hours';

export const reservationSchema = reservationBaseSchema
  .refine((d) => timeToMinutes(d.startTime) >= OPENING_MIN, {
    message: 'La hora de inicio no puede ser anterior a las 09:00',
    path: ['startTime'],
  })
  .refine((d) => timeToMinutes(d.startTime) < CLOSING_MIN, {
    message: 'La hora de inicio debe ser anterior a las 18:00',
    path: ['startTime'],
  })
  .refine((d) => timeToMinutes(d.endTime) <= CLOSING_MIN, {
    message: 'La hora de fin no puede ser posterior a las 18:00',
    path: ['endTime'],
  })
  .refine((d) => timeToMinutes(d.endTime) > timeToMinutes(d.startTime), {
    message: 'La hora de fin debe ser mayor que la hora de inicio',
    path: ['endTime'],
  });

export type ReservationFormData = z.infer<typeof reservationSchema>;
