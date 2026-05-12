using FluentAssertions;
using FluentValidation;
using Moq;
using RoomReservations.Application.DTOs;
using RoomReservations.Application.Services;
using RoomReservations.Application.Validators;
using RoomReservations.Domain.Entities;
using RoomReservations.Domain.Exceptions;
using RoomReservations.Domain.Repositories;
using static RoomReservations.Application.Tests.Services.TestHelpers;

namespace RoomReservations.Application.Tests.Services;

public class ReservationServiceCreateTests
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
    public async Task CreateAsync_ValidData_CreatesReservationAndReturnsResponse()
    {
        var req = ValidRequest();
        var room = CreateRoom();

        _roomRepoMock.Setup(r => r.GetByIdAsync(req.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        _reservationRepoMock.Setup(r => r.GetByRoomAndDateAsync(req.RoomId, req.Date, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());

        _reservationRepoMock.Setup(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation r, CancellationToken _) =>
            {
                SetReservationId(r, 100);
                SetReservationRoom(r, room);
                return r;
            });

        _reservationRepoMock.Setup(r => r.GetByIdAsync(100, It.IsAny<CancellationToken>()))
            .ReturnsAsync((int id, CancellationToken _) =>
            {
                var r = new Reservation(req.RoomId, req.CustomerName, req.EventName,
                    req.GuestCount, req.Date, req.StartTime, req.EndTime);
                SetReservationId(r, id);
                SetReservationRoom(r, room);
                return r;
            });

        var svc = CreateService();

        var response = await svc.CreateAsync(req);

        response.Should().NotBeNull();
        response.Id.Should().Be(100);
        response.RoomId.Should().Be(req.RoomId);
        response.RoomName.Should().Be(room.Name);
        response.CustomerName.Should().Be(req.CustomerName);

        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _reservationRepoMock.Verify(r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ThrowsValidationException()
    {
        var req = ValidRequest();
        req.CustomerName = "";

        var svc = CreateService();

        var act = async () => await svc.CreateAsync(req);

        await act.Should().ThrowAsync<ValidationException>();
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_NonExistingRoom_ThrowsNotFoundException()
    {
        var req = ValidRequest(roomId: 999);

        _roomRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        var svc = CreateService();

        var act = async () => await svc.CreateAsync(req);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_InactiveRoom_ThrowsDomainException()
    {
        var req = ValidRequest();
        var room = CreateRoom(active: false);

        _roomRepoMock.Setup(r => r.GetByIdAsync(req.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        var svc = CreateService();

        var act = async () => await svc.CreateAsync(req);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*no esta activo*");
    }

    [Fact]
    public async Task CreateAsync_GuestCountExceedsCapacity_ThrowsDomainException()
    {
        var req = ValidRequest(guestCount: 200);
        var room = CreateRoom(capacity: 80);

        _roomRepoMock.Setup(r => r.GetByIdAsync(req.RoomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);

        var svc = CreateService();

        var act = async () => await svc.CreateAsync(req);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*supera la capacidad maxima*");
    }
}
