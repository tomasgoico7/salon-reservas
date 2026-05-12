using RoomReservations.Application.DTOs;
using RoomReservations.Application.Interfaces;
using RoomReservations.Application.Mappings;
using RoomReservations.Domain.Repositories;

namespace RoomReservations.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;

    public RoomService(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<IReadOnlyList<RoomResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var rooms = await _roomRepository.GetAllAsync(ct);
        return rooms.Select(RoomMapper.ToResponse).ToList();
    }
}
