using RoomReservations.Domain.Entities;

namespace RoomReservations.Infrastructure.Persistence;

/// <summary>
/// Inicializa la base de datos en memoria con datos de ejemplo.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(ReservationsDbContext context, CancellationToken ct = default)
    {
        if (context.Rooms.Any()) return;

        var rooms = new[]
        {
            new Room("Salon Estrella",   80),
            new Room("Salon Aventura",   60),
            new Room("Salon Magico",    100),
            new Room("Salon Galaxia",    50)
        };

        await context.Rooms.AddRangeAsync(rooms, ct);
        await context.SaveChangesAsync(ct);
    }
}
