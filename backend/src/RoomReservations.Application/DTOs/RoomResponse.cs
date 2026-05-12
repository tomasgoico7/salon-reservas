namespace RoomReservations.Application.DTOs;

public class RoomResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxCapacity { get; set; }
    public bool IsActive { get; set; }
}
