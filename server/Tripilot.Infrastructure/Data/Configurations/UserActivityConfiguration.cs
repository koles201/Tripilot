using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for UserActivity
/// </summary>
public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.ToTable("UserActivities");

        builder.HasKey(ua => ua.Id);

        builder.Property(ua => ua.ActivityType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(ua => ua.EntityType)
            .HasMaxLength(100);

        builder.Property(ua => ua.Metadata)
            .HasMaxLength(2000);

        builder.Property(ua => ua.IsVisible)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure relationship with User
        builder.HasOne(ua => ua.User)
            .WithMany()
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Audit fields
        builder.Property(ua => ua.CreatedAt)
            .IsRequired();

        builder.Property(ua => ua.CreatedBy)
            .HasMaxLength(256);

        builder.Property(ua => ua.ModifiedBy)
            .HasMaxLength(256);

        // Indexes
        builder.HasIndex(ua => ua.UserId);
        builder.HasIndex(ua => ua.ActivityType);
        builder.HasIndex(ua => ua.CreatedAt);
        builder.HasIndex(ua => new { ua.UserId, ua.CreatedAt });
    }
}
