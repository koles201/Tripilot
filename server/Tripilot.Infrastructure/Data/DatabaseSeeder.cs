using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tripilot.Infrastructure.Data;

namespace Tripilot.Infrastructure.Data;

/// <summary>
/// Database seeder for development data
/// </summary>
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Seeds the database with development data
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Check if database already has data
            // When we add entities, we'll check if they exist before seeding
            // Example: if (await _context.Users.AnyAsync()) return;

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    /// <summary>
    /// Seeds development data specifically for development environment
    /// </summary>
    public async Task SeedDevelopmentDataAsync()
    {
        await SeedAsync();

        // Add development-specific data here
        // Example: test users, sample places, etc.

        await _context.SaveChangesAsync();
        _logger.LogInformation("Development data seeded successfully");
    }
}
