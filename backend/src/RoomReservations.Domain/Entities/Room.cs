namespace RoomReservations.Domain.Entities;

/// <summary>
/// Representa un salon fisico donde se pueden realizar eventos infantiles.
/// </summary>
public class Room
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int MaxCapacity { get; private set; }
    public bool IsActive { get; private set; }

    private Room() { }

    public Room(string name, int maxCapacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre del salon es obligatorio.", nameof(name));

        if (maxCapacity <= 0)
            throw new ArgumentException("La capacidad maxima debe ser positiva.", nameof(maxCapacity));

        Name = name.Trim();
        MaxCapacity = maxCapacity;
        IsActive = true;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
