using RoomReservations.Domain.Constants;
using RoomReservations.Domain.Exceptions;

namespace RoomReservations.Domain.Entities;

public class Reservation
{
    public int Id { get; private set; }
    public int RoomId { get; private set; }
    public Room? Room { get; private set; }

    public string CustomerName { get; private set; } = string.Empty;
    public string EventName { get; private set; } = string.Empty;
    public int GuestCount { get; private set; }

    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Reservation() { }

    public Reservation(
        int roomId,
        string customerName,
        string eventName,
        int guestCount,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime)
    {
        ValidateBasicData(customerName, eventName, guestCount);
        ValidateTimeRange(startTime, endTime);

        RoomId = roomId;
        CustomerName = customerName.Trim();
        EventName = eventName.Trim();
        GuestCount = guestCount;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        CreatedAt = DateTime.UtcNow;
    }

    private static void ValidateBasicData(string customerName, string eventName, int guestCount)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("El nombre del cliente es obligatorio.");

        if (string.IsNullOrWhiteSpace(eventName))
            throw new DomainException("El nombre del evento es obligatorio.");

        if (guestCount <= 0)
            throw new DomainException("La cantidad de invitados debe ser mayor a cero.");
    }

    private static void ValidateTimeRange(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
            throw new DomainException("La hora de fin debe ser mayor que la hora de inicio.");

        if (startTime < BusinessRules.OpeningTime || endTime > BusinessRules.ClosingTime)
        {
            throw new DomainException(
                $"El horario de la reserva debe estar entre {BusinessRules.OpeningTime:HH\\:mm} " +
                $"y {BusinessRules.ClosingTime:HH\\:mm}.");
        }
    }

    /// <summary>
    /// Determina si esta reserva tiene conflicto con otra reserva del mismo salon,
    /// teniendo en cuenta el intervalo obligatorio entre reservas.
    /// </summary>
    public bool ConflictsWith(Reservation other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (other.RoomId != RoomId) return false;
        if (other.Date != Date) return false;

        var buffer = TimeSpan.FromMinutes(BusinessRules.IntervalMinutesBetweenReservations);
        var otherStartWithBuffer = other.StartTime.ToTimeSpan() - buffer;
        var otherEndWithBuffer = other.EndTime.ToTimeSpan() + buffer;

        var thisStart = StartTime.ToTimeSpan();
        var thisEnd = EndTime.ToTimeSpan();

        return thisStart < otherEndWithBuffer && thisEnd > otherStartWithBuffer;
    }
}
