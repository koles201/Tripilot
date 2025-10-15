---
post_title: "Tripilot - Database Schema Design"
author1: "Development Team"
post_slug: "tripilot-database-schema"
microsoft_alias: "development-team"
featured_image: ""
categories: ["Database", "Schema", "Design"]
tags: ["PostgreSQL", "Entity Framework", "Database Design", "Schema"]
ai_note: "AI assisted in creating this database schema design"
summary: "Comprehensive database schema design for Tripilot application including tables, relationships, indexes, and constraints"
post_date: "2025-10-15"
---

## Database Overview

The Tripilot application uses PostgreSQL as its primary database with Entity Framework Core Code-First approach for data access. The database schema is defined through C# entity classes and created/maintained using EF Core migrations. This approach ensures type safety, version control of schema changes, and seamless integration with the application domain model.

## Entity Relationship Diagram

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│      Users      │    │     Routes      │    │     Places      │
│─────────────────│    │─────────────────│    │─────────────────│
│ Id (PK)         │    │ Id (PK)         │    │ Id (PK)         │
│ Email           │◄───┤ CreatedBy (FK)  │    │ Name            │
│ Username        │    │ Name            │    │ Description     │
│ PasswordHash    │    │ Description     │    │ CategoryId (FK) │──┐
│ FirstName       │    │ Duration        │    │ Address         │  │
│ LastName        │    │ DifficultyLevel │    │ Latitude        │  │
│ Avatar          │    │ PriceRange      │    │ Longitude       │  │
│ Bio             │    │ City            │    │ ContactInfo     │  │
│ Role            │    │ Region          │    │ OperatingHours  │  │
│ IsBusiness      │    │ Latitude        │    │ Images          │  │
│ SubscriptionTier│    │ Longitude       │    │ Website         │  │
│ CreatedAt       │    │ IsPublic        │    │ ClaimedBy (FK)  │──┘
│ UpdatedAt       │    │ Tags            │    │ IsVerified      │
└─────────────────┘    │ AvgRating       │    │ AvgRating       │
                       │ ReviewCount     │    │ ReviewCount     │
                       │ CreatedAt       │    │ CreatedAt       │
                       │ UpdatedAt       │    │ UpdatedAt       │
                       └─────────────────┘    └─────────────────┘
                                │                       │
                                │                       │
                       ┌─────────────────┐    ┌─────────────────┐
                       │   RoutePlaces   │    │   Categories    │
                       │─────────────────│    │─────────────────│
                       │ Id (PK)         │    │ Id (PK)         │
                       │ RouteId (FK)    │    │ Name            │
                       │ PlaceId (FK)    │────┘ Icon            │
                       │ Order           │      │ Description     │
                       │ EstimatedTime   │      │ IsActive        │
                       │ Notes           │      │ HasAudioGuide   │
                       └─────────────────┘      │ CreatedAt       │
                                                └─────────────────┘

┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│     Reviews     │    │   AudioGuides   │    │  Subscriptions  │
│─────────────────│    │─────────────────│    │─────────────────│
│ Id (PK)         │    │ Id (PK)         │    │ Id (PK)         │
│ UserId (FK)     │────┤ PlaceId (FK)    │    │ UserId (FK)     │
│ TargetType      │    │ Title           │    │ PlanType        │
│ TargetId        │    │ Description     │    │ Status          │
│ Comment         │    │ TextContent     │    │ StartDate       │
│ Photos          │    │ AudioFileUrl    │    │ EndDate         │
│ VisitDate       │    │ AudioDuration   │    │ Amount          │
│ VisitContext    │    │ Language        │    │ Currency        │
│ OverallRating   │    │ LanguageCode    │    │ PaymentId       │
│ ValueRating     │    │ CreatedBy (FK)  │    │ CreatedAt       │
│ ServiceRating   │    │ Version         │    │ UpdatedAt       │
│ CleanRating     │    │ IsActive        │    └─────────────────┘
│ AccessRating    │    │ DownloadCount   │
│ WouldRecommend  │    │ PlayCount       │
│ HelpfulVotes    │    │ FileSize        │
│ ReportCount     │    │ CreatedAt       │
│ BusinessReply   │    │ UpdatedAt       │
│ CreatedAt       │    └─────────────────┘
│ UpdatedAt       │
└─────────────────┘

┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   UserFavorites │    │  Notifications  │    │  BusinessInfo   │
│─────────────────│    │─────────────────│    │─────────────────│
│ Id (PK)         │    │ Id (PK)         │    │ Id (PK)         │
│ UserId (FK)     │    │ UserId (FK)     │    │ UserId (FK)     │
│ TargetType      │    │ Type            │    │ BusinessName    │
│ TargetId        │    │ Title           │    │ BusinessType    │
│ CreatedAt       │    │ Message         │    │ Description     │
└─────────────────┘    │ IsRead          │    │ ContactEmail    │
                       │ ActionUrl       │    │ ContactPhone    │
                       │ CreatedAt       │    │ Address         │
                       └─────────────────┘    │ TaxId           │
                                              │ IsVerified      │
                                              │ VerifiedAt      │
                                              │ CreatedAt       │
                                              │ UpdatedAt       │
                                              └─────────────────┘
```

## Entity Definitions (Code-First)

### User Entity

```csharp
namespace Tripilot.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Avatar { get; set; }
        public string? Bio { get; set; }
        public UserRole Role { get; set; } = UserRole.Tourist;
        public bool IsBusiness { get; set; } = false;
        public BusinessSubscriptionTier BusinessSubscriptionTier { get; set; } = BusinessSubscriptionTier.None;
        public LocationPreferences? LocationPreferences { get; set; }
        public UserPreferences? Preferences { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation properties
        public ICollection<Route> CreatedRoutes { get; set; } = new List<Route>();
        public ICollection<Place> ClaimedPlaces { get; set; } = new List<Place>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public BusinessInfo? BusinessInfo { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }

    // Value Objects
    public class LocationPreferences
    {
        public string? PreferredCity { get; set; }
        public int Radius { get; set; } = 50;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class UserPreferences
    {
        public List<string> PreferredCategories { get; set; } = new();
        public string PriceRange { get; set; } = "Medium";
        public bool AccessibilityRequired { get; set; } = false;
        public List<string> Languages { get; set; } = new() { "en" };
    }

    // Enums
    public enum UserRole
    {
        Tourist,
        Admin
    }

    public enum BusinessSubscriptionTier
    {
        None,
        Basic,
        Premium
    }
}
```

### Entity Configuration

```csharp
namespace Tripilot.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
            
            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
            
            builder.Property(u => u.FirstName)
                .HasMaxLength(100);
            
            builder.Property(u => u.LastName)
                .HasMaxLength(100);
            
            builder.Property(u => u.Bio)
                .HasMaxLength(1000);
            
            builder.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(50);
            
            builder.Property(u => u.BusinessSubscriptionTier)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Complex types for JSON columns
            builder.OwnsOne(u => u.LocationPreferences, lp =>
            {
                lp.ToJson();
                lp.Property(p => p.PreferredCity).HasMaxLength(100);
                lp.Property(p => p.Radius).HasDefaultValue(50);
            });

            builder.OwnsOne(u => u.Preferences, p =>
            {
                p.ToJson();
                p.Property(pr => pr.PriceRange).HasMaxLength(20);
            });

            // Indexes
            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.IsBusiness);
            builder.HasIndex(u => u.BusinessSubscriptionTier);
            builder.HasIndex(u => u.IsActive);

            // Relationships
            builder.HasMany(u => u.CreatedRoutes)
                .WithOne(r => r.CreatedBy)
                .HasForeignKey(r => r.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.ClaimedPlaces)
                .WithOne(p => p.ClaimedBy)
                .HasForeignKey(p => p.ClaimedById)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.BusinessInfo)
                .WithOne(bi => bi.User)
                .HasForeignKey<BusinessInfo>(bi => bi.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
```

### Category Entity

```csharp
namespace Tripilot.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool HasAudioGuide { get; set; } = false;
        public int DisplayOrder { get; set; }

        // Navigation properties
        public ICollection<Place> Places { get; set; } = new List<Place>();
    }
}
```

### Category Configuration

```csharp
namespace Tripilot.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(c => c.Icon)
                .HasMaxLength(100);
            
            builder.Property(c => c.Description)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(c => c.Name).IsUnique();
            builder.HasIndex(c => c.IsActive);
            builder.HasIndex(c => c.DisplayOrder);

            // Relationships
            builder.HasMany(c => c.Places)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data
            builder.HasData(
                new Category { Id = Guid.NewGuid(), Name = "Restaurants", Icon = "restaurant", Description = "Dining establishments and food venues", HasAudioGuide = false, DisplayOrder = 1, CreatedAt = DateTime.UtcNow },
                new Category { Id = Guid.NewGuid(), Name = "Museums", Icon = "museum", Description = "Cultural institutions and exhibitions", HasAudioGuide = true, DisplayOrder = 2, CreatedAt = DateTime.UtcNow },
                new Category { Id = Guid.NewGuid(), Name = "Entertainment", Icon = "entertainment", Description = "Entertainment venues and activities", HasAudioGuide = false, DisplayOrder = 3, CreatedAt = DateTime.UtcNow },
                new Category { Id = Guid.NewGuid(), Name = "Cafes", Icon = "cafe", Description = "Coffee shops and casual dining", HasAudioGuide = false, DisplayOrder = 4, CreatedAt = DateTime.UtcNow },
                new Category { Id = Guid.NewGuid(), Name = "Hotels", Icon = "hotel", Description = "Accommodation and lodging", HasAudioGuide = false, DisplayOrder = 5, CreatedAt = DateTime.UtcNow },
                new Category { Id = Guid.NewGuid(), Name = "Shopping", Icon = "shopping", Description = "Retail stores and shopping centers", HasAudioGuide = false, DisplayOrder = 6, CreatedAt = DateTime.UtcNow },
                new Category { Id = Guid.NewGuid(), Name = "Historical Sites", Icon = "historical", Description = "Historical landmarks and monuments", HasAudioGuide = true, DisplayOrder = 7, CreatedAt = DateTime.UtcNow },
                new Category { Id = Guid.NewGuid(), Name = "Parks", Icon = "park", Description = "Parks and recreational areas", HasAudioGuide = false, DisplayOrder = 8, CreatedAt = DateTime.UtcNow },
                new Category { Id = Guid.NewGuid(), Name = "Transportation", Icon = "transport", Description = "Transportation hubs and services", HasAudioGuide = false, DisplayOrder = 9, CreatedAt = DateTime.UtcNow }
            );
        }
    }
}
```

### Place Entity

```csharp
namespace Tripilot.Domain.Entities
{
    public class Place : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public string Address { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public ContactInfo? ContactInfo { get; set; }
        public OperatingHours? OperatingHours { get; set; }
        public List<string> Images { get; set; } = new();
        public string? Website { get; set; }
        public Guid? ClaimedById { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime? VerifiedAt { get; set; }
        public decimal AvgRating { get; set; } = 0.00m;
        public int ReviewCount { get; set; } = 0;
        public int ViewCount { get; set; } = 0;
        public string? PriceRange { get; set; }
        public List<string> Features { get; set; } = new();
        public Amenities? Amenities { get; set; }
        public AccessibilityInfo? AccessibilityInfo { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public Category Category { get; set; } = null!;
        public User? ClaimedBy { get; set; }
        public ICollection<RoutePlace> RoutePlaces { get; set; } = new List<RoutePlace>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<AudioGuide> AudioGuides { get; set; } = new List<AudioGuide>();
        public ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();
    }

    // Value Objects
    public class ContactInfo
    {
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
    }

    public class OperatingHours
    {
        public string? Monday { get; set; }
        public string? Tuesday { get; set; }
        public string? Wednesday { get; set; }
        public string? Thursday { get; set; }
        public string? Friday { get; set; }
        public string? Saturday { get; set; }
        public string? Sunday { get; set; }
        public string? SpecialHours { get; set; }
    }

    public class Amenities
    {
        public bool Parking { get; set; }
        public bool Wifi { get; set; }
        public bool Restrooms { get; set; }
        public bool GiftShop { get; set; }
        public bool Cafe { get; set; }
        public bool PetFriendly { get; set; }
    }

    public class AccessibilityInfo
    {
        public bool WheelchairAccess { get; set; }
        public bool ElevatorAccess { get; set; }
        public bool AudioGuide { get; set; }
        public bool SignLanguage { get; set; }
        public bool BrailleInformation { get; set; }
        public string? AdditionalNotes { get; set; }
    }
}
```

### Place Configuration

```csharp
namespace Tripilot.Infrastructure.Data.Configurations
{
    public class PlaceConfiguration : IEntityTypeConfiguration<Place>
    {
        public void Configure(EntityTypeBuilder<Place> builder)
        {
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(255);
            
            builder.Property(p => p.Description)
                .HasMaxLength(2000);
            
            builder.Property(p => p.Address)
                .IsRequired()
                .HasMaxLength(500);
            
            builder.Property(p => p.Website)
                .HasMaxLength(500);
            
            builder.Property(p => p.PriceRange)
                .HasMaxLength(20);

            builder.Property(p => p.AvgRating)
                .HasPrecision(3, 2);

            // Complex types for JSON columns
            builder.OwnsOne(p => p.ContactInfo, ci =>
            {
                ci.ToJson();
                ci.Property(c => c.Phone).HasMaxLength(50);
                ci.Property(c => c.Email).HasMaxLength(255);
                ci.Property(c => c.Website).HasMaxLength(500);
            });

            builder.OwnsOne(p => p.OperatingHours, oh =>
            {
                oh.ToJson();
                oh.Property(o => o.Monday).HasMaxLength(50);
                oh.Property(o => o.Tuesday).HasMaxLength(50);
                oh.Property(o => o.Wednesday).HasMaxLength(50);
                oh.Property(o => o.Thursday).HasMaxLength(50);
                oh.Property(o => o.Friday).HasMaxLength(50);
                oh.Property(o => o.Saturday).HasMaxLength(50);
                oh.Property(o => o.Sunday).HasMaxLength(50);
                oh.Property(o => o.SpecialHours).HasMaxLength(200);
            });

            builder.OwnsOne(p => p.Amenities, a =>
            {
                a.ToJson();
            });

            builder.OwnsOne(p => p.AccessibilityInfo, ai =>
            {
                ai.ToJson();
                ai.Property(a => a.AdditionalNotes).HasMaxLength(500);
            });

            // Collections as JSON
            builder.Property(p => p.Images)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                );

            builder.Property(p => p.Features)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
                );

            // Indexes
            builder.HasIndex(p => p.CategoryId);
            builder.HasIndex(p => p.ClaimedById);
            builder.HasIndex(p => p.AvgRating);
            builder.HasIndex(p => p.IsActive);
            builder.HasIndex(p => new { p.Latitude, p.Longitude });

            // Full-text search index (will be created via migration)
            builder.HasIndex(p => p.Name);

            // Relationships
            builder.HasOne(p => p.Category)
                .WithMany(c => c.Places)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.ClaimedBy)
                .WithMany(u => u.ClaimedPlaces)
                .HasForeignKey(p => p.ClaimedById)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
```

### Routes Table

```sql
CREATE TABLE Routes (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    Name VARCHAR(255) NOT NULL,
    Description TEXT,
    CreatedBy UUID NOT NULL REFERENCES Users(Id),
    Duration INTEGER, -- in minutes
    DifficultyLevel VARCHAR(20) DEFAULT 'Easy',
    PriceRange VARCHAR(20),
    City VARCHAR(100),
    Region VARCHAR(100),
    Country VARCHAR(100),
    Latitude DECIMAL(10,8),
    Longitude DECIMAL(11,8),
    IsPublic BOOLEAN NOT NULL DEFAULT TRUE,
    Tags TEXT[],
    AvgRating DECIMAL(3,2) DEFAULT 0.00,
    ReviewCount INTEGER DEFAULT 0,
    ViewCount INTEGER DEFAULT 0,
    CompletionCount INTEGER DEFAULT 0,
    IsFeatured BOOLEAN NOT NULL DEFAULT FALSE,
    AccessibilityLevel VARCHAR(20),
    BestTimeToVisit VARCHAR(100),
    EstimatedCost DECIMAL(10,2),
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes
CREATE INDEX idx_routes_created_by ON Routes(CreatedBy);
CREATE INDEX idx_routes_location ON Routes USING GIST (point(Longitude, Latitude));
CREATE INDEX idx_routes_public ON Routes(IsPublic);
CREATE INDEX idx_routes_rating ON Routes(AvgRating DESC);
CREATE INDEX idx_routes_city ON Routes(City);
CREATE INDEX idx_routes_name_search ON Routes USING GIN (to_tsvector('english', Name || ' ' || COALESCE(Description, '')));
CREATE INDEX idx_routes_featured ON Routes(IsFeatured);
```

### RoutePlaces Table

```sql
CREATE TABLE RoutePlaces (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    RouteId UUID NOT NULL REFERENCES Routes(Id) ON DELETE CASCADE,
    PlaceId UUID NOT NULL REFERENCES Places(Id) ON DELETE CASCADE,
    Order INTEGER NOT NULL,
    EstimatedTime INTEGER, -- minutes to spend at this place
    Notes TEXT,
    IsOptional BOOLEAN NOT NULL DEFAULT FALSE,
    TransportationNotes TEXT,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    UNIQUE(RouteId, PlaceId),
    UNIQUE(RouteId, Order)
);

-- Indexes
CREATE INDEX idx_route_places_route ON RoutePlaces(RouteId, Order);
CREATE INDEX idx_route_places_place ON RoutePlaces(PlaceId);
```

### Reviews Table

```sql
CREATE TABLE Reviews (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserId UUID NOT NULL REFERENCES Users(Id),
    TargetType VARCHAR(20) NOT NULL, -- 'Route' or 'Place'
    TargetId UUID NOT NULL,
    Comment TEXT,
    Photos TEXT[],
    VisitDate DATE,
    VisitDuration INTEGER, -- in hours
    VisitContext VARCHAR(50), -- 'Solo', 'Couple', 'Family', 'Business', 'Friends'
    OverallRating INTEGER NOT NULL CHECK (OverallRating >= 1 AND OverallRating <= 5),
    ValueRating INTEGER CHECK (ValueRating >= 1 AND ValueRating <= 5),
    ServiceRating INTEGER CHECK (ServiceRating >= 1 AND ServiceRating <= 5),
    CleanlinessRating INTEGER CHECK (CleanlinessRating >= 1 AND CleanlinessRating <= 5),
    AccessibilityRating INTEGER CHECK (AccessibilityRating >= 1 AND AccessibilityRating <= 5),
    WouldRecommend BOOLEAN,
    HelpfulVotes INTEGER DEFAULT 0,
    NotHelpfulVotes INTEGER DEFAULT 0,
    ReportCount INTEGER DEFAULT 0,
    BusinessReply TEXT,
    BusinessReplyAt TIMESTAMP WITH TIME ZONE,
    IsVerified BOOLEAN NOT NULL DEFAULT FALSE,
    IsModerated BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes
CREATE INDEX idx_reviews_user ON Reviews(UserId);
CREATE INDEX idx_reviews_target ON Reviews(TargetType, TargetId);
CREATE INDEX idx_reviews_rating ON Reviews(OverallRating DESC);
CREATE INDEX idx_reviews_created ON Reviews(CreatedAt DESC);
CREATE INDEX idx_reviews_verified ON Reviews(IsVerified);
```

### AudioGuides Table

```sql
CREATE TABLE AudioGuides (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    PlaceId UUID NOT NULL REFERENCES Places(Id) ON DELETE CASCADE,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    TextContent TEXT NOT NULL,
    AudioFileUrl TEXT NOT NULL,
    AudioDuration INTEGER NOT NULL, -- in seconds
    Language VARCHAR(10) NOT NULL DEFAULT 'en',
    LanguageCode VARCHAR(5) NOT NULL DEFAULT 'en-US',
    CreatedBy UUID NOT NULL REFERENCES Users(Id),
    Version INTEGER NOT NULL DEFAULT 1,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    DownloadCount INTEGER DEFAULT 0,
    PlayCount INTEGER DEFAULT 0,
    CompletionRate DECIMAL(5,2) DEFAULT 0.00,
    FileSize BIGINT, -- in bytes
    Checksum VARCHAR(64),
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    UNIQUE(PlaceId, Language)
);

-- Indexes
CREATE INDEX idx_audio_guides_place ON AudioGuides(PlaceId);
CREATE INDEX idx_audio_guides_language ON AudioGuides(Language);
CREATE INDEX idx_audio_guides_active ON AudioGuides(IsActive);
CREATE INDEX idx_audio_guides_created_by ON AudioGuides(CreatedBy);
```

### UserFavorites Table

```sql
CREATE TABLE UserFavorites (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserId UUID NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    TargetType VARCHAR(20) NOT NULL, -- 'Route' or 'Place'
    TargetId UUID NOT NULL,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    UNIQUE(UserId, TargetType, TargetId)
);

-- Indexes
CREATE INDEX idx_user_favorites_user ON UserFavorites(UserId);
CREATE INDEX idx_user_favorites_target ON UserFavorites(TargetType, TargetId);
```

### Subscriptions Table

```sql
CREATE TABLE Subscriptions (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserId UUID NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    PlanType VARCHAR(50) NOT NULL, -- 'Basic', 'Premium'
    Status VARCHAR(20) NOT NULL DEFAULT 'Active', -- 'Active', 'Cancelled', 'Expired', 'Past_Due'
    StartDate DATE NOT NULL,
    EndDate DATE,
    Amount DECIMAL(10,2) NOT NULL,
    Currency VARCHAR(3) NOT NULL DEFAULT 'USD',
    PaymentId VARCHAR(255),
    PaymentProvider VARCHAR(50), -- 'Stripe', 'PayPal'
    AutoRenew BOOLEAN NOT NULL DEFAULT TRUE,
    CancelledAt TIMESTAMP WITH TIME ZONE,
    CancellationReason TEXT,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes
CREATE INDEX idx_subscriptions_user ON Subscriptions(UserId);
CREATE INDEX idx_subscriptions_status ON Subscriptions(Status);
CREATE INDEX idx_subscriptions_end_date ON Subscriptions(EndDate);
```

### BusinessInfo Table

```sql
CREATE TABLE BusinessInfo (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserId UUID NOT NULL REFERENCES Users(Id) ON DELETE CASCADE UNIQUE,
    BusinessName VARCHAR(255) NOT NULL,
    BusinessType VARCHAR(100),
    Description TEXT,
    ContactEmail VARCHAR(255),
    ContactPhone VARCHAR(50),
    Address JSONB,
    TaxId VARCHAR(100),
    BusinessHours JSONB,
    SocialMedia JSONB,
    Website VARCHAR(500),
    Logo TEXT,
    CoverImage TEXT,
    IsVerified BOOLEAN NOT NULL DEFAULT FALSE,
    VerifiedAt TIMESTAMP WITH TIME ZONE,
    VerificationDocuments TEXT[],
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes
CREATE INDEX idx_business_info_user ON BusinessInfo(UserId);
CREATE INDEX idx_business_info_verified ON BusinessInfo(IsVerified);
CREATE INDEX idx_business_info_name ON BusinessInfo USING GIN (to_tsvector('english', BusinessName));
```

### Notifications Table

```sql
CREATE TABLE Notifications (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    UserId UUID NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    Type VARCHAR(50) NOT NULL, -- 'review_received', 'route_shared', 'subscription_expiring', etc.
    Title VARCHAR(255) NOT NULL,
    Message TEXT NOT NULL,
    IsRead BOOLEAN NOT NULL DEFAULT FALSE,
    ActionUrl TEXT,
    Metadata JSONB,
    ExpiresAt TIMESTAMP WITH TIME ZONE,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Indexes
CREATE INDEX idx_notifications_user ON Notifications(UserId);
CREATE INDEX idx_notifications_unread ON Notifications(UserId, IsRead) WHERE IsRead = FALSE;
CREATE INDEX idx_notifications_type ON Notifications(Type);
CREATE INDEX idx_notifications_created ON Notifications(CreatedAt DESC);
```

### ReviewVotes Table

```sql
CREATE TABLE ReviewVotes (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    ReviewId UUID NOT NULL REFERENCES Reviews(Id) ON DELETE CASCADE,
    UserId UUID NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    IsHelpful BOOLEAN NOT NULL,
    CreatedAt TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    UNIQUE(ReviewId, UserId)
);

-- Indexes
CREATE INDEX idx_review_votes_review ON ReviewVotes(ReviewId);
CREATE INDEX idx_review_votes_user ON ReviewVotes(UserId);
```

## Database Functions and Triggers

### Update Ratings Trigger Function

```sql
CREATE OR REPLACE FUNCTION update_average_rating()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
        IF NEW.TargetType = 'Place' THEN
            UPDATE Places 
            SET 
                AvgRating = (
                    SELECT ROUND(AVG(OverallRating::DECIMAL), 2)
                    FROM Reviews 
                    WHERE TargetType = 'Place' AND TargetId = NEW.TargetId
                ),
                ReviewCount = (
                    SELECT COUNT(*)
                    FROM Reviews 
                    WHERE TargetType = 'Place' AND TargetId = NEW.TargetId
                ),
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE Id = NEW.TargetId;
        ELSIF NEW.TargetType = 'Route' THEN
            UPDATE Routes 
            SET 
                AvgRating = (
                    SELECT ROUND(AVG(OverallRating::DECIMAL), 2)
                    FROM Reviews 
                    WHERE TargetType = 'Route' AND TargetId = NEW.TargetId
                ),
                ReviewCount = (
                    SELECT COUNT(*)
                    FROM Reviews 
                    WHERE TargetType = 'Route' AND TargetId = NEW.TargetId
                ),
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE Id = NEW.TargetId;
        END IF;
        RETURN NEW;
    END IF;
    
    IF TG_OP = 'DELETE' THEN
        IF OLD.TargetType = 'Place' THEN
            UPDATE Places 
            SET 
                AvgRating = COALESCE((
                    SELECT ROUND(AVG(OverallRating::DECIMAL), 2)
                    FROM Reviews 
                    WHERE TargetType = 'Place' AND TargetId = OLD.TargetId
                ), 0.00),
                ReviewCount = (
                    SELECT COUNT(*)
                    FROM Reviews 
                    WHERE TargetType = 'Place' AND TargetId = OLD.TargetId
                ),
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE Id = OLD.TargetId;
        ELSIF OLD.TargetType = 'Route' THEN
            UPDATE Routes 
            SET 
                AvgRating = COALESCE((
                    SELECT ROUND(AVG(OverallRating::DECIMAL), 2)
                    FROM Reviews 
                    WHERE TargetType = 'Route' AND TargetId = OLD.TargetId
                ), 0.00),
                ReviewCount = (
                    SELECT COUNT(*)
                    FROM Reviews 
                    WHERE TargetType = 'Route' AND TargetId = OLD.TargetId
                ),
                UpdatedAt = CURRENT_TIMESTAMP
            WHERE Id = OLD.TargetId;
        END IF;
        RETURN OLD;
    END IF;
    
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- Create triggers
CREATE TRIGGER trg_update_rating_after_review_change
    AFTER INSERT OR UPDATE OR DELETE ON Reviews
    FOR EACH ROW
    EXECUTE FUNCTION update_average_rating();
```

### Update Review Vote Counts Function

```sql
CREATE OR REPLACE FUNCTION update_review_votes()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
        UPDATE Reviews 
        SET 
            HelpfulVotes = (
                SELECT COUNT(*) 
                FROM ReviewVotes 
                WHERE ReviewId = NEW.ReviewId AND IsHelpful = TRUE
            ),
            NotHelpfulVotes = (
                SELECT COUNT(*) 
                FROM ReviewVotes 
                WHERE ReviewId = NEW.ReviewId AND IsHelpful = FALSE
            ),
            UpdatedAt = CURRENT_TIMESTAMP
        WHERE Id = NEW.ReviewId;
        RETURN NEW;
    END IF;
    
    IF TG_OP = 'DELETE' THEN
        UPDATE Reviews 
        SET 
            HelpfulVotes = (
                SELECT COUNT(*) 
                FROM ReviewVotes 
                WHERE ReviewId = OLD.ReviewId AND IsHelpful = TRUE
            ),
            NotHelpfulVotes = (
                SELECT COUNT(*) 
                FROM ReviewVotes 
                WHERE ReviewId = OLD.ReviewId AND IsHelpful = FALSE
            ),
            UpdatedAt = CURRENT_TIMESTAMP
        WHERE Id = OLD.ReviewId;
        RETURN OLD;
    END IF;
    
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- Create trigger
CREATE TRIGGER trg_update_review_votes
    AFTER INSERT OR UPDATE OR DELETE ON ReviewVotes
    FOR EACH ROW
    EXECUTE FUNCTION update_review_votes();
```

### Update Timestamps Function

```sql
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.UpdatedAt = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Apply to all tables with UpdatedAt column
CREATE TRIGGER trg_users_updated_at BEFORE UPDATE ON Users FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER trg_places_updated_at BEFORE UPDATE ON Places FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER trg_routes_updated_at BEFORE UPDATE ON Routes FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER trg_reviews_updated_at BEFORE UPDATE ON Reviews FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER trg_audio_guides_updated_at BEFORE UPDATE ON AudioGuides FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER trg_subscriptions_updated_at BEFORE UPDATE ON Subscriptions FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
CREATE TRIGGER trg_business_info_updated_at BEFORE UPDATE ON BusinessInfo FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();
```

## Performance Considerations

### Indexing Strategy

1. **Primary Keys**: All tables use UUID primary keys for scalability
2. **Foreign Keys**: Indexed for join performance
3. **Search Fields**: Full-text search indexes on name and description fields
4. **Geographic Data**: PostGIS indexes for location-based queries
5. **Filtering Fields**: Indexes on commonly filtered columns (rating, active status, etc.)

### Query Optimization

1. **Materialized Views**: For complex aggregations and reporting
2. **Partitioning**: Consider partitioning large tables by date
3. **Connection Pooling**: Use pgBouncer for connection management
4. **Read Replicas**: For read-heavy operations

### Data Archival Strategy

1. **Soft Delete**: Use IsActive flags instead of hard deletes
2. **Audit Trail**: Keep track of data changes for compliance
3. **Data Retention**: Archive old notifications and analytics data

## Security Considerations

### Data Protection

1. **Encryption**: Sensitive data encrypted at rest and in transit
2. **Access Control**: Role-based database access
3. **Audit Logging**: Track all data modifications
4. **Backup Strategy**: Regular encrypted backups with point-in-time recovery

### Compliance

1. **GDPR**: Support for data export and deletion
2. **Data Anonymization**: For analytics and reporting
3. **Retention Policies**: Automatic cleanup of expired data

## DbContext Implementation

```csharp
namespace Tripilot.Infrastructure.Data
{
    public class TripilotDbContext : DbContext
    {
        public TripilotDbContext(DbContextOptions<TripilotDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RoutePlace> RoutePlaces { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<AudioGuide> AudioGuides { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<BusinessInfo> BusinessInfos { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ReviewVote> ReviewVotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TripilotDbContext).Assembly);

            // Global query filters
            modelBuilder.Entity<User>().HasQueryFilter(u => u.IsActive);
            modelBuilder.Entity<Place>().HasQueryFilter(p => p.IsActive);
            modelBuilder.Entity<Route>().HasQueryFilter(r => r.IsActive);

            // Global configurations
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Configure BaseEntity properties
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<Guid>("Id")
                        .HasDefaultValueSql("gen_random_uuid()");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime>("CreatedAt")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property<DateTime>("UpdatedAt")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
                }

                // Configure string properties
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string) && property.GetMaxLength() == null)
                    {
                        property.SetMaxLength(500); // Default max length
                    }
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Update timestamps
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

// Base Entity
namespace Tripilot.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
```

## Migration Strategy (Code-First)

### Initial Migration Commands

```bash
# Add initial migration
dotnet ef migrations add InitialCreate --project src/Tripilot.Infrastructure --startup-project src/Tripilot.Api

# Update database
dotnet ef database update --project src/Tripilot.Infrastructure --startup-project src/Tripilot.Api
```

### Custom Migration for PostgreSQL Extensions

```csharp
namespace Tripilot.Infrastructure.Data.Migrations
{
    public partial class AddPostgreSQLExtensions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Enable UUID generation
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");
            
            // Enable full-text search
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS \"unaccent\";");
            
            // Enable PostGIS for geospatial queries (if needed)
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS \"postgis\";");

            // Create custom indexes
            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_places_fulltext_search 
                ON ""Places"" USING gin(to_tsvector('english', ""Name"" || ' ' || COALESCE(""Description"", '')));
            ");

            migrationBuilder.Sql(@"
                CREATE INDEX IF NOT EXISTS idx_routes_fulltext_search 
                ON ""Routes"" USING gin(to_tsvector('english', ""Name"" || ' ' || COALESCE(""Description"", '')));
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_places_fulltext_search;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_routes_fulltext_search;");
        }
    }
}
```

### Database Seeding Strategy

```csharp
namespace Tripilot.Infrastructure.Data
{
    public static class DbContextSeed
    {
        public static async Task SeedAsync(TripilotDbContext context, ILogger logger)
        {
            try
            {
                await SeedCategoriesAsync(context);
                await SeedSampleDataAsync(context);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the database");
                throw;
            }
        }

        private static async Task SeedCategoriesAsync(TripilotDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new() { Name = "Restaurants", Icon = "restaurant", Description = "Dining establishments and food venues", HasAudioGuide = false, DisplayOrder = 1 },
                    new() { Name = "Museums", Icon = "museum", Description = "Cultural institutions and exhibitions", HasAudioGuide = true, DisplayOrder = 2 },
                    new() { Name = "Entertainment", Icon = "entertainment", Description = "Entertainment venues and activities", HasAudioGuide = false, DisplayOrder = 3 },
                    new() { Name = "Cafes", Icon = "cafe", Description = "Coffee shops and casual dining", HasAudioGuide = false, DisplayOrder = 4 },
                    new() { Name = "Hotels", Icon = "hotel", Description = "Accommodation and lodging", HasAudioGuide = false, DisplayOrder = 5 },
                    new() { Name = "Shopping", Icon = "shopping", Description = "Retail stores and shopping centers", HasAudioGuide = false, DisplayOrder = 6 },
                    new() { Name = "Historical Sites", Icon = "historical", Description = "Historical landmarks and monuments", HasAudioGuide = true, DisplayOrder = 7 },
                    new() { Name = "Parks", Icon = "park", Description = "Parks and recreational areas", HasAudioGuide = false, DisplayOrder = 8 },
                    new() { Name = "Transportation", Icon = "transport", Description = "Transportation hubs and services", HasAudioGuide = false, DisplayOrder = 9 }
                };

                context.Categories.AddRange(categories);
            }
        }

        private static async Task SeedSampleDataAsync(TripilotDbContext context)
        {
            // Add sample data for development/testing
            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    Email = "admin@tripilot.com",
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    FirstName = "Admin",
                    LastName = "User",
                    Role = UserRole.Admin,
                    IsEmailVerified = true
                };

                context.Users.Add(adminUser);
            }
        }
    }
}
```

### Development vs Production Migrations

```csharp
// Program.cs - Development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TripilotDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    // Ensure database is created
    await context.Database.EnsureCreatedAsync();
    
    // Apply pending migrations
    if (context.Database.GetPendingMigrations().Any())
    {
        await context.Database.MigrateAsync();
    }
    
    // Seed data
    await DbContextSeed.SeedAsync(context, logger);
}

// Program.cs - Production
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TripilotDbContext>();
    
    // Only apply migrations, don't seed in production
    if (context.Database.GetPendingMigrations().Any())
    {
        await context.Database.MigrateAsync();
    }
}
```

### Migration Best Practices

1. **Version Control**: All migrations committed to source control
2. **Backward Compatibility**: Migrations should be backward compatible when possible
3. **Data Migrations**: Separate data migrations from schema migrations
4. **Rollback Strategy**: Always test rollback scenarios
5. **Environment Parity**: Same migrations across all environments
6. **Production Deployment**: 
   - Backup database before migrations
   - Run migrations during maintenance windows
   - Monitor migration performance

### Entity Framework Configuration

```csharp
// Startup.cs or Program.cs
services.AddDbContext<TripilotDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);
            
        npgsqlOptions.MigrationsAssembly("Tripilot.Infrastructure");
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
    });

    if (environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});
```

This code-first approach provides type safety, version control of schema changes, and seamless integration with the application domain model while maintaining all the performance and scalability benefits of the original design.