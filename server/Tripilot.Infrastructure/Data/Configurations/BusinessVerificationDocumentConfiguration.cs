using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data.Configurations;

public class BusinessVerificationDocumentConfiguration : IEntityTypeConfiguration<BusinessVerificationDocument>
{
    public void Configure(EntityTypeBuilder<BusinessVerificationDocument> builder)
    {
        builder.ToTable("BusinessVerificationDocuments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DocumentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.FileUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.MimeType)
            .HasMaxLength(100);

        builder.Property(d => d.ReviewNotes)
            .HasMaxLength(2000);

        builder.HasIndex(d => d.BusinessProfileId);

        builder.HasOne(d => d.BusinessProfile)
            .WithMany()
            .HasForeignKey(d => d.BusinessProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
