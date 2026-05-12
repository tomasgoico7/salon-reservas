export interface Room {
  id: number;
  name: string;
  maxCapacity: number;
  isActive: boolean;
}

export interface ReservationResponse {
  id: number;
  roomId: number;
  roomName: string;
  customerName: string;
  eventName: string;
  guestCount: number;
  date: string;
  startTime: string;
  endTime: string;
  createdAt: string;
}

export interface CreateReservationRequest {
  roomId: number;
  customerName: string;
  eventName: string;
  guestCount: number;
  date: string;
  startTime: string;
  endTime: string;
}

export interface ApiError {
  status: number;
  title: string;
  detail?: string;
  errors?: Record<string, string[]>;
}

export function isApiError(err: unknown): err is ApiError {
  return (
    typeof err === 'object' &&
    err !== null &&
    'title' in err &&
    typeof (err as Record<string, unknown>).title === 'string'
  );
}
