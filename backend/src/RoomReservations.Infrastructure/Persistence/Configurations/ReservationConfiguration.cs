using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservations.Domain.Entities;

namespace RoomReservations.Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).ValueGeneratedOnAdd();

        builder.Property(r => r.CustomerName).IsRequired().HasMaxLength(120);
        builder.Property(r => r.EventName).IsRequired().HasMaxLength(150);
        builder.Property(r => r.GuestCount).IsRequired();
        builder.Property(r => r.Date).IsRequired();
        builder.Property(r => r.StartTime).IsRequired();
        builder.Property(r => r.EndTime).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasOne(r => r.Room)
            .WithMany()
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.RoomId, r.Date });
    }
}
