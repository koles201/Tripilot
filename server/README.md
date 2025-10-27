# Tripilot Backend Server

ASP.NET Core Web API project for the Tripilot tourist route planning and discovery platform.

## Project Structure

The solution follows **Clean Architecture** principles with **CQRS** pattern:

```
server/
├── Tripilot.Api/              # Presentation Layer - Web API
│   ├── Controllers/           # API Controllers
│   ├── Program.cs            # Application entry point
│   └── appsettings.json      # Configuration files
│
├── Tripilot.Application/      # Application Layer - Business Logic
│   ├── Commands/             # Command handlers (CQRS)
│   ├── Queries/              # Query handlers (CQRS)
│   ├── Services/             # Application services
│   ├── Validators/           # FluentValidation validators
│   ├── Mappings/             # AutoMapper profiles
│   └── Interfaces/           # Application interfaces
│
├── Tripilot.Domain/           # Domain Layer - Core Business
│   ├── Entities/             # Domain entities
│   ├── ValueObjects/         # Value objects
│   ├── Events/               # Domain events
│   ├── Exceptions/           # Domain exceptions
│   └── Interfaces/           # Domain interfaces
│
├── Tripilot.Infrastructure/   # Infrastructure Layer - External Concerns
│   ├── Data/                 # Entity Framework context
│   │   └── Configurations/   # Entity configurations
│   ├── Repositories/         # Repository implementations
│   └── Services/             # External service implementations
│
└── Tripilot.Shared/          # Shared Layer - DTOs and Contracts
    ├── DTOs/                 # Data transfer objects
    └── Constants/            # Application constants
```

## Technology Stack

- **.NET 8.0** - Latest LTS version
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 9.0** - ORM
- **PostgreSQL** - Primary database
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Request validation
- **AutoMapper** - Object mapping
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL 15+ (optional for initial development)
- Visual Studio 2022, VS Code, or Rider

### Build the Solution

```powershell
cd server
dotnet build Tripilot.sln
```

### Run the Application

```powershell
cd server/Tripilot.Api
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5139`

### Access Swagger Documentation

Once running, navigate to:
```
http://localhost:5139/swagger
```

## API Endpoints

### Health Check

- **GET** `/health` - Basic health check
- **GET** `/api/health` - Detailed health information
- **GET** `/api/health/detailed` - Extended health diagnostics

## Configuration

Configuration files are located in `Tripilot.Api/`:

- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production settings

### Key Configuration Sections

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tripilot_dev;..."
  },
  "JwtSettings": {
    "SecretKey": "...",
    "Issuer": "TripilotAPI",
    "Audience": "TripilotClient",
    "ExpirationMinutes": 60
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    }
  }
}
```

## Logging

Logs are written to:
- **Console** - For real-time monitoring
- **File** - `logs/tripilot-YYYYMMDD.log` (rolling daily)

Log levels can be configured per environment in `appsettings.{Environment}.json`.

## Architecture Patterns

### CQRS (Command Query Responsibility Segregation)

Commands and queries are separated:
- **Commands** (Application/Commands/) - Modify state
- **Queries** (Application/Queries/) - Read data

### Repository Pattern

Data access is abstracted through repositories:
- `IRepository<T>` - Generic repository interface
- `IUnitOfWork` - Transaction management

### Domain Events

Entities can raise domain events using `BaseEntity.AddDomainEvent()`.

## Dependencies

### Project References

```
Api → Application → Domain
Api → Infrastructure → Domain, Application
Api → Shared
Application → Shared
```

### Key NuGet Packages

**Domain Layer:**
- MediatR.Contracts

**Application Layer:**
- MediatR
- FluentValidation
- AutoMapper

**Infrastructure Layer:**
- Microsoft.EntityFrameworkCore
- Npgsql.EntityFrameworkCore.PostgreSQL

**API Layer:**
- Serilog.AspNetCore
- Swashbuckle.AspNetCore (Swagger)

## Development Guidelines

### Adding a New Feature

1. Create entities in `Tripilot.Domain/Entities/`
2. Define DTOs in `Tripilot.Shared/DTOs/`
3. Create commands/queries in `Tripilot.Application/`
4. Add validators in `Tripilot.Application/Validators/`
5. Implement repositories in `Tripilot.Infrastructure/Repositories/`
6. Create controllers in `Tripilot.Api/Controllers/`

### Code Style

- Follow C# coding conventions
- Use async/await for all I/O operations
- Apply proper error handling
- Write XML documentation comments for public APIs

## Next Steps

- [ ] Configure Entity Framework Core DbContext
- [ ] Set up database migrations
- [ ] Implement authentication and authorization
- [ ] Create user management endpoints
- [ ] Add place management features
- [ ] Implement route planning functionality

## Support

For issues or questions, contact the development team.

## License

Proprietary - All rights reserved
