using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using RoomReservations.Api.Middleware;
using RoomReservations.Application.DTOs;
using RoomReservations.Domain.Exceptions;

namespace RoomReservations.Api.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private static async Task<(int statusCode, ErrorResponse body)> Invoke(Exception ex)
    {
        var context = new DefaultHttpContext();
        var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        RequestDelegate next = _ => throw ex;
        var mw = new ExceptionHandlingMiddleware(next, NullLogger<ExceptionHandlingMiddleware>.Instance);

        await mw.InvokeAsync(context);

        responseStream.Position = 0;
        var json = Encoding.UTF8.GetString(responseStream.ToArray());
        var body = JsonSerializer.Deserialize<ErrorResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        return (context.Response.StatusCode, body);
    }

    [Fact]
    public async Task InvokeAsync_DomainException_ReturnsBadRequest400()
    {
        var (status, body) = await Invoke(new DomainException("Regla violada"));
        status.Should().Be((int)HttpStatusCode.BadRequest);
        body.Status.Should().Be(400);
    }

    [Fact]
    public async Task InvokeAsync_NotFoundException_ReturnsNotFound404()
    {
        var (status, body) = await Invoke(NotFoundException.For("Salon", 999));
        status.Should().Be((int)HttpStatusCode.NotFound);
        body.Status.Should().Be(404);
    }

    [Fact]
    public async Task InvokeAsync_ReservationConflictException_ReturnsConflict409()
    {
        var (status, body) = await Invoke(new ReservationConflictException("Solapado"));
        status.Should().Be((int)HttpStatusCode.Conflict);
        body.Status.Should().Be(409);
    }

    [Fact]
    public async Task InvokeAsync_ValidationException_ReturnsBadRequestWithErrors()
    {
        var failures = new List<ValidationFailure>
        {
            new("CustomerName", "Es obligatorio."),
            new("GuestCount", "Debe ser mayor a cero.")
        };

        var (status, body) = await Invoke(new ValidationException(failures));

        status.Should().Be((int)HttpStatusCode.BadRequest);
        body.Errors.Should().NotBeNull();
        body.Errors!.Should().ContainKey("CustomerName");
        body.Errors.Should().ContainKey("GuestCount");
    }

    [Fact]
    public async Task InvokeAsync_GenericException_ReturnsInternalServerError500WithRedactedDetail()
    {
        var (status, body) = await Invoke(new InvalidOperationException("Algo se rompio"));
        status.Should().Be((int)HttpStatusCode.InternalServerError);
        body.Status.Should().Be(500);
        body.Detail.Should().NotContain("Algo se rompio");
    }
}
