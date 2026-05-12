using Microsoft.EntityFrameworkCore;
using RoomReservations.Domain.Entities;

namespace RoomReservations.Infrastructure.Persistence;

public class ReservationsDbContext : DbContext
{
    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options) { }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReservationsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
