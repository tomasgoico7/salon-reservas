namespace RoomReservations.Domain.Exceptions;

/// <summary>
/// Excepcion base para errores de reglas de negocio del dominio.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}
