using FluentAssertions;
using Moq;
using RoomReservations.Application.Services;
using RoomReservations.Application.Validators;
using RoomReservations.Domain.Entities;
using RoomReservations.Domain.Repositories;
using static RoomReservations.Application.Tests.Services.TestHelpers;

namespace RoomReservations.Application.Tests.Services;

public class ReservationServiceHappyPathTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IRoomRepository> _roomRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly CreateReservationRequestValidator _validator = new();
    private readonly ReservationLockService _lockService = new();

    private ReservationService CreateService() => new(
        _reservationRepoMock.Object, _roomRepoMock.Object, _uowMock.Object, _validator, _lockService);

    [Fact]
    public async Task CreateAsync_RespectsExact30MinBuffer_DoesNotThrow()
    {
        var req = ValidRequest(start: new TimeOnly(12, 30), end: new TimeOnly(14, 0));
        var room = CreateRoom();
        var existing = CreateExistingReservation(req.RoomId, req.Date,
            new TimeOnly(10, 0), new TimeOnly(12, 0));

        _roomRepoMock.Setup(r => r.GetByIdAsync(req.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _reservationRepoMock.Setup(r => r.GetByRoomAndDateAsync(req.RoomId, req.Date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { existing });
        _reservationRepoMock.Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation r, CancellationToken _) => { SetReservationId(r, 1); return r; });
        _reservationRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((int _, CancellationToken __) =>
            {
                var rr = new Reservation(req.RoomId, req.CustomerName, req.EventName,
                    req.GuestCount, req.Date, req.StartTime, req.EndTime);
                SetReservationRoom(rr, room);
                return rr;
            });

        var svc = CreateService();
        var act = async () => await svc.CreateAsync(req);
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetByDateAsync_ReturnsOrderedList()
    {
        var date = new DateOnly(2030, 6, 1);
        var room = CreateRoom();
        var r1 = new Reservation(1, "A", "Evento A", 10, date, new TimeOnly(14, 0), new TimeOnly(15, 0));
        var r2 = new Reservation(1, "B", "Evento B", 10, date, new TimeOnly(10, 0), new TimeOnly(11, 0));
        SetReservationRoom(r1, room);
        SetReservationRoom(r2, room);

        _reservationRepoMock.Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { r1, r2 });

        var svc = CreateService();
        var result = await svc.GetByDateAsync(date);

        result.Should().HaveCount(2);
        result[0].StartTime.Should().Be(new TimeOnly(10, 0));
        result[1].StartTime.Should().Be(new TimeOnly(14, 0));
    }

    [Fact]
    public async Task GetByDateAsync_NoReservations_ReturnsEmptyList()
    {
        var date = new DateOnly(2030, 6, 1);
        _reservationRepoMock.Setup(r => r.GetByDateAsync(date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());

        var svc = CreateService();
        var result = await svc.GetByDateAsync(date);

        result.Should().BeEmpty();
    }
}
