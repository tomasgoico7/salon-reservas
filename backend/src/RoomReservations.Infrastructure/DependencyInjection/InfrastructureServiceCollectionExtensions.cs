using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoomReservations.Domain.Repositories;
using RoomReservations.Infrastructure.Persistence;
using RoomReservations.Infrastructure.Repositories;

namespace RoomReservations.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<ReservationsDbContext>(options =>
            options.UseInMemoryDatabase("RoomReservationsDb"));

        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
