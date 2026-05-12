using FluentAssertions;
using RoomReservations.Application.DTOs;
using RoomReservations.Application.Validators;

namespace RoomReservations.Application.Tests.Validators;

public class CreateReservationRequestValidatorTests
{
    private readonly CreateReservationRequestValidator _validator = new();

    private static CreateReservationRequest ValidRequest() => new()
    {
        RoomId = 1,
        CustomerName = "Maria Lopez",
        EventName = "Cumpleanios Lucas",
        GuestCount = 25,
        Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
        StartTime = new TimeOnly(10, 0),
        EndTime = new TimeOnly(12, 0)
    };

    [Fact]
    public void Validate_ValidRequest_HasNoErrors()
    {
        var result = _validator.Validate(ValidRequest());
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_RoomIdZero_HasError()
    {
        var req = ValidRequest();
        req.RoomId = 0;

        var result = _validator.Validate(req);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(req.RoomId));
    }

    [Fact]
    public void Validate_EmptyCustomerName_HasError()
    {
        var req = ValidRequest();
        req.CustomerName = "";

        var result = _validator.Validate(req);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(req.CustomerName));
    }

    [Fact]
    public void Validate_GuestCountZero_HasError()
    {
        var req = ValidRequest();
        req.GuestCount = 0;

        var result = _validator.Validate(req);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_PastDate_HasError()
    {
        var req = ValidRequest();
        req.Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        var result = _validator.Validate(req);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(req.Date));
    }

    [Fact]
    public void Validate_EndTimeBeforeStart_HasError()
    {
        var req = ValidRequest();
        req.StartTime = new TimeOnly(15, 0);
        req.EndTime = new TimeOnly(10, 0);

        var result = _validator.Validate(req);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_StartTimeOutOfRange_HasError()
    {
        var req = ValidRequest();
        req.StartTime = new TimeOnly(8, 0);
        req.EndTime = new TimeOnly(10, 0);

        var result = _validator.Validate(req);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_EndTimeOutOfRange_HasError()
    {
        var req = ValidRequest();
        req.StartTime = new TimeOnly(17, 0);
        req.EndTime = new TimeOnly(19, 0);

        var result = _validator.Validate(req);

        result.IsValid.Should().BeFalse();
    }
}
