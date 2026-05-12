import { useQuery } from '@tanstack/react-query';
import { reservationsApi } from '../api/reservations';
import type { Room } from '../types/api';

export function useRooms(): { rooms: Room[]; loading: boolean; error: string | null } {
  const { data, isLoading, isError } = useQuery({
    queryKey: ['rooms'],
    queryFn: reservationsApi.getRooms,
    staleTime: 5 * 60 * 1000, // 5 minutes — room list rarely changes
  });

  return {
    rooms: data ?? [],
    loading: isLoading,
    error: isError ? 'No se pudieron cargar los salones' : null,
  };
}
