using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Review entity
/// </summary>
public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        // Primary key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(r => r.OverallRating)
            .IsRequired()
            .HasPrecision(3, 2);

        builder.Property(r => r.CleanlinessRating)
            .HasPrecision(3, 2);

        builder.Property(r => r.ServiceRating)
            .HasPrecision(3, 2);

        builder.Property(r => r.ValueRating)
            .HasPrecision(3, 2);

        builder.Property(r => r.LocationRating)
            .HasPrecision(3, 2);

        builder.Property(r => r.FlagReason)
            .HasMaxLength(500);

        builder.Property(r => r.OwnerResponse)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Place)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.PlaceId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasOne(r => r.Route)
            .WithMany(r => r.Reviews)
            .HasForeignKey(r => r.RouteId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        // Constraints - must have either PlaceId or RouteId, but not both
        builder.HasCheckConstraint(
            "CK_Review_PlaceOrRoute",
            "(\"PlaceId\" IS NOT NULL AND \"RouteId\" IS NULL) OR (\"PlaceId\" IS NULL AND \"RouteId\" IS NOT NULL)"
        );

        // Indexes
        builder.HasIndex(r => r.PlaceId)
            .HasDatabaseName("IX_Reviews_PlaceId");

        builder.HasIndex(r => r.RouteId)
            .HasDatabaseName("IX_Reviews_RouteId");

        builder.HasIndex(r => r.ReviewerId)
            .HasDatabaseName("IX_Reviews_ReviewerId");

        builder.HasIndex(r => new { r.PlaceId, r.ReviewerId })
            .HasDatabaseName("IX_Reviews_PlaceId_ReviewerId")
            .IsUnique();

        builder.HasIndex(r => new { r.RouteId, r.ReviewerId })
            .HasDatabaseName("IX_Reviews_RouteId_ReviewerId")
            .IsUnique();

        builder.HasIndex(r => r.IsActive)
            .HasDatabaseName("IX_Reviews_IsActive");

        builder.HasIndex(r => r.CreatedAt)
            .HasDatabaseName("IX_Reviews_CreatedAt");

        builder.HasIndex(r => r.OverallRating)
            .HasDatabaseName("IX_Reviews_OverallRating");
    }
}
