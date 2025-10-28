using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tripilot.Application.Common.Interfaces;
using Tripilot.Application.Interfaces;
using Tripilot.Infrastructure.Data;
using Tripilot.Infrastructure.Repositories;
using Tripilot.Infrastructure.Services;
using Tripilot.Shared.Settings;

namespace Tripilot.Infrastructure;

/// <summary>
/// Dependency injection configuration for Infrastructure layer
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Add DbContext with PostgreSQL
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                connectionString,
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                });

            // Enable detailed errors in development
            var detailedErrors = configuration["DetailedErrors"];
            if (detailedErrors != null && bool.Parse(detailedErrors))
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            }
        });

        // Add repositories and unit of work
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Add database seeder
        services.AddScoped<DatabaseSeeder>();

        // Add authentication services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

    /// <summary>
    /// Ensures the database is created and migrations are applied
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="seedDevelopmentData">Whether to seed development data</param>
    /// <returns>Task</returns>
    public static async Task InitializeDatabaseAsync(
        IServiceProvider serviceProvider,
        bool seedDevelopmentData = false)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Apply pending migrations
        await context.Database.MigrateAsync();

        // Seed data if requested
        if (seedDevelopmentData)
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedDevelopmentDataAsync();
        }
    }
}
