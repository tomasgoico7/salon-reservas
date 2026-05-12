import { apiClient } from './client';
import type { CreateReservationRequest, ReservationResponse, Room } from '../types/api';

export const reservationsApi = {
  getRooms: async (): Promise<Room[]> => {
    const { data } = await apiClient.get<Room[]>('/rooms');
    return data;
  },

  create: async (req: CreateReservationRequest): Promise<ReservationResponse> => {
    const { data } = await apiClient.post<ReservationResponse>('/reservations', req);
    return data;
  },

  getByDate: async (date: string): Promise<ReservationResponse[]> => {
    const { data } = await apiClient.get<ReservationResponse[]>('/reservations', {
      params: { date },
    });
    return data;
  },
};
