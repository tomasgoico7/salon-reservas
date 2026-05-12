namespace RoomReservations.Application.DTOs;

/// <summary>
/// DTO de salida con los datos de una reserva.
/// </summary>
public class ReservationResponse
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public int GuestCount { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public DateTime CreatedAt { get; set; }
}
