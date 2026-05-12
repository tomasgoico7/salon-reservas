import { describe, it, expect } from 'vitest';
import { reservationSchema } from '../schemas/reservationSchema';

const valid = {
  roomId: 1,
  customerName: 'Juan Pérez',
  eventName: 'Cumpleaños',
  guestCount: 50,
  date: '2026-12-01',
  startTime: '10:00',
  endTime: '12:00',
};

describe('reservationSchema', () => {
  it('accepts a valid reservation', () => {
    expect(reservationSchema.safeParse(valid).success).toBe(true);
  });

  it('rejects startTime before 09:00', () => {
    const result = reservationSchema.safeParse({ ...valid, startTime: '08:00' });
    expect(result.success).toBe(false);
    if (!result.success) {
      const paths = result.error.issues.map((i) => i.path[0]);
      expect(paths).toContain('startTime');
    }
  });

  it('rejects startTime at or after 18:00', () => {
    const result = reservationSchema.safeParse({ ...valid, startTime: '18:00', endTime: '18:30' });
    expect(result.success).toBe(false);
    if (!result.success) {
      const paths = result.error.issues.map((i) => i.path[0]);
      expect(paths).toContain('startTime');
    }
  });

  it('rejects endTime after 18:00', () => {
    const result = reservationSchema.safeParse({ ...valid, endTime: '18:30' });
    expect(result.success).toBe(false);
    if (!result.success) {
      const paths = result.error.issues.map((i) => i.path[0]);
      expect(paths).toContain('endTime');
    }
  });

  it('rejects endTime equal to startTime', () => {
    const result = reservationSchema.safeParse({ ...valid, startTime: '10:00', endTime: '10:00' });
    expect(result.success).toBe(false);
    if (!result.success) {
      const paths = result.error.issues.map((i) => i.path[0]);
      expect(paths).toContain('endTime');
    }
  });

  it('rejects endTime before startTime', () => {
    const result = reservationSchema.safeParse({ ...valid, startTime: '14:00', endTime: '13:00' });
    expect(result.success).toBe(false);
    if (!result.success) {
      const paths = result.error.issues.map((i) => i.path[0]);
      expect(paths).toContain('endTime');
    }
  });

  it('rejects roomId of 0', () => {
    const result = reservationSchema.safeParse({ ...valid, roomId: 0 });
    expect(result.success).toBe(false);
  });

  it('rejects empty customerName', () => {
    const result = reservationSchema.safeParse({ ...valid, customerName: '' });
    expect(result.success).toBe(false);
  });

  it('rejects guestCount of 0', () => {
    const result = reservationSchema.safeParse({ ...valid, guestCount: 0 });
    expect(result.success).toBe(false);
  });

  it('accepts boundary: startTime 09:00, endTime 18:00', () => {
    const result = reservationSchema.safeParse({ ...valid, startTime: '09:00', endTime: '18:00' });
    expect(result.success).toBe(true);
  });

  it('accepts boundary: startTime 17:30, endTime 18:00', () => {
    const result = reservationSchema.safeParse({ ...valid, startTime: '17:30', endTime: '18:00' });
    expect(result.success).toBe(true);
  });
});
