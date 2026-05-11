using RoomReservations.Domain.Entities;

namespace RoomReservations.Domain.Repositories;

public interface IReservationRepository
{
    Task<Reservation> AddAsync(Reservation reservation, CancellationToken ct = default);

    Task<IReadOnlyList<Reservation>> GetByDateAsync(DateOnly date, CancellationToken ct = default);

    Task<IReadOnlyList<Reservation>> GetByRoomAndDateAsync(
        int roomId,
        DateOnly date,
        CancellationToken ct = default);

    Task<Reservation?> GetByIdAsync(int id, CancellationToken ct = default);
}
