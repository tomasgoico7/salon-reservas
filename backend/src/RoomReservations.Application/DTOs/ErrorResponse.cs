namespace RoomReservations.Application.DTOs;

/// <summary>
/// Modelo estandar de respuesta de error de la API.
/// </summary>
public class ErrorResponse
{
    public int Status { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Detail { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public IDictionary<string, string[]>? Errors { get; set; }
}
