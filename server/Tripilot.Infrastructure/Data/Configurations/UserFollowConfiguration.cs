using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for UserFollow
/// </summary>
public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
{
    public void Configure(EntityTypeBuilder<UserFollow> builder)
    {
        builder.ToTable("UserFollows");

        builder.HasKey(uf => uf.Id);

        builder.Property(uf => uf.IsNotified)
            .IsRequired()
            .HasDefaultValue(false);

        // Configure relationship with Follower
        builder.HasOne(uf => uf.Follower)
            .WithMany()
            .HasForeignKey(uf => uf.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship with Following
        builder.HasOne(uf => uf.Following)
            .WithMany()
            .HasForeignKey(uf => uf.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Audit fields
        builder.Property(uf => uf.CreatedAt)
            .IsRequired();

        builder.Property(uf => uf.CreatedBy)
            .HasMaxLength(256);

        builder.Property(uf => uf.ModifiedBy)
            .HasMaxLength(256);

        // Indexes
        builder.HasIndex(uf => uf.FollowerId);
        builder.HasIndex(uf => uf.FollowingId);
        builder.HasIndex(uf => new { uf.FollowerId, uf.FollowingId })
            .IsUnique();
    }
}
