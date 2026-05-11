namespace RoomReservations.Domain.Exceptions;

/// <summary>
/// Se lanza cuando una nueva reserva entra en conflicto con otra existente.
/// El middleware global la traduce a HTTP 409 (Conflict).
/// </summary>
public class ReservationConflictException : DomainException
{
    public ReservationConflictException(string message) : base(message) { }
}
