using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Infrastructure.Data.Configurations;

public class PlaceClaimConfiguration : IEntityTypeConfiguration<PlaceClaim>
{
    public void Configure(EntityTypeBuilder<PlaceClaim> builder)
    {
        builder.ToTable("PlaceClaims");

        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.ClaimReason)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(pc => pc.DocumentUrls)
            .HasMaxLength(4000);

        builder.Property(pc => pc.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(pc => pc.AdminDecisionReason)
            .HasMaxLength(2000);

        builder.Property(pc => pc.SubmittedAt)
            .IsRequired();

        builder.HasOne(pc => pc.Place)
            .WithMany()
            .HasForeignKey(pc => pc.PlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pc => pc.Claimant)
            .WithMany()
            .HasForeignKey(pc => pc.ClaimantUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pc => pc.BusinessProfile)
            .WithMany()
            .HasForeignKey(pc => pc.BusinessProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pc => pc.ReviewedByAdmin)
            .WithMany()
            .HasForeignKey(pc => pc.ReviewedByAdminId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(pc => pc.PlaceId);
        builder.HasIndex(pc => pc.ClaimantUserId);
        builder.HasIndex(pc => pc.Status);
        builder.HasIndex(pc => new { pc.PlaceId, pc.Status });
    }
}
