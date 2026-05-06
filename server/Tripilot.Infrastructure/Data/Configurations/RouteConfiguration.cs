using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Route
/// </summary>
public class RouteConfiguration : IEntityTypeConfiguration<Route>
{
    public void Configure(EntityTypeBuilder<Route> builder)
    {
        builder.ToTable("Routes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(r => r.Difficulty)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.Privacy)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.EstimatedDuration)
            .IsRequired();

        builder.Property(r => r.TotalDistance)
            .HasPrecision(10, 2);

        builder.Property(r => r.ImageUrl)
            .HasMaxLength(1000);

        builder.Property(r => r.ShareToken)
            .HasMaxLength(50);

        builder.Property(r => r.IsEmbeddable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.AverageRating)
            .HasPrecision(3, 2)
            .HasDefaultValue(0);

        builder.Property(r => r.ReviewCount)
            .HasDefaultValue(0);

        builder.Property(r => r.ViewCount)
            .HasDefaultValue(0);

        builder.Property(r => r.FavoriteCount)
            .HasDefaultValue(0);

        builder.Property(r => r.ShareCount)
            .HasDefaultValue(0);

        builder.Property(r => r.Tags)
            .HasMaxLength(1000);

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.IsFeatured)
            .IsRequired()
            .HasDefaultValue(false);

        // Configure relationship with User (Creator)
        builder.HasOne(r => r.Creator)
            .WithMany()
            .HasForeignKey(r => r.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Audit fields
        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.CreatedBy)
            .HasMaxLength(256);

        builder.Property(r => r.ModifiedBy)
            .HasMaxLength(256);

        // Indexes
        builder.HasIndex(r => r.Name);
        builder.HasIndex(r => r.CreatorId);
        builder.HasIndex(r => r.IsActive);
        builder.HasIndex(r => r.Difficulty);
        builder.HasIndex(r => r.Privacy);
        builder.HasIndex(r => r.AverageRating);
        builder.HasIndex(r => r.IsFeatured);
        builder.HasIndex(r => r.ShareToken)
            .IsUnique()
            .HasFilter("[ShareToken] IS NOT NULL");
    }
}
