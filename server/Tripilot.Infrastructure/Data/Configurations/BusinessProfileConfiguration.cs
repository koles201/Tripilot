using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;

namespace Tripilot.Infrastructure.Data.Configurations;

public class BusinessProfileConfiguration : IEntityTypeConfiguration<BusinessProfile>
{
    public void Configure(EntityTypeBuilder<BusinessProfile> builder)
    {
        builder.ToTable("BusinessProfiles");

        builder.HasKey(bp => bp.Id);

        builder.Property(bp => bp.BusinessName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(bp => bp.Description)
            .HasMaxLength(2000);

        builder.Property(bp => bp.Address)
            .HasMaxLength(300);

        builder.Property(bp => bp.City)
            .HasMaxLength(100);

        builder.Property(bp => bp.Country)
            .HasMaxLength(100);

        builder.Property(bp => bp.PostalCode)
            .HasMaxLength(20);

        builder.Property(bp => bp.Phone)
            .HasMaxLength(50);

        builder.Property(bp => bp.Email)
            .HasMaxLength(200);

        builder.Property(bp => bp.Website)
            .HasMaxLength(300);

        builder.Property(bp => bp.LogoUrl)
            .HasMaxLength(500);

        builder.Property(bp => bp.BannerUrl)
            .HasMaxLength(500);

        builder.Property(bp => bp.VerificationStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(bp => bp.RejectionReason)
            .HasMaxLength(1000);

        builder.HasIndex(bp => bp.UserId).IsUnique(); // One profile per user

        builder.HasOne<User>()
            .WithMany() // No navigation collection on User yet
            .HasForeignKey(bp => bp.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
