using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for RouteCollection
/// </summary>
public class RouteCollectionConfiguration : IEntityTypeConfiguration<RouteCollection>
{
    public void Configure(EntityTypeBuilder<RouteCollection> builder)
    {
        builder.ToTable("RouteCollections");

        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(rc => rc.Description)
            .HasMaxLength(1000);

        builder.Property(rc => rc.IsPublic)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(rc => rc.CoverImageUrl)
            .HasMaxLength(1000);

        // Configure relationship with User
        builder.HasOne(rc => rc.User)
            .WithMany()
            .HasForeignKey(rc => rc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(rc => rc.CreatedAt)
            .IsRequired();

        builder.Property(rc => rc.CreatedBy)
            .HasMaxLength(256);

        builder.Property(rc => rc.ModifiedBy)
            .HasMaxLength(256);

        // Indexes
        builder.HasIndex(rc => rc.UserId);
        builder.HasIndex(rc => rc.IsPublic);
        builder.HasIndex(rc => rc.Name);
    }
}
