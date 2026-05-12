using System.Collections.Concurrent;
using RoomReservations.Application.Interfaces;

namespace RoomReservations.Application.Services;

public sealed class ReservationLockService : IReservationLockService, IDisposable
{
    private readonly ConcurrentDictionary<(int, DateOnly), SemaphoreSlim> _locks = new();

    public async Task<IDisposable> AcquireAsync(int roomId, DateOnly date, CancellationToken ct = default)
    {
        var semaphore = _locks.GetOrAdd((roomId, date), _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(ct);
        return new LockHandle(semaphore);
    }

    public void Dispose()
    {
        foreach (var semaphore in _locks.Values)
            semaphore.Dispose();
    }

    private sealed class LockHandle : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        internal LockHandle(SemaphoreSlim semaphore) => _semaphore = semaphore;
        public void Dispose() => _semaphore.Release();
    }
}
