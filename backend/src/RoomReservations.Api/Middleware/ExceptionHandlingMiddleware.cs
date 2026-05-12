using System.Net;
using System.Text.Json;
using FluentValidation;
using RoomReservations.Application.DTOs;
using RoomReservations.Domain.Exceptions;

namespace RoomReservations.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (status, title, errors) = MapException(ex);

        if (status >= 500)
            _logger.LogError(ex, "Error no controlado en la API");
        else
            _logger.LogWarning(ex, "Error controlado: {Message}", ex.Message);

        var payload = new ErrorResponse
        {
            Status = status,
            Title = title,
            Detail = status >= 500
                ? "Ha ocurrido un error interno. Intente nuevamente mas tarde."
                : ex.Message,
            Errors = errors
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }

    private static (int status, string title, IDictionary<string, string[]>? errors) MapException(Exception ex)
    {
        return ex switch
        {
            ValidationException ve => (
                (int)HttpStatusCode.BadRequest,
                "Errores de validacion",
                ve.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                    as IDictionary<string, string[]>),

            NotFoundException => (
                (int)HttpStatusCode.NotFound,
                "Recurso no encontrado",
                null),

            ReservationConflictException => (
                (int)HttpStatusCode.Conflict,
                "Conflicto de reserva",
                null),

            DomainException => (
                (int)HttpStatusCode.BadRequest,
                "Regla de negocio violada",
                null),

            ArgumentException => (
                (int)HttpStatusCode.BadRequest,
                "Solicitud invalida",
                null),

            _ => (
                (int)HttpStatusCode.InternalServerError,
                "Error interno del servidor",
                null)
        };
    }
}
