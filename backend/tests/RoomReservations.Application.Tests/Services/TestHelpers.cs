using RoomReservations.Application.DTOs;
using RoomReservations.Domain.Entities;

namespace RoomReservations.Application.Tests.Services;

internal static class TestHelpers
{
    public static CreateReservationRequest ValidRequest(
        int roomId = 1,
        int guestCount = 20,
        TimeOnly? start = null,
        TimeOnly? end = null) => new()
    {
        RoomId = roomId,
        CustomerName = "Maria Lopez",
        EventName = "Cumpleanios Lucas",
        GuestCount = guestCount,
        Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
        StartTime = start ?? new TimeOnly(10, 0),
        EndTime = end ?? new TimeOnly(12, 0)
    };

    public static Room CreateRoom(int id = 1, string name = "Salon Estrella", int capacity = 80, bool active = true)
    {
        var room = new Room(name, capacity);
        typeof(Room).GetProperty(nameof(Room.Id))!.SetValue(room, id);
        if (!active) room.Deactivate();
        return room;
    }

    public static Reservation CreateExistingReservation(int roomId, DateOnly date, TimeOnly start, TimeOnly end)
    {
        return new Reservation(roomId, "Otro Cliente", "Otro Evento", 10, date, start, end);
    }

    public static void SetReservationId(Reservation r, int id)
        => typeof(Reservation).GetProperty(nameof(Reservation.Id))!.SetValue(r, id);

    public static void SetReservationRoom(Reservation r, Room s)
        => typeof(Reservation).GetProperty(nameof(Reservation.Room))!.SetValue(r, s);
}
