using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for User
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(u => u.EmailVerified)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.EmailVerificationToken)
            .HasMaxLength(500);

        builder.Property(u => u.PasswordResetToken)
            .HasMaxLength(500);

        builder.Property(u => u.RefreshToken)
            .HasMaxLength(500);

        builder.Property(u => u.Bio)
            .HasMaxLength(500);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(1000);

        builder.Property(u => u.FollowerCount)
            .HasDefaultValue(0);

        builder.Property(u => u.FollowingCount)
            .HasDefaultValue(0);

        // Audit fields
        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(256);

        builder.Property(u => u.ModifiedBy)
            .HasMaxLength(256);

        // Indexes
        builder.HasIndex(u => u.RefreshToken);
        builder.HasIndex(u => u.EmailVerificationToken);
        builder.HasIndex(u => u.PasswordResetToken);
    }
}
