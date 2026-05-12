using FluentValidation;
using RoomReservations.Application.DTOs;
using RoomReservations.Domain.Constants;

namespace RoomReservations.Application.Validators;

/// <summary>
/// Validador de entrada para la creacion de reservas.
/// </summary>
public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("Debe especificarse un RoomId valido.");

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage("El nombre del cliente es obligatorio.")
            .MaximumLength(120).WithMessage("El nombre del cliente no puede superar 120 caracteres.");

        RuleFor(x => x.EventName)
            .NotEmpty().WithMessage("El nombre del evento es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre del evento no puede superar 150 caracteres.");

        RuleFor(x => x.GuestCount)
            .GreaterThan(0).WithMessage("La cantidad de invitados debe ser mayor a cero.")
            .LessThanOrEqualTo(500).WithMessage("La cantidad de invitados no puede superar 500.");

        RuleFor(x => x.Date)
            .Must(NotPastDate)
            .WithMessage("La fecha de la reserva no puede ser anterior a hoy.");

        RuleFor(x => x.StartTime)
            .Must(h => h >= BusinessRules.OpeningTime)
            .WithMessage($"La hora de inicio no puede ser anterior a {BusinessRules.OpeningTime:HH\\:mm}.");

        RuleFor(x => x.EndTime)
            .Must(h => h <= BusinessRules.ClosingTime)
            .WithMessage($"La hora de fin no puede ser posterior a {BusinessRules.ClosingTime:HH\\:mm}.");

        RuleFor(x => x)
            .Must(x => x.EndTime > x.StartTime)
            .WithMessage("La hora de fin debe ser mayor que la hora de inicio.")
            .OverridePropertyName(nameof(CreateReservationRequest.EndTime));
    }

    private static bool NotPastDate(DateOnly date)
        => date >= DateOnly.FromDateTime(DateTime.UtcNow.Date);
}
