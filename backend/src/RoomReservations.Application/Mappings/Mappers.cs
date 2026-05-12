using RoomReservations.Application.DTOs;
using RoomReservations.Domain.Entities;

namespace RoomReservations.Application.Mappings;

public static class ReservationMapper
{
    public static ReservationResponse ToResponse(Reservation r) => new()
    {
        Id = r.Id,
        RoomId = r.RoomId,
        RoomName = r.Room?.Name ?? string.Empty,
        CustomerName = r.CustomerName,
        EventName = r.EventName,
        GuestCount = r.GuestCount,
        Date = r.Date,
        StartTime = r.StartTime,
        EndTime = r.EndTime,
        CreatedAt = r.CreatedAt
    };
}

public static class RoomMapper
{
    public static RoomResponse ToResponse(Room s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        MaxCapacity = s.MaxCapacity,
        IsActive = s.IsActive
    };
}
