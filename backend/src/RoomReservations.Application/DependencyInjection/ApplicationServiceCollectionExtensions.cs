using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RoomReservations.Application.Interfaces;
using RoomReservations.Application.Services;

namespace RoomReservations.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IReservationLockService, ReservationLockService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IRoomService, RoomService>();

        services.AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly);

        return services;
    }
}
