using Microsoft.EntityFrameworkCore;
using MediatR;
using Tripilot.Domain.Entities;

namespace Tripilot.Infrastructure.Data;

/// <summary>
/// Application database context for Entity Framework Core
/// </summary>
public class ApplicationDbContext : DbContext
{
    private readonly IServiceProvider? _serviceProvider;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IServiceProvider? serviceProvider = null)
        : base(options)
    {
        _serviceProvider = serviceProvider; // optional; mediator resolved lazily
    }

    // DbSets
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Place> Places { get; set; } = null!;
    public DbSet<Route> Routes { get; set; } = null!;
    public DbSet<RoutePlace> RoutePlaces { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<BusinessProfile> BusinessProfiles { get; set; } = null!;
    public DbSet<BusinessVerificationDocument> BusinessVerificationDocuments { get; set; } = null!;

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

        // Publish domain events via MediatR after successful save
        if (domainEvents.Count > 0 && _serviceProvider != null)
        {
            var mediator = _serviceProvider.GetService(typeof(IMediator)) as IMediator;
            if (mediator != null)
            {
                foreach (var domainEvent in domainEvents)
                {
                    await mediator.Publish(domainEvent, cancellationToken);
                }
            }
        }

        return result;
    }
}
