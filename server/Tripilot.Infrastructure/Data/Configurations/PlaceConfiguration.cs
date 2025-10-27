using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;
using Tripilot.Domain.ValueObjects;

namespace Tripilot.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Place
/// </summary>
public class PlaceConfiguration : IEntityTypeConfiguration<Place>
{
    public void Configure(EntityTypeBuilder<Place> builder)
    {
        builder.ToTable("Places");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Configure Location as owned entity
        builder.OwnsOne(p => p.Location, location =>
        {
            location.Property(l => l.Latitude)
                .IsRequired()
                .HasPrecision(10, 7);

            location.Property(l => l.Longitude)
                .IsRequired()
                .HasPrecision(10, 7);

            location.Property(l => l.Address)
                .HasMaxLength(500);

            location.Property(l => l.City)
                .HasMaxLength(100);

            location.Property(l => l.Country)
                .HasMaxLength(100);

            location.Property(l => l.PostalCode)
                .HasMaxLength(20);
        });

        // Configure ContactInfo as owned entity
        builder.OwnsOne(p => p.ContactInfo, contact =>
        {
            contact.Property(c => c.Phone)
                .HasMaxLength(50);

            contact.Property(c => c.Email)
                .HasMaxLength(256);

            contact.Property(c => c.Website)
                .HasMaxLength(500);
        });

        // Configure OperatingHours as owned entity
        builder.OwnsOne(p => p.OperatingHours, hours =>
        {
            hours.Property(h => h.DaysOfWeek)
                .HasMaxLength(100);

            hours.Property(h => h.SpecialNotes)
                .HasMaxLength(500);
        });

        builder.Property(p => p.AverageRating)
            .HasPrecision(3, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.ReviewCount)
            .HasDefaultValue(0);

        builder.Property(p => p.PriceLevel)
            .HasDefaultValue(null);

        builder.Property(p => p.Amenities)
            .HasMaxLength(1000);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(1000);

        builder.Property(p => p.GalleryImages)
            .HasMaxLength(4000);

        builder.Property(p => p.IsVerified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.ViewCount)
            .HasDefaultValue(0);

        // Configure relationship with User (Owner)
        builder.HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Audit fields
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasMaxLength(256);

        builder.Property(p => p.ModifiedBy)
            .HasMaxLength(256);

        // Indexes
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.AverageRating);
        builder.HasIndex(p => p.OwnerId);
    }
}
