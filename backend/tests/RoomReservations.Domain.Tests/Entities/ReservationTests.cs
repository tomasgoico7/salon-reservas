using FluentAssertions;
using RoomReservations.Domain.Entities;
using RoomReservations.Domain.Exceptions;

namespace RoomReservations.Domain.Tests.Entities;

public class ReservationTests
{
    private static Reservation CreateValidReservation(
        int roomId = 1,
        string customerName = "Juan Perez",
        string eventName = "Cumpleanios de Sofia",
        int guestCount = 20,
        DateOnly? date = null,
        TimeOnly? startTime = null,
        TimeOnly? endTime = null)
    {
        return new Reservation(
            roomId,
            customerName,
            eventName,
            guestCount,
            date ?? new DateOnly(2030, 1, 1),
            startTime ?? new TimeOnly(10, 0),
            endTime ?? new TimeOnly(12, 0));
    }

    [Fact]
    public void Constructor_ValidData_CreatesReservation()
    {
        var reservation = CreateValidReservation();

        reservation.RoomId.Should().Be(1);
        reservation.CustomerName.Should().Be("Juan Perez");
        reservation.EventName.Should().Be("Cumpleanios de Sofia");
        reservation.GuestCount.Should().Be(20);
        reservation.StartTime.Should().Be(new TimeOnly(10, 0));
        reservation.EndTime.Should().Be(new TimeOnly(12, 0));
        reservation.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_InvalidCustomerName_ThrowsDomainException(string? customerName)
    {
        var act = () => CreateValidReservation(customerName: customerName!);
        act.Should().Throw<DomainException>().WithMessage("*cliente*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_InvalidEventName_ThrowsDomainException(string eventName)
    {
        var act = () => CreateValidReservation(eventName: eventName);
        act.Should().Throw<DomainException>().WithMessage("*evento*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-50)]
    public void Constructor_NonPositiveGuestCount_ThrowsDomainException(int count)
    {
        var act = () => CreateValidReservation(guestCount: count);
        act.Should().Throw<DomainException>().WithMessage("*invitados*");
    }

    [Fact]
    public void Constructor_EndTimeEqualToStartTime_ThrowsDomainException()
    {
        var act = () => CreateValidReservation(
            startTime: new TimeOnly(10, 0),
            endTime: new TimeOnly(10, 0));

        act.Should().Throw<DomainException>().WithMessage("*hora de fin*mayor*");
    }

    [Fact]
    public void Constructor_EndTimeBeforeStartTime_ThrowsDomainException()
    {
        var act = () => CreateValidReservation(
            startTime: new TimeOnly(15, 0),
            endTime: new TimeOnly(10, 0));

        act.Should().Throw<DomainException>().WithMessage("*hora de fin*mayor*");
    }

    [Theory]
    [InlineData(8, 30, 10, 0)]   // antes de 09:00
    [InlineData(7, 0, 9, 0)]     // antes de 09:00
    public void Constructor_StartBeforeOpening_ThrowsDomainException(int hi, int mi, int hf, int mf)
    {
        var act = () => CreateValidReservation(
            startTime: new TimeOnly(hi, mi),
            endTime: new TimeOnly(hf, mf));

        act.Should().Throw<DomainException>().WithMessage("*horario*");
    }

    [Theory]
    [InlineData(17, 0, 18, 30)]  // despues de 18:00
    [InlineData(17, 0, 19, 0)]
    public void Constructor_EndAfterClosing_ThrowsDomainException(int hi, int mi, int hf, int mf)
    {
        var act = () => CreateValidReservation(
            startTime: new TimeOnly(hi, mi),
            endTime: new TimeOnly(hf, mf));

        act.Should().Throw<DomainException>().WithMessage("*horario*");
    }

    [Fact]
    public void Constructor_ExactOpeningToClosingRange_DoesNotThrow()
    {
        var act = () => CreateValidReservation(
            startTime: new TimeOnly(9, 0),
            endTime: new TimeOnly(18, 0));

        act.Should().NotThrow();
    }

    // ---------- ConflictsWith ----------

    [Fact]
    public void ConflictsWith_DifferentRoom_ReturnsFalse()
    {
        var r1 = CreateValidReservation(roomId: 1, startTime: new TimeOnly(10, 0), endTime: new TimeOnly(12, 0));
        var r2 = CreateValidReservation(roomId: 2, startTime: new TimeOnly(10, 0), endTime: new TimeOnly(12, 0));

        r1.ConflictsWith(r2).Should().BeFalse();
    }

    [Fact]
    public void ConflictsWith_DifferentDate_ReturnsFalse()
    {
        var r1 = CreateValidReservation(date: new DateOnly(2030, 1, 1));
        var r2 = CreateValidReservation(date: new DateOnly(2030, 1, 2));

        r1.ConflictsWith(r2).Should().BeFalse();
    }

    [Fact]
    public void ConflictsWith_OverlappingRanges_ReturnsTrue()
    {
        var r1 = CreateValidReservation(startTime: new TimeOnly(10, 0), endTime: new TimeOnly(12, 0));
        var r2 = CreateValidReservation(startTime: new TimeOnly(11, 0), endTime: new TimeOnly(13, 0));

        r1.ConflictsWith(r2).Should().BeTrue();
    }

    [Fact]
    public void ConflictsWith_RangesFullyContained_ReturnsTrue()
    {
        var r1 = CreateValidReservation(startTime: new TimeOnly(10, 0), endTime: new TimeOnly(14, 0));
        var r2 = CreateValidReservation(startTime: new TimeOnly(11, 0), endTime: new TimeOnly(12, 0));

        r1.ConflictsWith(r2).Should().BeTrue();
    }

    [Fact]
    public void ConflictsWith_NewStartsAtOtherEnd_NoBuffer_ReturnsTrue()
    {
        // r2 termina 12:00, r1 arranca 12:00 -> sin buffer de 30 min entre ellas -> CONFLICTO
        var r2 = CreateValidReservation(startTime: new TimeOnly(10, 0), endTime: new TimeOnly(12, 0));
        var r1 = CreateValidReservation(startTime: new TimeOnly(12, 0), endTime: new TimeOnly(14, 0));

        r1.ConflictsWith(r2).Should().BeTrue();
    }

    [Fact]
    public void ConflictsWith_NewStartsLessThan30MinAfter_ReturnsTrue()
    {
        var r2 = CreateValidReservation(startTime: new TimeOnly(10, 0), endTime: new TimeOnly(12, 0));
        var r1 = CreateValidReservation(startTime: new TimeOnly(12, 15), endTime: new TimeOnly(14, 0));

        r1.ConflictsWith(r2).Should().BeTrue();
    }

    [Fact]
    public void ConflictsWith_NewStartsExactly30MinAfter_ReturnsFalse()
    {
        // r2 termina 12:00 + 30 min = 12:30 -> r1 desde 12:30 cumple el buffer exacto
        var r2 = CreateValidReservation(startTime: new TimeOnly(10, 0), endTime: new TimeOnly(12, 0));
        var r1 = CreateValidReservation(startTime: new TimeOnly(12, 30), endTime: new TimeOnly(14, 0));

        r1.ConflictsWith(r2).Should().BeFalse();
    }

    [Fact]
    public void ConflictsWith_NewEndsLessThan30MinBefore_ReturnsTrue()
    {
        var r2 = CreateValidReservation(startTime: new TimeOnly(14, 0), endTime: new TimeOnly(16, 0));
        var r1 = CreateValidReservation(startTime: new TimeOnly(12, 0), endTime: new TimeOnly(13, 45));

        r1.ConflictsWith(r2).Should().BeTrue();
    }

    [Fact]
    public void ConflictsWith_NewEndsExactly30MinBefore_ReturnsFalse()
    {
        var r2 = CreateValidReservation(startTime: new TimeOnly(14, 0), endTime: new TimeOnly(16, 0));
        var r1 = CreateValidReservation(startTime: new TimeOnly(12, 0), endTime: new TimeOnly(13, 30));

        r1.ConflictsWith(r2).Should().BeFalse();
    }

    [Fact]
    public void ConflictsWith_Null_ThrowsArgumentNullException()
    {
        var r1 = CreateValidReservation();
        var act = () => r1.ConflictsWith(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
