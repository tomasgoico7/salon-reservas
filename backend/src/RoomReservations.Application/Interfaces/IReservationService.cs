using RoomReservations.Application.DTOs;

namespace RoomReservations.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationResponse> CreateAsync(CreateReservationRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<ReservationResponse>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
}
