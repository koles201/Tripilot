using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for RouteCollectionItem
/// </summary>
public class RouteCollectionItemConfiguration : IEntityTypeConfiguration<RouteCollectionItem>
{
    public void Configure(EntityTypeBuilder<RouteCollectionItem> builder)
    {
        builder.ToTable("RouteCollectionItems");

        builder.HasKey(rci => rci.Id);

        builder.Property(rci => rci.Order)
            .IsRequired();

        builder.Property(rci => rci.Note)
            .HasMaxLength(500);

        // Configure relationship with Collection
        builder.HasOne(rci => rci.Collection)
            .WithMany(rc => rc.Items)
            .HasForeignKey(rci => rci.CollectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with Route
        builder.HasOne(rci => rci.Route)
            .WithMany()
            .HasForeignKey(rci => rci.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(rci => rci.CreatedAt)
            .IsRequired();

        builder.Property(rci => rci.CreatedBy)
            .HasMaxLength(256);

        builder.Property(rci => rci.ModifiedBy)
            .HasMaxLength(256);

        // Indexes
        builder.HasIndex(rci => rci.CollectionId);
        builder.HasIndex(rci => rci.RouteId);
        builder.HasIndex(rci => new { rci.CollectionId, rci.RouteId })
            .IsUnique();
    }
}
