using FluentValidation;
using RoomReservations.Application.DTOs;
using RoomReservations.Application.Interfaces;
using RoomReservations.Application.Mappings;
using RoomReservations.Domain.Entities;
using RoomReservations.Domain.Exceptions;
using RoomReservations.Domain.Repositories;

namespace RoomReservations.Application.Services;

/// <summary>
/// Caso de uso de reservas: crea reservas validando reglas de dominio (incluyendo
/// solapamientos con buffer obligatorio) y consulta reservas por fecha.
/// </summary>
public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateReservationRequest> _validator;
    private readonly IReservationLockService _lockService;

    public ReservationService(
        IReservationRepository reservationRepository,
        IRoomRepository roomRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateReservationRequest> validator,
        IReservationLockService lockService)
    {
        _reservationRepository = reservationRepository;
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _lockService = lockService;
    }

    public async Task<ReservationResponse> CreateAsync(CreateReservationRequest request, CancellationToken ct = default)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            throw new ValidationException(validation.Errors);

        var room = await _roomRepository.GetByIdAsync(request.RoomId, ct)
            ?? throw NotFoundException.For("Salon", request.RoomId);

        if (!room.IsActive)
            throw new DomainException($"El salon '{room.Name}' no esta activo.");

        if (request.GuestCount > room.MaxCapacity)
            throw new DomainException(
                $"La cantidad de invitados ({request.GuestCount}) supera la capacidad maxima " +
                $"del salon '{room.Name}' ({room.MaxCapacity}).");

        var newReservation = new Reservation(
            roomId: request.RoomId,
            customerName: request.CustomerName,
            eventName: request.EventName,
            guestCount: request.GuestCount,
            date: request.Date,
            startTime: request.StartTime,
            endTime: request.EndTime);

        // Lock por (room, date) para evitar race condition en el chequeo de solapamiento
        using var _ = await _lockService.AcquireAsync(request.RoomId, request.Date, ct);

        var sameDayReservations = await _reservationRepository
            .GetByRoomAndDateAsync(request.RoomId, request.Date, ct);

        var conflict = sameDayReservations.FirstOrDefault(r => newReservation.ConflictsWith(r));
        if (conflict is not null)
        {
            throw new ReservationConflictException(
                $"La reserva se superpone con otra existente o no respeta el intervalo minimo de 30 minutos.");
        }

        var created = await _reservationRepository.AddAsync(newReservation, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var withRoom = await _reservationRepository.GetByIdAsync(created.Id, ct) ?? created;
        return ReservationMapper.ToResponse(withRoom);
    }

    public async Task<IReadOnlyList<ReservationResponse>> GetByDateAsync(
        DateOnly date,
        CancellationToken ct = default)
    {
        var reservations = await _reservationRepository.GetByDateAsync(date, ct);
        return reservations
            .OrderBy(r => r.RoomId)
            .ThenBy(r => r.StartTime)
            .Select(ReservationMapper.ToResponse)
            .ToList();
    }
}
