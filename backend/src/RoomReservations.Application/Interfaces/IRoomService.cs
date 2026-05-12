using RoomReservations.Application.DTOs;

namespace RoomReservations.Application.Interfaces;

public interface IRoomService
{
    Task<IReadOnlyList<RoomResponse>> GetAllAsync(CancellationToken ct = default);
}
