using Microsoft.EntityFrameworkCore;
using RoomReservations.Domain.Entities;
using RoomReservations.Domain.Repositories;
using RoomReservations.Infrastructure.Persistence;

namespace RoomReservations.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ReservationsDbContext _context;

    public ReservationRepository(ReservationsDbContext context) => _context = context;

    public async Task<Reservation> AddAsync(Reservation reservation, CancellationToken ct = default)
    {
        await _context.Reservations.AddAsync(reservation, ct);
        return reservation;
    }

    public async Task<IReadOnlyList<Reservation>> GetByDateAsync(
        DateOnly date,
        CancellationToken ct = default)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Include(r => r.Room)
            .Where(r => r.Date == date)
            .OrderBy(r => r.RoomId).ThenBy(r => r.StartTime)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Reservation>> GetByRoomAndDateAsync(
        int roomId,
        DateOnly date,
        CancellationToken ct = default)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Where(r => r.RoomId == roomId && r.Date == date)
            .OrderBy(r => r.StartTime)
            .ToListAsync(ct);
    }

    public async Task<Reservation?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Reservations
            .AsNoTracking()
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }
}
