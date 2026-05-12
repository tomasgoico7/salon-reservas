using Microsoft.AspNetCore.Mvc;
using RoomReservations.Application.DTOs;
using RoomReservations.Application.Interfaces;

namespace RoomReservations.Api.Controllers;

/// <summary>
/// Endpoints de consulta de salones disponibles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    /// <summary>
    /// Obtiene el listado de salones disponibles.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<RoomResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var rooms = await _roomService.GetAllAsync(ct);
        return Ok(rooms);
    }
}
