using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Favorite entity
/// </summary>
public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("Favorites");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .ValueGeneratedOnAdd();

        builder.Property(f => f.UserId)
            .IsRequired();

        builder.Property(f => f.PlaceId)
            .IsRequired();

        builder.Property(f => f.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        // Relationships
        builder.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Place)
            .WithMany()
            .HasForeignKey(f => f.PlaceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(f => f.UserId)
            .HasDatabaseName("IX_Favorites_UserId");

        builder.HasIndex(f => f.PlaceId)
            .HasDatabaseName("IX_Favorites_PlaceId");

        // Unique constraint to prevent duplicate favorites
        builder.HasIndex(f => new { f.UserId, f.PlaceId })
            .IsUnique()
            .HasDatabaseName("IX_Favorites_User_Place_Unique");

        builder.HasIndex(f => f.CreatedAt)
            .HasDatabaseName("IX_Favorites_CreatedAt");
    }
}
