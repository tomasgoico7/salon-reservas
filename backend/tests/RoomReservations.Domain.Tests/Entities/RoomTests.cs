using FluentAssertions;
using RoomReservations.Domain.Entities;

namespace RoomReservations.Domain.Tests.Entities;

public class RoomTests
{
    [Fact]
    public void Constructor_ValidData_CreatesActiveRoom()
    {
        var room = new Room("Salon Estrella", 80);

        room.Name.Should().Be("Salon Estrella");
        room.MaxCapacity.Should().Be(80);
        room.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_InvalidName_ThrowsArgumentException(string? name)
    {
        var act = () => new Room(name!, 50);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_NonPositiveCapacity_ThrowsArgumentException(int capacity)
    {
        var act = () => new Room("Salon X", capacity);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var room = new Room("Salon X", 50);
        room.Deactivate();
        room.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ReactivatesRoom()
    {
        var room = new Room("Salon X", 50);
        room.Deactivate();
        room.Activate();
        room.IsActive.Should().BeTrue();
    }
}
