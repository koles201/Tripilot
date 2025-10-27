using Microsoft.EntityFrameworkCore;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data;

/// <summary>
/// Application database context for Entity Framework Core
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    /// <summary>
    /// Override SaveChangesAsync to handle auditable entities
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Handle auditable entities
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = "System"; // TODO: Get from current user context
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = "System"; // TODO: Get from current user context
                    break;
            }
        }

        // Dispatch domain events before saving
        var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        var result = await base.SaveChangesAsync(cancellationToken);

        // TODO: Publish domain events via MediatR after successful save
        // This will be implemented when we add MediatR publisher

        return result;
    }
}
