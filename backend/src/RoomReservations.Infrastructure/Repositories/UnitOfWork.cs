using RoomReservations.Domain.Repositories;
using RoomReservations.Infrastructure.Persistence;

namespace RoomReservations.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ReservationsDbContext _context;

    public UnitOfWork(ReservationsDbContext context) => _context = context;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
