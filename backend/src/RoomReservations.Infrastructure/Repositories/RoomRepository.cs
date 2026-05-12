using Microsoft.EntityFrameworkCore;
using RoomReservations.Domain.Entities;
using RoomReservations.Domain.Repositories;
using RoomReservations.Infrastructure.Persistence;

namespace RoomReservations.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ReservationsDbContext _context;

    public RoomRepository(ReservationsDbContext context) => _context = context;

    public async Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken ct = default)
        => await _context.Rooms.AsNoTracking().OrderBy(s => s.Id).ToListAsync(ct);

    public async Task<Room?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.Rooms.FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => await _context.Rooms.AnyAsync(s => s.Id == id, ct);
}
