namespace RoomReservations.Domain.Exceptions;

/// <summary>
/// Se lanza cuando un recurso (salon, reserva, etc.) no se encuentra.
/// El middleware global la traduce a HTTP 404 (NotFound).
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message) { }

    public static NotFoundException For(string resource, object id)
        => new($"{resource} con identificador '{id}' no fue encontrado.");
}
