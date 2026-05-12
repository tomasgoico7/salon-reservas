using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoomReservations.Infrastructure.Persistence;

namespace RoomReservations.Api.Tests.Helpers;

public sealed class ApiTestFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ReservationsDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<ReservationsDbContext>(opts =>
                opts.UseInMemoryDatabase(_dbName));
        });
    }
}
