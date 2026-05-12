namespace RoomReservations.Application.Interfaces;

public interface IReservationLockService
{
    Task<IDisposable> AcquireAsync(int roomId, DateOnly date, CancellationToken ct = default);
}
