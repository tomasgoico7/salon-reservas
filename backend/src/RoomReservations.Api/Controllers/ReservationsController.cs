using Microsoft.AspNetCore.Mvc;
using RoomReservations.Application.DTOs;
using RoomReservations.Application.Interfaces;

namespace RoomReservations.Api.Controllers;

/// <summary>
/// Endpoints para gestion de reservas de salones para eventos infantiles.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    /// <summary>
    /// Crea una nueva reserva.
    /// </summary>
    /// <remarks>
    /// Reglas validadas:
    /// - El salon debe existir y estar activo.
    /// - El horario debe estar entre 09:00 y 18:00.
    /// - La hora de fin debe ser mayor que la hora de inicio.
    /// - No debe haber superposicion con otra reserva del mismo salon ese dia.
    /// - Debe respetarse un intervalo minimo de 30 minutos entre reservas consecutivas.
    /// - La cantidad de invitados no puede superar la capacidad del salon.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateReservationRequest request,
        CancellationToken ct)
    {
        var created = await _reservationService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetByDate), new { date = created.Date }, created);
    }

    /// <summary>
    /// Obtiene todas las reservas para una fecha especifica.
    /// </summary>
    /// <param name="date">Fecha en formato yyyy-MM-dd</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ReservationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByDate(
        [FromQuery] DateOnly date,
        CancellationToken ct)
    {
        var reservations = await _reservationService.GetByDateAsync(date, ct);
        return Ok(reservations);
    }
}
