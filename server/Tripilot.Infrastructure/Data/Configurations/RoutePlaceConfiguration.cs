using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for RoutePlace junction table
/// </summary>
public class RoutePlaceConfiguration : IEntityTypeConfiguration<RoutePlace>
{
    public void Configure(EntityTypeBuilder<RoutePlace> builder)
    {
        builder.ToTable("RoutePlaces");

        // Composite primary key
        builder.HasKey(rp => new { rp.RouteId, rp.PlaceId });

        builder.Property(rp => rp.Order)
            .IsRequired();

        builder.Property(rp => rp.Notes)
            .HasMaxLength(500);

        builder.Property(rp => rp.EstimatedTimeAtPlace);

        builder.Property(rp => rp.CreatedAt)
            .IsRequired();

        // Configure relationships
        builder.HasOne(rp => rp.Route)
            .WithMany(r => r.RoutePlaces)
            .HasForeignKey(rp => rp.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Place)
            .WithMany()
            .HasForeignKey(rp => rp.PlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(rp => rp.RouteId);
        builder.HasIndex(rp => rp.PlaceId);
        builder.HasIndex(rp => new { rp.RouteId, rp.Order });
    }
}
