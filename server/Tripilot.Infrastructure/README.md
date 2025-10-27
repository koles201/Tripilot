# Tripilot Infrastructure Layer

This layer contains the data access implementation using Entity Framework Core with PostgreSQL.

## Components

### ApplicationDbContext
- Main database context for Entity Framework Core
- Handles audit fields automatically (CreatedAt, ModifiedAt, CreatedBy, ModifiedBy)
- Manages domain events before saving changes
- Applies all entity configurations from assembly

### Repositories
- **Repository<T>**: Generic repository implementation for CRUD operations
- **UnitOfWork**: Transaction management and coordinated saves

### Database Seeder
- **DatabaseSeeder**: Seeds initial data for development and testing

## Configuration

### Connection String
Configure the PostgreSQL connection string in ppsettings.json:

`json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tripilot_dev;Username=postgres;Password=postgres"
  },
  "DetailedErrors": true
}
`

### Environment-Specific Settings
- **Development**: Detailed errors and sensitive data logging enabled
- **Production**: Optimized for performance and security

## Migrations

### Create a Migration
`ash
cd server/Tripilot.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../Tripilot.Api/Tripilot.Api.csproj --context ApplicationDbContext
`

### Apply Migrations
Migrations are applied automatically on application startup via InitializeDatabaseAsync.

### Remove Last Migration
`ash
dotnet ef migrations remove --startup-project ../Tripilot.Api/Tripilot.Api.csproj --context ApplicationDbContext
`

### Update Database Manually
`ash
dotnet ef database update --startup-project ../Tripilot.Api/Tripilot.Api.csproj --context ApplicationDbContext
`

## PostgreSQL Setup

### Using Docker
`ash
docker run --name tripilot-postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=tripilot_dev -p 5432:5432 -d postgres:16
`

### Local Installation
1. Install PostgreSQL 16 or later
2. Create database: CREATE DATABASE tripilot_dev;
3. Update connection string in appsettings.json

## Features

### Audit Trail
All entities implementing IAuditableEntity automatically track:
- CreatedAt
- CreatedBy
- ModifiedAt
- ModifiedBy

### Domain Events
Entities can raise domain events that are collected and can be published via MediatR after successful save.

### Retry Logic
Configured with retry policy for transient failures:
- Max retry count: 5
- Max retry delay: 30 seconds

### Connection Pooling
Entity Framework Core connection pooling is enabled by default for optimal performance.

## Usage Examples

### Using Repository
`csharp
public class SomeService
{
    private readonly IRepository<User> _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SomeService(IRepository<User> userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return user;
    }
}
`

### Using Unit of Work with Transaction
`csharp
public async Task TransferDataAsync()
{
    await _unitOfWork.BeginTransactionAsync();
    
    try
    {
        // Multiple operations
        await _repository1.AddAsync(entity1);
        await _repository2.UpdateAsync(entity2);
        
        await _unitOfWork.CommitTransactionAsync();
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync();
        throw;
    }
}
`

## Dependencies

- Microsoft.EntityFrameworkCore (9.0.10)
- Microsoft.EntityFrameworkCore.Design (9.0.10)
- Npgsql.EntityFrameworkCore.PostgreSQL (9.0.4)

## Best Practices

1. **Always use IUnitOfWork.SaveChangesAsync()** instead of DbContext.SaveChangesAsync()
2. **Use transactions for multi-step operations** that must succeed or fail together
3. **Create specific repositories** that inherit from Repository<T> for complex entity-specific queries
4. **Use entity configurations** in separate files under Data/Configurations/
5. **Never expose DbContext** outside the Infrastructure layer
