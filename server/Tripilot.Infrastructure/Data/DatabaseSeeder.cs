using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tripilot.Domain.Entities;
using Tripilot.Domain.Enums;
using Tripilot.Domain.ValueObjects;
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

        // Seed sample places if none exist
        if (!await _context.Places.AnyAsync())
        {
            await SeedPlacesAsync();
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Development data seeded successfully");
    }

    /// <summary>
    /// Seeds sample places for development
    /// </summary>
    private async Task SeedPlacesAsync()
    {
        var places = new List<Place>
        {
            new Place
            {
                Name = "The Louvre Museum",
                Description = "The world's largest art museum and a historic monument in Paris, France. Home to thousands of works of art, including the Mona Lisa.",
                Category = PlaceCategory.Museum,
                Location = Location.Create(48.8606, 2.3376, "Rue de Rivoli", "Paris", "France", "75001"),
                ContactInfo = ContactInfo.Create("+33 1 40 20 50 50", "info@louvre.fr", "https://www.louvre.fr"),
                OperatingHours = OperatingHours.Create(TimeSpan.FromHours(9), TimeSpan.FromHours(18), "Monday,Wednesday,Thursday,Friday,Saturday,Sunday", false, "Closed on Tuesdays"),
                AverageRating = 4.7m,
                ReviewCount = 15234,
                PriceLevel = 3,
                ImageUrl = "https://example.com/louvre.jpg",
                IsVerified = true,
                IsActive = true,
                ViewCount = 125000
            },
            new Place
            {
                Name = "Eiffel Tower",
                Description = "Iconic iron lattice tower on the Champ de Mars in Paris. One of the most recognizable structures in the world.",
                Category = PlaceCategory.HistoricalSite,
                Location = Location.Create(48.8584, 2.2945, "Champ de Mars, 5 Avenue Anatole", "Paris", "France", "75007"),
                ContactInfo = ContactInfo.Create("+33 892 70 12 39", null, "https://www.toureiffel.paris"),
                OperatingHours = OperatingHours.Create(TimeSpan.FromHours(9), TimeSpan.FromHours(23), "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday", false, null),
                AverageRating = 4.6m,
                ReviewCount = 98567,
                PriceLevel = 2,
                ImageUrl = "https://example.com/eiffel.jpg",
                IsVerified = true,
                IsActive = true,
                ViewCount = 250000
            },
            new Place
            {
                Name = "Le Jules Verne",
                Description = "Michelin-starred restaurant located on the second floor of the Eiffel Tower, offering French haute cuisine with spectacular views.",
                Category = PlaceCategory.Restaurant,
                Location = Location.Create(48.8584, 2.2945, "Avenue Gustave Eiffel", "Paris", "France", "75007"),
                ContactInfo = ContactInfo.Create("+33 1 45 55 61 44", "reservation@lejulesverne-paris.com", "https://www.lejulesverne-paris.com"),
                OperatingHours = OperatingHours.Create(TimeSpan.FromHours(12), TimeSpan.FromHours(21), "Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday", false, "Closed on Mondays"),
                AverageRating = 4.5m,
                ReviewCount = 2341,
                PriceLevel = 4,
                Amenities = "[\"Fine Dining\",\"City Views\",\"Reservations Required\",\"Dress Code\"]",
                ImageUrl = "https://example.com/jules-verne.jpg",
                IsVerified = true,
                IsActive = true,
                ViewCount = 15000
            },
            new Place
            {
                Name = "Café de Flore",
                Description = "Historic Parisian café in the Saint-Germain-des-Prés quarter. Famous for its intellectual and artistic clientele.",
                Category = PlaceCategory.Cafe,
                Location = Location.Create(48.8543, 2.3324, "172 Boulevard Saint-Germain", "Paris", "France", "75006"),
                ContactInfo = ContactInfo.Create("+33 1 45 48 55 26", null, "https://www.cafedeflore.fr"),
                OperatingHours = OperatingHours.Create(TimeSpan.FromHours(7), TimeSpan.FromHours(1), "Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday", false, null),
                AverageRating = 4.3m,
                ReviewCount = 5678,
                PriceLevel = 3,
                Amenities = "[\"Outdoor Seating\",\"WiFi\",\"Historic Site\"]",
                ImageUrl = "https://example.com/cafe-flore.jpg",
                IsVerified = true,
                IsActive = true,
                ViewCount = 32000
            },
            new Place
            {
                Name = "Central Park",
                Description = "Urban park in Manhattan, New York City. One of the most visited urban parks in the United States.",
                Category = PlaceCategory.Park,
                Location = Location.Create(40.7829, -73.9654, "Central Park", "New York", "USA", "10024"),
                ContactInfo = ContactInfo.Create("+1 212-310-6600", null, "https://www.centralparknyc.org"),
                OperatingHours = OperatingHours.Create24Hours(),
                AverageRating = 4.8m,
                ReviewCount = 125000,
                PriceLevel = null,
                Amenities = "[\"Walking Trails\",\"Playgrounds\",\"Lake\",\"Free Entry\",\"Dog Friendly\"]",
                ImageUrl = "https://example.com/central-park.jpg",
                IsVerified = true,
                IsActive = true,
                ViewCount = 500000
            },
            new Place
            {
                Name = "Times Square",
                Description = "Major commercial intersection and tourist destination in Midtown Manhattan. Known for its bright lights and Broadway theaters.",
                Category = PlaceCategory.Entertainment,
                Location = Location.Create(40.7580, -73.9855, "Times Square", "New York", "USA", "10036"),
                ContactInfo = ContactInfo.Create(null, null, "https://www.timessquarenyc.org"),
                OperatingHours = OperatingHours.Create24Hours(),
                AverageRating = 4.5m,
                ReviewCount = 78900,
                PriceLevel = null,
                Amenities = "[\"Shopping\",\"Theaters\",\"Restaurants\",\"Photography\"]",
                ImageUrl = "https://example.com/times-square.jpg",
                IsVerified = true,
                IsActive = true,
                ViewCount = 750000
            }
        };

        await _context.Places.AddRangeAsync(places);
        _logger.LogInformation("Seeded {Count} sample places", places.Count);
    }
}
