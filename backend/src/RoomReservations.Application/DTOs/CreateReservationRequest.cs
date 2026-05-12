namespace RoomReservations.Application.DTOs;

/// <summary>
/// DTO de entrada para crear una nueva reserva.
/// </summary>
public class CreateReservationRequest
{
    public int RoomId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public int GuestCount { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}
