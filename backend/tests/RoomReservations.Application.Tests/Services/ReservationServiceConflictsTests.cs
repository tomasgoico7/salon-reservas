using FluentAssertions;
using Moq;
using RoomReservations.Application.Services;
using RoomReservations.Application.Validators;
using RoomReservations.Domain.Entities;
using RoomReservations.Domain.Exceptions;
using RoomReservations.Domain.Repositories;
using static RoomReservations.Application.Tests.Services.TestHelpers;

namespace RoomReservations.Application.Tests.Services;

public class ReservationServiceConflictsTests
{
    private readonly Mock<IReservationRepository> _reservationRepoMock = new();
    private readonly Mock<IRoomRepository> _roomRepoMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly CreateReservationRequestValidator _validator = new();
    private readonly ReservationLockService _lockService = new();

    private ReservationService CreateService() => new(
        _reservationRepoMock.Object,
        _roomRepoMock.Object,
        _uowMock.Object,
        _validator,
        _lockService);

    [Fact]
    public async Task CreateAsync_DirectOverlap_ThrowsConflict()
    {
        var req = ValidRequest(start: new TimeOnly(11, 0), end: new TimeOnly(13, 0));
        var room = CreateRoom();
        var existing = CreateExistingReservation(req.RoomId, req.Date,
            new TimeOnly(10, 0), new TimeOnly(12, 0));

        _roomRepoMock.Setup(r => r.GetByIdAsync(req.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _reservationRepoMock.Setup(r => r.GetByRoomAndDateAsync(req.RoomId, req.Date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { existing });

        var svc = CreateService();

        var act = async () => await svc.CreateAsync(req);

        await act.Should().ThrowAsync<ReservationConflictException>();
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_StartsRightWhenOtherEnds_ThrowsConflict()
    {
        var req = ValidRequest(start: new TimeOnly(12, 0), end: new TimeOnly(14, 0));
        var room = CreateRoom();
        var existing = CreateExistingReservation(req.RoomId, req.Date,
            new TimeOnly(10, 0), new TimeOnly(12, 0));

        _roomRepoMock.Setup(r => r.GetByIdAsync(req.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _reservationRepoMock.Setup(r => r.GetByRoomAndDateAsync(req.RoomId, req.Date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { existing });

        var svc = CreateService();

        var act = async () => await svc.CreateAsync(req);

        await act.Should().ThrowAsync<ReservationConflictException>();
    }

    [Fact]
    public async Task CreateAsync_LessThan30MinFromOther_ThrowsConflict()
    {
        var req = ValidRequest(start: new TimeOnly(12, 15), end: new TimeOnly(14, 0));
        var room = CreateRoom();
        var existing = CreateExistingReservation(req.RoomId, req.Date,
            new TimeOnly(10, 0), new TimeOnly(12, 0));

        _roomRepoMock.Setup(r => r.GetByIdAsync(req.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _reservationRepoMock.Setup(r => r.GetByRoomAndDateAsync(req.RoomId, req.Date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { existing });

        var svc = CreateService();

        var act = async () => await svc.CreateAsync(req);

        await act.Should().ThrowAsync<ReservationConflictException>();
    }
}
