---
post_title: "Tripilot - Technical Architecture"
author1: "Development Team"
post_slug: "tripilot-technical-architecture"
microsoft_alias: "development-team"
featured_image: ""
categories: ["Architecture", "Technical Design", "System Design"]
tags: ["Architecture", "CQRS", "Microservices", "System Design", "Scalability"]
ai_note: "AI assisted in creating this technical architecture documentation"
summary: "Complete technical architecture documentation for Tripilot application including system design, component architecture, deployment strategies, and scalability considerations"
post_date: "2025-10-15"
---

## Architecture Overview

Tripilot follows a modern, scalable architecture designed to handle high traffic loads while maintaining performance and reliability. The system is built using CQRS (Command Query Responsibility Segregation) pattern with Domain-Driven Design principles.

## High-Level System Architecture

```
                                    ┌─────────────────┐
                                    │   Load Balancer │
                                    │    (nginx)      │
                                    └─────────┬───────┘
                                              │
                              ┌───────────────┼───────────────┐
                              │               │               │
                    ┌─────────▼─────────┐   ┌─▼──────────────▼─┐
                    │   Web Frontend    │   │   API Gateway    │
                    │    (React SPA)    │   │   (ASP.NET Core) │
                    └─────────┬─────────┘   └─┬──────────────┬─┘
                              │               │              │
                              └───────────────┘              │
                                                             │
                    ┌────────────────────────────────────────┼─────────────────┐
                    │                                        │                 │
          ┌─────────▼─────────┐              ┌──────────────▼────────┐        │
          │  Authentication   │              │    Core API Services  │        │
          │     Service       │              │   (ASP.NET Core)      │        │
          │  (Identity Server)│              └──────────┬─────────┬──┘        │
          └─────────┬─────────┘                         │         │           │
                    │                                   │         │           │
                    │                  ┌────────────────▼─────────▼───────────▼───┐
                    │                  │           Message Bus (RabbitMQ)         │
                    │                  └────────────────┬─────────┬───────────┬───┘
                    │                                   │         │           │
                    │        ┌──────────────────────────▼─────────▼───────────▼───┐
                    │        │                 Background Services                 │
                    │        │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ │
                    │        │  │Email Service│ │File Service │ │Analytics    │ │
                    │        │  │   Worker    │ │   Worker    │ │   Worker    │ │
                    │        │  └─────────────┘ └─────────────┘ └─────────────┘ │
                    │        └────────────────────────────────────────────────────┘
                    │
              ┌─────▼─────────────────────────────────────────────────────────────┐
              │                        Data Layer                                │
              │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐│
              │  │ PostgreSQL  │ │    Redis    │ │   File      │ │  Analytics  ││
              │  │ (Primary DB)│ │   (Cache)   │ │  Storage    │ │  Database   ││
              │  └─────────────┘ └─────────────┘ └─────────────┘ └─────────────┘│
              └───────────────────────────────────────────────────────────────────┘
```

## Application Architecture

### Frontend Architecture (React)

```
src/
├── components/           # Reusable UI components
│   ├── common/          # Generic components (Button, Modal, etc.)
│   ├── forms/           # Form components with validation
│   ├── navigation/      # Navigation components
│   ├── media/           # Image, video, audio components
│   └── specialized/     # Feature-specific components
├── pages/               # Page components
│   ├── auth/           # Authentication pages
│   ├── places/         # Place-related pages
│   ├── routes/         # Route-related pages
│   ├── profile/        # User profile pages
│   └── business/       # Business dashboard pages
├── services/           # API and external service layers
│   ├── api/           # REST API clients
│   ├── audio/         # Audio guide services
│   ├── maps/          # Maps integration
│   └── offline/       # Offline data management
├── store/             # Redux store configuration
│   ├── slices/        # Redux Toolkit slices
│   ├── middleware/    # Custom middleware
│   └── selectors/     # Reusable selectors
├── hooks/             # Custom React hooks
├── utils/             # Utility functions
├── types/             # TypeScript type definitions
└── workers/           # Service workers
```

#### Component Architecture Pattern

```typescript
// Component Structure Example
interface PlaceCardProps {
  place: Place;
  showDistance?: boolean;
  onFavorite?: (placeId: string) => void;
  onClick?: (place: Place) => void;
}

export const PlaceCard: React.FC<PlaceCardProps> = ({
  place,
  showDistance = false,
  onFavorite,
  onClick
}) => {
  // Component implementation
};
```

#### State Management Architecture

```typescript
// Redux Store Structure
interface RootState {
  auth: AuthState;
  places: PlacesState;
  routes: RoutesState;
  reviews: ReviewsState;
  ui: UIState;
  offline: OfflineState;
}

// RTK Query API Slice Example
export const placesApi = createApi({
  reducerPath: 'placesApi',
  baseQuery: fetchBaseQuery({
    baseUrl: '/api/places',
    prepareHeaders: (headers, { getState }) => {
      const token = (getState() as RootState).auth.token;
      if (token) {
        headers.set('authorization', `Bearer ${token}`);
      }
      return headers;
    },
  }),
  tagTypes: ['Place', 'Review'],
  endpoints: (builder) => ({
    getPlaces: builder.query<PlacesResponse, PlacesQuery>({
      query: (params) => ({
        url: '',
        params,
      }),
      providesTags: ['Place'],
    }),
  }),
});
```

### Backend Architecture (ASP.NET Core)

```
src/
├── Tripilot.Api/              # Web API project (Presentation Layer)
│   ├── Controllers/           # REST API controllers
│   ├── Middleware/           # Custom middleware
│   ├── Filters/              # Action filters
│   ├── Extensions/           # Service extensions
│   └── Program.cs            # Application entry point
├── Tripilot.Application/      # Application Layer (CQRS)
│   ├── Commands/             # Command handlers
│   │   ├── Users/
│   │   ├── Places/
│   │   ├── Routes/
│   │   └── Reviews/
│   ├── Queries/              # Query handlers
│   │   ├── Users/
│   │   ├── Places/
│   │   ├── Routes/
│   │   └── Reviews/
│   ├── Services/             # Application services
│   ├── Validators/           # FluentValidation validators
│   ├── Mappings/             # AutoMapper profiles
│   └── Interfaces/           # Application interfaces
├── Tripilot.Domain/           # Domain Layer
│   ├── Entities/             # Domain entities
│   ├── ValueObjects/         # Value objects
│   ├── Events/               # Domain events
│   ├── Exceptions/           # Domain exceptions
│   └── Interfaces/           # Domain interfaces
├── Tripilot.Infrastructure/   # Infrastructure Layer
│   ├── Data/                 # Entity Framework context
│   │   ├── Configurations/   # Entity configurations
│   │   ├── Migrations/       # Database migrations
│   │   └── TripilotDbContext.cs
│   ├── Repositories/         # Repository implementations
│   ├── Services/             # External service implementations
│   │   ├── EmailService.cs
│   │   ├── FileStorageService.cs
│   │   └── CacheService.cs
│   └── Extensions/           # Infrastructure extensions
└── Tripilot.Shared/          # Shared contracts and DTOs
    ├── DTOs/                 # Data transfer objects
    ├── Enums/                # Shared enumerations
    ├── Constants/            # Application constants
    └── Extensions/           # Extension methods
```

#### CQRS Implementation

```csharp
// Command Example
public class CreatePlaceCommand : IRequest<CreatePlaceResponse>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
    public string Address { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    // Additional properties...
}

// Command Handler
public class CreatePlaceCommandHandler : IRequestHandler<CreatePlaceCommand, CreatePlaceResponse>
{
    private readonly ITripilotDbContext _context;
    private readonly IMapper _mapper;
    private readonly IValidator<CreatePlaceCommand> _validator;

    public async Task<CreatePlaceResponse> Handle(CreatePlaceCommand request, CancellationToken cancellationToken)
    {
        // Validation
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Business logic
        var place = _mapper.Map<Place>(request);
        place.Id = Guid.NewGuid();
        place.CreatedAt = DateTime.UtcNow;

        _context.Places.Add(place);
        await _context.SaveChangesAsync(cancellationToken);

        // Publish domain event
        await _mediator.Publish(new PlaceCreatedEvent(place.Id), cancellationToken);

        return _mapper.Map<CreatePlaceResponse>(place);
    }
}

// Query Example
public class GetPlacesQuery : IRequest<GetPlacesResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? CategoryId { get; set; }
    public string? City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? Radius { get; set; }
    public string? Search { get; set; }
}

// Query Handler
public class GetPlacesQueryHandler : IRequestHandler<GetPlacesQuery, GetPlacesResponse>
{
    private readonly ITripilotDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICacheService _cache;

    public async Task<GetPlacesResponse> Handle(GetPlacesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"places_{JsonSerializer.Serialize(request)}";
        var cachedResult = await _cache.GetAsync<GetPlacesResponse>(cacheKey);
        if (cachedResult != null)
            return cachedResult;

        var query = _context.Places.AsQueryable();

        // Apply filters
        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (!string.IsNullOrEmpty(request.City))
            query = query.Where(p => p.Address.Contains(request.City));

        // Location-based filtering
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            var radius = request.Radius ?? 50;
            query = query.Where(p => 
                EF.Functions.DistanceBetween(
                    EF.Functions.Point(request.Longitude.Value, request.Latitude.Value),
                    EF.Functions.Point(p.Longitude, p.Latitude)
                ) <= radius * 1000);
        }

        // Search functionality
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(p => 
                EF.Functions.ToTsVector("english", p.Name + " " + p.Description)
                    .Matches(EF.Functions.PhraseToTsQuery("english", request.Search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var places = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var response = new GetPlacesResponse
        {
            Places = _mapper.Map<List<PlaceDto>>(places),
            Pagination = new PaginationDto
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalItems = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                HasNext = request.Page * request.PageSize < totalCount,
                HasPrevious = request.Page > 1
            }
        };

        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(15));
        return response;
    }
}
```

## Data Architecture

### Database Design Patterns

#### Repository Pattern with Code-First Entities
```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    IQueryable<T> Query();
}

public class PlaceRepository : Repository<Place>, IPlaceRepository
{
    public PlaceRepository(TripilotDbContext context) : base(context)
    {
    }

    public async Task<Place?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Places
            .Include(p => p.Category)
            .Include(p => p.AudioGuides.Where(ag => ag.IsActive))
            .Include(p => p.Reviews.OrderByDescending(r => r.CreatedAt).Take(10))
            .Include(p => p.ClaimedBy)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Place>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .OrderByDescending(p => p.AvgRating)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Place>> SearchPlacesAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _context.Places
            .Where(p => EF.Functions.ToTsVector("english", p.Name + " " + (p.Description ?? ""))
                .Matches(EF.Functions.PhraseToTsQuery("english", searchTerm)))
            .Include(p => p.Category)
            .OrderByDescending(p => p.AvgRating)
            .ToListAsync(cancellationToken);
    }
}

// Base Repository Implementation
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly TripilotDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(TripilotDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entry = await _dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }
}
```

#### Unit of Work Pattern with Code-First Context
```csharp
public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IPlaceRepository Places { get; }
    IRouteRepository Routes { get; }
    IReviewRepository Reviews { get; }
    ICategoryRepository Categories { get; }
    IAudioGuideRepository AudioGuides { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly TripilotDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(TripilotDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Places = new PlaceRepository(_context);
        Routes = new RouteRepository(_context);
        Reviews = new ReviewRepository(_context);
        Categories = new CategoryRepository(_context);
        AudioGuides = new AudioGuideRepository(_context);
    }

    public IUserRepository Users { get; }
    public IPlaceRepository Places { get; }
    public IRouteRepository Routes { get; }
    public IReviewRepository Reviews { get; }
    public ICategoryRepository Categories { get; }
    public IAudioGuideRepository AudioGuides { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Domain events will be handled here via MediatR
        var domainEvents = _context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        var result = await _context.SaveChangesAsync(cancellationToken);

        // Publish domain events after successful save
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        return result;
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}

// Enhanced BaseEntity with Domain Events
namespace Tripilot.Domain.Entities
{
    public abstract class BaseEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new();

        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }

    // Domain Event Interface
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }

    // Example Domain Event
    public class PlaceCreatedEvent : IDomainEvent
    {
        public PlaceCreatedEvent(Guid placeId, string placeName, Guid categoryId)
        {
            PlaceId = placeId;
            PlaceName = placeName;
            CategoryId = categoryId;
            OccurredOn = DateTime.UtcNow;
        }

        public Guid PlaceId { get; }
        public string PlaceName { get; }
        public Guid CategoryId { get; }
        public DateTime OccurredOn { get; }
    }
}
```

### Caching Strategy

#### Multi-Level Caching
```csharp
public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IMemoryCache _memoryCache;

    // Level 1: Memory Cache (fastest, limited capacity)
    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        // Try memory cache first
        if (_memoryCache.TryGetValue(key, out T? memoryResult))
            return memoryResult;

        // Try distributed cache
        var distributedResult = await _distributedCache.GetStringAsync(key);
        if (distributedResult != null)
        {
            var deserializedResult = JsonSerializer.Deserialize<T>(distributedResult);
            // Store in memory cache for faster access
            _memoryCache.Set(key, deserializedResult, TimeSpan.FromMinutes(5));
            return deserializedResult;
        }

        return null;
    }

    // Level 2: Distributed Cache (Redis)
    public async Task SetAsync<T>(string key, T value, TimeSpan expiration) where T : class
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, serializedValue, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        });

        // Also store in memory cache
        _memoryCache.Set(key, value, TimeSpan.FromMinutes(Math.Min(5, expiration.TotalMinutes)));
    }
}
```

## Security Architecture

### Authentication & Authorization Flow

```
1. User Login Request
   ↓
2. Validate Credentials
   ↓
3. Generate JWT Token + Refresh Token
   ↓
4. Return Tokens to Client
   ↓
5. Client Stores Tokens
   ↓
6. API Request with JWT Token
   ↓
7. Validate JWT Token
   ↓
8. Check User Permissions
   ↓
9. Process Request
```

#### JWT Implementation
```csharp
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUserRepository _userRepository;

    public async Task<TokenResponse> GenerateTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("role", user.Role),
            new Claim("isBusiness", user.IsBusiness.ToString()),
            new Claim("subscriptionTier", user.BusinessSubscriptionTier)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var refreshToken = GenerateRefreshToken();
        
        // Store refresh token in database
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await _userRepository.UpdateAsync(user);

        return new TokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            ExpiresAt = token.ValidTo
        };
    }
}
```

### API Security Middleware
```csharp
public class SecurityMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        // Rate limiting
        await ApplyRateLimitingAsync(context);

        // Request validation
        await ValidateRequestAsync(context);

        // CORS handling
        ApplyCorsHeaders(context);

        // Security headers
        ApplySecurityHeaders(context);

        await _next(context);
    }

    private async Task ApplyRateLimitingAsync(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        var endpoint = context.Request.Path;
        
        var rateLimitKey = $"rate_limit:{clientId}:{endpoint}";
        var requestCount = await _cache.GetAsync<int>(rateLimitKey) ?? 0;
        
        var limit = GetRateLimitForUser(context.User);
        
        if (requestCount >= limit)
        {
            context.Response.StatusCode = 429;
            await context.Response.WriteAsync("Rate limit exceeded");
            return;
        }

        await _cache.SetAsync(rateLimitKey, requestCount + 1, TimeSpan.FromHours(1));
    }
}
```

## Deployment Architecture

### Containerization Strategy

#### Docker Configuration
```dockerfile
# Frontend Dockerfile
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

```dockerfile
# Backend Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Tripilot.Api/Tripilot.Api.csproj", "src/Tripilot.Api/"]
COPY ["src/Tripilot.Application/Tripilot.Application.csproj", "src/Tripilot.Application/"]
COPY ["src/Tripilot.Domain/Tripilot.Domain.csproj", "src/Tripilot.Domain/"]
COPY ["src/Tripilot.Infrastructure/Tripilot.Infrastructure.csproj", "src/Tripilot.Infrastructure/"]
COPY ["src/Tripilot.Shared/Tripilot.Shared.csproj", "src/Tripilot.Shared/"]
RUN dotnet restore "src/Tripilot.Api/Tripilot.Api.csproj"
COPY . .
WORKDIR "/src/src/Tripilot.Api"
RUN dotnet build "Tripilot.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tripilot.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tripilot.Api.dll"]
```

#### Docker Compose for Development
```yaml
version: '3.8'

services:
  tripilot-db:
    image: postgres:15
    environment:
      POSTGRES_DB: tripilot_dev
      POSTGRES_USER: tripilot
      POSTGRES_PASSWORD: tripilot_dev_password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  tripilot-redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"

  tripilot-api:
    build:
      context: .
      dockerfile: server/Dockerfile
    ports:
      - "5000:80"
    depends_on:
      - tripilot-db
      - tripilot-redis
    environment:
      - ConnectionStrings__DefaultConnection=Host=tripilot-db;Database=tripilot_dev;Username=tripilot;Password=tripilot_dev_password
      - ConnectionStrings__Redis=tripilot-redis:6379

  tripilot-client:
    build:
      context: .
      dockerfile: client/Dockerfile
    ports:
      - "3000:80"
    depends_on:
      - tripilot-api

volumes:
  postgres_data:
```

### Cloud Deployment (Azure)

#### Infrastructure as Code (Terraform)
```hcl
# Azure Resource Group
resource "azurerm_resource_group" "tripilot" {
  name     = "rg-tripilot-${var.environment}"
  location = var.location
}

# Azure Container Registry
resource "azurerm_container_registry" "tripilot" {
  name                = "acrtripilot${var.environment}"
  resource_group_name = azurerm_resource_group.tripilot.name
  location           = azurerm_resource_group.tripilot.location
  sku                = "Premium"
  admin_enabled      = true
}

# Azure Container Apps Environment
resource "azurerm_container_app_environment" "tripilot" {
  name                       = "cae-tripilot-${var.environment}"
  location                   = azurerm_resource_group.tripilot.location
  resource_group_name        = azurerm_resource_group.tripilot.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.tripilot.id
}

# PostgreSQL Flexible Server
resource "azurerm_postgresql_flexible_server" "tripilot" {
  name                   = "psql-tripilot-${var.environment}"
  resource_group_name    = azurerm_resource_group.tripilot.name
  location              = azurerm_resource_group.tripilot.location
  version               = "15"
  administrator_login    = var.db_admin_username
  administrator_password = var.db_admin_password
  zone                  = "1"
  
  storage_mb = 32768
  sku_name   = "GP_Standard_D2s_v3"
}

# Redis Cache
resource "azurerm_redis_cache" "tripilot" {
  name                = "redis-tripilot-${var.environment}"
  location            = azurerm_resource_group.tripilot.location
  resource_group_name = azurerm_resource_group.tripilot.name
  capacity            = 1
  family              = "C"
  sku_name            = "Standard"
  enable_non_ssl_port = false
}

# Container App for API
resource "azurerm_container_app" "api" {
  name                         = "ca-tripilot-api-${var.environment}"
  container_app_environment_id = azurerm_container_app_environment.tripilot.id
  resource_group_name          = azurerm_resource_group.tripilot.name
  revision_mode                = "Single"

  template {
    container {
      name   = "tripilot-api"
      image  = "${azurerm_container_registry.tripilot.name}.azurecr.io/tripilot-api:latest"
      cpu    = 0.5
      memory = "1Gi"

      env {
        name  = "ConnectionStrings__DefaultConnection"
        value = "Host=${azurerm_postgresql_flexible_server.tripilot.fqdn};Database=tripilot;Username=${var.db_admin_username};Password=${var.db_admin_password}"
      }

      env {
        name  = "ConnectionStrings__Redis"
        value = "${azurerm_redis_cache.tripilot.hostname}:${azurerm_redis_cache.tripilot.ssl_port},password=${azurerm_redis_cache.tripilot.primary_access_key},ssl=True"
      }
    }

    max_replicas = 10
    min_replicas = 2
  }

  ingress {
    allow_insecure_connections = false
    external_enabled          = true
    target_port               = 80

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }
}

# CDN for static assets
resource "azurerm_cdn_profile" "tripilot" {
  name                = "cdn-tripilot-${var.environment}"
  location            = azurerm_resource_group.tripilot.location
  resource_group_name = azurerm_resource_group.tripilot.name
  sku                 = "Standard_Microsoft"
}
```

### CI/CD Pipeline (GitHub Actions)

```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main]

jobs:
  test-backend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore dependencies
        run: dotnet restore server/Tripilot.sln
      
      - name: Run tests
        run: dotnet test server/Tripilot.sln --configuration Release --no-restore --verbosity normal
        
      - name: Generate coverage report
        run: dotnet test server/Tripilot.sln --configuration Release --no-restore --collect:"XPlat Code Coverage"

  test-frontend:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
          cache: 'npm'
          cache-dependency-path: client/package-lock.json
      
      - name: Install dependencies
        run: npm ci
        working-directory: client
      
      - name: Run tests
        run: npm test -- --coverage --watchAll=false
        working-directory: client
        
      - name: Run e2e tests
        run: npm run test:e2e
        working-directory: client

  build-and-deploy:
    needs: [test-backend, test-frontend]
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    
    steps:
      - uses: actions/checkout@v3
      
      - name: Login to Azure Container Registry
        uses: azure/docker-login@v1
        with:
          login-server: ${{ secrets.ACR_LOGIN_SERVER }}
          username: ${{ secrets.ACR_USERNAME }}
          password: ${{ secrets.ACR_PASSWORD }}
      
      - name: Build and push API image
        run: |
          docker build -t ${{ secrets.ACR_LOGIN_SERVER }}/tripilot-api:${{ github.sha }} -f server/Dockerfile .
          docker push ${{ secrets.ACR_LOGIN_SERVER }}/tripilot-api:${{ github.sha }}
      
      - name: Build and push Client image
        run: |
          docker build -t ${{ secrets.ACR_LOGIN_SERVER }}/tripilot-client:${{ github.sha }} -f client/Dockerfile .
          docker push ${{ secrets.ACR_LOGIN_SERVER }}/tripilot-client:${{ github.sha }}
      
      - name: Deploy to Azure Container Apps
        uses: azure/container-apps-deploy-action@v1
        with:
          containerAppName: ca-tripilot-api-prod
          resourceGroup: rg-tripilot-prod
          imageToDeploy: ${{ secrets.ACR_LOGIN_SERVER }}/tripilot-api:${{ github.sha }}
```

## Scalability & Performance

### Horizontal Scaling Strategy

#### Auto-scaling Configuration
```yaml
# Kubernetes HPA example
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: tripilot-api-hpa
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: tripilot-api
  minReplicas: 2
  maxReplicas: 20
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
  - type: Resource
    resource:
      name: memory
      target:
        type: Utilization
        averageUtilization: 80
```

### Database Scaling

#### Read Replicas Configuration
```csharp
public class DbContextFactory : IDbContextFactory<TripilotDbContext>
{
    private readonly string _writeConnectionString;
    private readonly string[] _readConnectionStrings;
    private readonly Random _random = new Random();

    public TripilotDbContext CreateDbContext()
    {
        return new TripilotDbContext(_writeConnectionString);
    }

    public TripilotDbContext CreateReadOnlyDbContext()
    {
        var connectionString = _readConnectionStrings[_random.Next(_readConnectionStrings.Length)];
        return new TripilotDbContext(connectionString);
    }
}

// Usage in query handlers
public class GetPlacesQueryHandler : IRequestHandler<GetPlacesQuery, GetPlacesResponse>
{
    private readonly IDbContextFactory<TripilotDbContext> _dbContextFactory;

    public async Task<GetPlacesResponse> Handle(GetPlacesQuery request, CancellationToken cancellationToken)
    {
        using var context = _dbContextFactory.CreateReadOnlyDbContext();
        // Query implementation...
    }
}
```

### Monitoring & Observability

#### Application Insights Integration
```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Application Insights
        builder.Services.AddApplicationInsightsTelemetry();

        // Custom telemetry
        builder.Services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();

        // Logging
        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddApplicationInsights();
        });

        var app = builder.Build();

        // Custom middleware for request tracking
        app.UseMiddleware<RequestTrackingMiddleware>();

        app.Run();
    }
}

public class RequestTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTrackingMiddleware> _logger;
    private readonly TelemetryClient _telemetryClient;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            _telemetryClient.TrackRequest(
                context.Request.Path,
                DateTimeOffset.UtcNow.Subtract(stopwatch.Elapsed),
                stopwatch.Elapsed,
                context.Response.StatusCode.ToString(),
                context.Response.StatusCode < 400);

            _logger.LogInformation(
                "Request {Method} {Path} completed in {Duration}ms with status {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                context.Response.StatusCode);
        }
    }
}
```

This comprehensive technical architecture documentation provides the foundation for building, deploying, and scaling the Tripilot application while maintaining high performance, security, and reliability standards.