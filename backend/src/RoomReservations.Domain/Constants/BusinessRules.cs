namespace RoomReservations.Domain.Constants;

/// <summary>
/// Constantes de las reglas de negocio del dominio.
/// </summary>
public static class BusinessRules
{
    public static readonly TimeOnly OpeningTime = new(9, 0);

    public static readonly TimeOnly ClosingTime = new(18, 0);

    public const int IntervalMinutesBetweenReservations = 30;

    public const int MinimumRoomCount = 3;
}
