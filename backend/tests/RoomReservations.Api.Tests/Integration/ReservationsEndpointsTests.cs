using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using RoomReservations.Api.Tests.Helpers;
using RoomReservations.Application.DTOs;

namespace RoomReservations.Api.Tests.Integration;

/// <summary>
/// Tests de integracion end-to-end levantando la API en memoria con WebApplicationFactory.
/// Cada test recibe su propia ApiTestFactory (y por lo tanto su propia DB limpia).
/// </summary>
public class ReservationsEndpointsTests : IDisposable
{
    private readonly ApiTestFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public ReservationsEndpointsTests()
    {
        _factory = new ApiTestFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private static CreateReservationRequest NewRequest(
        int roomId = 1,
        int days = 7,
        TimeOnly? start = null,
        TimeOnly? end = null) => new()
    {
        RoomId = roomId,
        CustomerName = "Cliente Test",
        EventName = "Cumple Test",
        GuestCount = 15,
        Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(days)),
        StartTime = start ?? new TimeOnly(10, 0),
        EndTime = end ?? new TimeOnly(12, 0)
    };

    [Fact]
    public async Task GET_Rooms_ReturnsAtLeast3ActiveRooms()
    {
        var resp = await _client.GetAsync("/api/rooms");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var rooms = await resp.Content.ReadFromJsonAsync<List<RoomResponse>>(JsonOpts);
        rooms.Should().NotBeNull();
        rooms!.Count.Should().BeGreaterThanOrEqualTo(3);
        rooms.Should().OnlyContain(s => s.IsActive);
    }

    [Fact]
    public async Task POST_Reservations_ValidData_Returns201()
    {
        var req = NewRequest(roomId: 1, days: 10);

        var resp = await _client.PostAsJsonAsync("/api/reservations", req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await resp.Content.ReadFromJsonAsync<ReservationResponse>(JsonOpts);
        created.Should().NotBeNull();
        created!.Id.Should().BeGreaterThan(0);
        created.RoomId.Should().Be(req.RoomId);
        created.CustomerName.Should().Be(req.CustomerName);
    }

    [Fact]
    public async Task POST_Reservations_EndTimeBeforeStart_Returns400()
    {
        var req = NewRequest(roomId: 2, days: 11);
        req.StartTime = new TimeOnly(15, 0);
        req.EndTime = new TimeOnly(10, 0);

        var resp = await _client.PostAsJsonAsync("/api/reservations", req);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_Reservations_NonExistingRoom_Returns404()
    {
        var req = NewRequest(roomId: 99999, days: 12);

        var resp = await _client.PostAsJsonAsync("/api/reservations", req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task POST_Reservations_Overlapping_ReturnsConflict409()
    {
        var uniqueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(20));

        var first = new CreateReservationRequest
        {
            RoomId = 3,
            CustomerName = "Cliente 1",
            EventName = "Evento 1",
            GuestCount = 10,
            Date = uniqueDate,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(12, 0)
        };

        var firstResp = await _client.PostAsJsonAsync("/api/reservations", first);
        firstResp.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = new CreateReservationRequest
        {
            RoomId = 3,
            CustomerName = "Cliente 2",
            EventName = "Evento 2",
            GuestCount = 10,
            Date = uniqueDate,
            StartTime = new TimeOnly(11, 0),
            EndTime = new TimeOnly(13, 0)
        };

        var secondResp = await _client.PostAsJsonAsync("/api/reservations", second);

        secondResp.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GET_ReservationsByDate_ReturnsReservationsForThatDate()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

        var req = new CreateReservationRequest
        {
            RoomId = 2,
            CustomerName = "Cliente Filtro",
            EventName = "Evento Filtro",
            GuestCount = 20,
            Date = date,
            StartTime = new TimeOnly(14, 0),
            EndTime = new TimeOnly(16, 0)
        };
        await _client.PostAsJsonAsync("/api/reservations", req);

        var resp = await _client.GetAsync($"/api/reservations?date={date:yyyy-MM-dd}");

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<ReservationResponse>>(JsonOpts);
        list.Should().NotBeNull();
        list!.Should().Contain(r => r.CustomerName == "Cliente Filtro");
    }

    [Fact]
    public async Task GET_Health_ReturnsOk()
    {
        var resp = await _client.GetAsync("/health");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
