import pypandoc, os, textwrap

out = "/mnt/data/ASP.NET-Core-10-Clean-Architecture-README.md"

md = r'''# ASP.NET Core 10 Clean Architecture — Step-by-Step Setup Guide

A practical starter guide for building an ASP.NET Core 10 Web API with Clean Architecture, CQRS, MediatR, FluentValidation, Entity Framework Core, SQL Server, and automated tests.

> **Example domain:** Gymnastics Class Management — class sessions, attendees, attendance, and monthly payments. Replace these examples with your own domain as needed.

## Contents

- [1. What Clean Architecture means](#1. What Clean Architecture means)
- [2. Prerequisites](#2-prerequisites)
- [3. Create the solution](#3-create-the-solution)
- [4. Add project references](#4-add-project-references)
- [5. Understand the dependency direction](#5-understand-the-dependency-direction)
- [6. Create the Domain project](#6-create-the-domain-project)
- [7. Create the Application project](#7-create-the-application-project)
- [8. Add CQRS and MediatR](#8-add-cqrs-and-mediatr)
- [9. Add FluentValidation](#9-add-fluentvalidation)
- [10. Create the Infrastructure project](#10-create-the-infrastructure-project)
- [11. Configure the API project](#11-configure-the-api-project)
- [12. Configure SQL Server and EF Core](#12-configure-sql-server-and-ef-core)
- [13. Add exception handling](#13-add-exception-handling)
- [14. Add tests](#14-add-tests)
- [15. Build, run, and verify](#15-build-run-and-verify)
- [16. Suggested feature-folder structure](#16-suggested-feature-folder-structure)
- [17. Recommended implementation order](#17-recommended-implementation-order)
- [18. Common setup problems](#18-common-setup-problems)
- [19. Official references](#19-official-references)

---

## 1. What Clean Architecture means

Clean Architecture separates business rules from delivery mechanisms and infrastructure.

Typical projects:

| Project | Responsibility | Depends on |
|---|---|---|
| `Domain` | Entities, value objects, domain rules, domain events | No other application project |
| `Application` | Use cases, CQRS requests/handlers, validation, interfaces | `Domain` |
| `Infrastructure` | EF Core, SQL Server, repository implementations, external services | `Application` and `Domain` |
| `WebApi` | HTTP endpoints, authentication configuration, middleware, dependency injection | `Application`; references `Infrastructure` for registration |
| `UnitTests` | Domain and application behavior tests | Projects under test |

### Dependency rule

```text
WebApi  ───────────────► Application
  │                           ▲
  └────► Infrastructure ──────┘
                  │
                  ▼
                Domain

Application ───────────► Domain
Infrastructure ───────► Domain (through Application contracts / project references)
```

The important rule is that **Domain must not depend on EF Core, ASP.NET Core, or Infrastructure**. Application contains abstractions; Infrastructure implements infrastructure-specific details.

A common project-reference setup is:

- `Domain`: no project references.
- `Application` → `Domain`.
- `Infrastructure` → `Application` and `Domain`.
- `WebApi` → `Application` and `Infrastructure`.
- `UnitTests` → `Domain` and/or `Application`.

Do not make every project reference every other project.

## 2. Prerequisites

Install:

1. **.NET 10 SDK** — use the official download page: <https://dotnet.microsoft.com/en-us/download/dotnet/10.0>
2. **An editor**:
   - Visual Studio 2026 (version 18.0 or later for .NET 10), or
   - Visual Studio Code with the C# Dev Kit.
3. **SQL Server** — local SQL Server, a development container, or a reachable SQL Server instance.
4. **Git** — recommended for source control.

Verify the SDK:

```powershell
dotnet --info
dotnet --list-sdks
dotnet --version
```

The SDK version should be `10.0.x`. If `dotnet --version` reports a different major version, install the .NET 10 SDK and reopen the terminal.

> The .NET SDK includes the runtime required to build and run .NET applications locally. Production servers can use the appropriate runtime or a self-contained deployment.

## 3. Create the solution

Open PowerShell in the directory where you keep your source code.

```powershell
mkdir GymnasticsManagement
cd GymnasticsManagement

dotnet new sln -n GymnasticsManagement
mkdir src
mkdir tests

dotnet new classlib -n Gymnastics.Domain -o src/Gymnastics.Domain -f net10.0
dotnet new classlib -n Gymnastics.Application -o src/Gymnastics.Application -f net10.0
dotnet new classlib -n Gymnastics.Infrastructure -o src/Gymnastics.Infrastructure -f net10.0
dotnet new webapi -n Gymnastics.WebApi -o src/Gymnastics.WebApi -f net10.0
dotnet new xunit -n Gymnastics.UnitTests -o tests/Gymnastics.UnitTests -f net10.0
```

Add projects to the solution. Recent SDKs may create either `.sln` or `.slnx`; use the solution file that was actually created.

```powershell
dotnet sln add src/Gymnastics.Domain/Gymnastics.Domain.csproj
dotnet sln add src/Gymnastics.Application/Gymnastics.Application.csproj
dotnet sln add src/Gymnastics.Infrastructure/Gymnastics.Infrastructure.csproj
dotnet sln add src/Gymnastics.WebApi/Gymnastics.WebApi.csproj
dotnet sln add tests/Gymnastics.UnitTests/Gymnastics.UnitTests.csproj
```

If your SDK created an `.slnx` and the command does not locate the solution automatically, specify it explicitly, for example:

```powershell
dotnet sln GymnasticsManagement.slnx add src/Gymnastics.Domain/Gymnastics.Domain.csproj
```

Check the solution:

```powershell
dotnet sln list
```

## 4. Add project references

Run these commands from the solution root:

```powershell
dotnet add src/Gymnastics.Application/Gymnastics.Application.csproj reference src/Gymnastics.Domain/Gymnastics.Domain.csproj

dotnet add src/Gymnastics.Infrastructure/Gymnastics.Infrastructure.csproj reference src/Gymnastics.Application/Gymnastics.Application.csproj
dotnet add src/Gymnastics.Infrastructure/Gymnastics.Infrastructure.csproj reference src/Gymnastics.Domain/Gymnastics.Domain.csproj

dotnet add src/Gymnastics.WebApi/Gymnastics.WebApi.csproj reference src/Gymnastics.Application/Gymnastics.Application.csproj
dotnet add src/Gymnastics.WebApi/Gymnastics.WebApi.csproj reference src/Gymnastics.Infrastructure/Gymnastics.Infrastructure.csproj

dotnet add tests/Gymnastics.UnitTests/Gymnastics.UnitTests.csproj reference src/Gymnastics.Domain/Gymnastics.Domain.csproj
dotnet add tests/Gymnastics.UnitTests/Gymnastics.UnitTests.csproj reference src/Gymnastics.Application/Gymnastics.Application.csproj
```

Inspect project references:

```powershell
dotnet list src/Gymnastics.Application/Gymnastics.Application.csproj reference
dotnet list src/Gymnastics.Infrastructure/Gymnastics.Infrastructure.csproj reference
dotnet list src/Gymnastics.WebApi/Gymnastics.WebApi.csproj reference
```

## 5. Understand the dependency direction

Use this mental model:

- **Domain:** What are the core business concepts and rules?
- **Application:** What can a user or system do?
- **Infrastructure:** How are data and external systems accessed?
- **WebApi:** How do HTTP clients call the application?

Example request:

```text
HTTP POST /api/class-sessions
        │
        ▼
WebApi endpoint/controller
        │
        ▼
CreateClassSessionCommand
        │
        ▼
Validation → Handler
        │
        ▼
IClassSessionRepository
        │
        ▼
Infrastructure implementation → SQL Server
```

The Application layer should depend on a repository interface, not on a concrete SQL Server repository.

## 6. Create the Domain project

Delete the template `Class1.cs`. Add a `Entities` folder and create `ClassSession.cs`.

```csharp
namespace Gymnastics.Domain.Entities;

public sealed class ClassSession
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public DateTime StartsAtUtc { get; private set; }
    public int DurationMinutes { get; private set; }
    public decimal MonthlyFee { get; private set; }

    private ClassSession() { } // For ORM materialization

    public ClassSession(
        string name,
        DateTime startsAtUtc,
        int durationMinutes,
        decimal monthlyFee)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Session name is required.", nameof(name));

        if (durationMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(durationMinutes));

        if (monthlyFee <= 0)
            throw new ArgumentOutOfRangeException(nameof(monthlyFee));

        Name = name.Trim();
        StartsAtUtc = DateTime.SpecifyKind(startsAtUtc, DateTimeKind.Utc);
        DurationMinutes = durationMinutes;
        MonthlyFee = monthlyFee;
    }
}
```

**Design notes:**

- Keep business invariants in the Domain where they must always hold.
- FluentValidation is useful for validating incoming requests, but it should not be the only protection for important domain invariants.
- `private set` helps control changes to entity state.
- Decide explicitly whether a fee is monthly, per session, or hourly; avoid mixing billing models in one property.
- Store timestamps consistently (UTC is a common choice) and convert to local time at the UI/API boundary as appropriate.

## 7. Create the Application project

Delete `Class1.cs`. Create these folders:

```text
Gymnastics.Application/
├── Abstractions/
│   └── Persistence/
│       └── IClassSessionRepository.cs
├── Features/
│   └── ClassSessions/
│       └── Commands/
│           └── CreateClassSession/
│               ├── CreateClassSessionCommand.cs
│               ├── CreateClassSessionCommandHandler.cs
│               └── CreateClassSessionCommandValidator.cs
└── DependencyInjection.cs
```

### 7.1 Define a repository interface

`Abstractions/Persistence/IClassSessionRepository.cs`

```csharp
using Gymnastics.Domain.Entities;

namespace Gymnastics.Application.Abstractions.Persistence;

public interface IClassSessionRepository
{
    Task AddAsync(
        ClassSession session,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string name,
        DateTime startsAtUtc,
        CancellationToken cancellationToken = default);
}
```

This interface belongs in Application because it is a contract the use case needs. The implementation will live in Infrastructure.

### 7.2 Add Application packages

```powershell
dotnet add src/Gymnastics.Application/Gymnastics.Application.csproj package MediatR
dotnet add src/Gymnastics.Application/Gymnastics.Application.csproj package FluentValidation
dotnet add src/Gymnastics.Application/Gymnastics.Application.csproj package Microsoft.Extensions.DependencyInjection.Abstractions
```

Package APIs can evolve. Follow the installed MediatR and FluentValidation versions when wiring registration and pipeline behaviors.

### 7.3 Create the command

`Features/ClassSessions/Commands/CreateClassSession/CreateClassSessionCommand.cs`

```csharp
using MediatR;

namespace Gymnastics.Application.Features.ClassSessions.Commands.CreateClassSession;

public sealed record CreateClassSessionCommand(
    string Name,
    DateTime StartsAtUtc,
    int DurationMinutes,
    decimal MonthlyFee) : IRequest<Guid>;
```

A command represents a request to change application state. `IRequest<Guid>` means the handler returns the created entity's ID.

### 7.4 Create the validator

`CreateClassSessionCommandValidator.cs`

```csharp
using FluentValidation;

namespace Gymnastics.Application.Features.ClassSessions.Commands.CreateClassSession;

public sealed class CreateClassSessionCommandValidator
    : AbstractValidator<CreateClassSessionCommand>
{
    public CreateClassSessionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.StartsAtUtc)
            .NotEmpty();

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .LessThanOrEqualTo(240);

        RuleFor(x => x.MonthlyFee)
            .GreaterThan(0);
    }
}
```

These are example constraints, not universal gymnastics business rules. Confirm limits such as maximum duration with the business owner.

### 7.5 Create the handler

`CreateClassSessionCommandHandler.cs`

```csharp
using Gymnastics.Application.Abstractions.Persistence;
using Gymnastics.Domain.Entities;
using MediatR;

namespace Gymnastics.Application.Features.ClassSessions.Commands.CreateClassSession;

public sealed class CreateClassSessionCommandHandler(
    IClassSessionRepository repository)
    : IRequestHandler<CreateClassSessionCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateClassSessionCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await repository.ExistsAsync(
            request.Name,
            request.StartsAtUtc,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "A session with the same name and start time already exists.");
        }

        var session = new ClassSession(
            request.Name,
            request.StartsAtUtc,
            request.DurationMinutes,
            request.MonthlyFee);

        await repository.AddAsync(session, cancellationToken);

        return session.Id;
    }
}
```

For a production application, replace `InvalidOperationException` with a meaningful application exception or result type and map it consistently to an API response. A database unique constraint should also protect uniqueness where required, because two concurrent requests can pass an application-level check at the same time.

### 7.6 Register Application services

Create `DependencyInjection.cs`:

```csharp
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Gymnastics.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
```

This registers MediatR handlers and FluentValidation validators from the Application assembly. It does **not** yet execute validators automatically before each MediatR handler. Add a MediatR pipeline behavior (recommended) or validate explicitly in handlers.

## 8. Add CQRS and MediatR

CQRS separates commands that change state from queries that read state.

```text
Commands                         Queries
CreateClassSession               GetClassSessions
UpdateClassSession               GetClassSessionById
CancelClassSession               GetAttendanceReport
MarkAttendance
```

Each request generally has its own handler. MediatR dispatches the request to the registered handler; MediatR is an implementation choice, not the definition of CQRS.

Recommended feature structure:

```text
Features/
├── ClassSessions/
│   ├── Commands/
│   │   ├── CreateClassSession/
│   │   ├── UpdateClassSession/
│   │   └── CancelClassSession/
│   └── Queries/
│       ├── GetClassSessions/
│       └── GetClassSessionById/
├── Attendance/
│   └── Commands/MarkAttendance/
└── Payments/
    └── Queries/GetPaymentHistory/
```

Start with one simple feature end-to-end. Add more features when you need them rather than creating empty folders for every possible future requirement.

## 9. Add FluentValidation

### 9.1 Recommended: validate with a MediatR pipeline behavior

This avoids manually repeating validation code in every handler. Add a behavior such as `ValidationBehavior<TRequest, TResponse>` that:

1. Resolves all `IValidator<TRequest>` validators.
2. Runs them asynchronously.
3. Collects validation failures.
4. Throws a validation exception or returns a defined failure result.
5. Calls the next handler only when validation succeeds.

The exact MediatR pipeline interface and registration should match the MediatR package version installed in your project. Register the behavior before relying on automatic validation.

### 9.2 Validation vs. domain rules

- **Request validation:** required fields, format, length, sensible ranges.
- **Domain rules:** conditions that must remain true regardless of where the entity is created or updated.
- **Database constraints:** unique indexes, foreign keys, and other integrity guarantees.
- **Application errors:** expected failures such as a missing resource or a conflicting operation.

Do not put HTTP status codes in Application exceptions. Let WebApi translate application failures to HTTP responses.

## 10. Create the Infrastructure project

Delete `Class1.cs`. Add the EF Core SQL Server provider and design package:

```powershell
dotnet add src/Gymnastics.Infrastructure/Gymnastics.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/Gymnastics.Infrastructure/Gymnastics.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design
```

Use compatible EF Core 10 package versions for a .NET 10 project. Keep EF Core packages aligned rather than mixing major versions.

### 10.1 Create the DbContext

`Persistence/GymnasticsDbContext.cs`

```csharp
using Gymnastics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gymnastics.Infrastructure.Persistence;

public sealed class GymnasticsDbContext(
    DbContextOptions<GymnasticsDbContext> options)
    : DbContext(options)
{
    public DbSet<ClassSession> ClassSessions => Set<ClassSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClassSession>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.MonthlyFee).HasPrecision(10, 2);
            entity.HasIndex(x => new { x.Name, x.StartsAtUtc });
        });
    }
}
```

The index shown is not unique by default. If the business rule requires uniqueness, configure a unique index after confirming the exact rule and how case/whitespace should be handled.

### 10.2 Implement the repository

`Persistence/Repositories/ClassSessionRepository.cs`

```csharp
using Gymnastics.Application.Abstractions.Persistence;
using Gymnastics.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gymnastics.Infrastructure.Persistence.Repositories;

public sealed class ClassSessionRepository(
    GymnasticsDbContext dbContext)
    : IClassSessionRepository
{
    public async Task AddAsync(
        ClassSession session,
        CancellationToken cancellationToken = default)
    {
        await dbContext.ClassSessions.AddAsync(session, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(
        string name,
        DateTime startsAtUtc,
        CancellationToken cancellationToken = default)
    {
        return dbContext.ClassSessions.AnyAsync(
            x => x.Name == name && x.StartsAtUtc == startsAtUtc,
            cancellationToken);
    }
}
```

### 10.3 Register Infrastructure services

Create `DependencyInjection.cs` in Infrastructure:

```csharp
using Gymnastics.Application.Abstractions.Persistence;
using Gymnastics.Infrastructure.Persistence;
using Gymnastics.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gymnastics.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("GymnasticsDatabase")
            ?? throw new InvalidOperationException(
                "Connection string 'GymnasticsDatabase' was not found.");

        services.AddDbContext<GymnasticsDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IClassSessionRepository, ClassSessionRepository>();

        return services;
    }
}
```

If you prefer, put all EF Core entity configurations in separate `IEntityTypeConfiguration<T>` classes as the model grows.

## 11. Configure the API project

### 11.1 Configure `appsettings.Development.json`

For local development, add a connection string. Prefer user secrets or environment variables for credentials rather than committing passwords to source control.

```json
{
  "ConnectionStrings": {
    "GymnasticsDatabase": "Server=(localdb)\\MSSQLLocalDB;Database=GymnasticsManagementDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

This example uses SQL Server LocalDB on Windows. For SQL Server Express, Docker, or a remote server, use the appropriate server name and authentication configuration.

Initialize services in `Program.cs`:

```csharp
using Gymnastics.Application;
using Gymnastics.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

The default .NET 10 Web API template may use Minimal APIs and may have a slightly different OpenAPI setup. Adapt this snippet to the actual template generated by your SDK. Add authentication and authorization policies before exposing protected endpoints.

### 11.2 Add a controller

Create `Controllers/ClassSessionsController.cs`:

```csharp
using Gymnastics.Application.Features.ClassSessions.Commands.CreateClassSession;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gymnastics.WebApi.Controllers;

[ApiController]
[Route("api/class-sessions")]
public sealed class ClassSessionsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateClassSessionCommand command,
        CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);

        return Created($"/api/class-sessions/{id}", new { id });
    }
}
```

This demonstrates the request flow; it is not a complete production API. Add a GET endpoint, response DTOs, consistent error handling, authorization, and API contract tests as appropriate.

## 12. Configure SQL Server and EF Core

Install the EF Core CLI tool if it is not already available:

```powershell
dotnet tool install --global dotnet-ef
```

If already installed, update it:

```powershell
dotnet tool update --global dotnet-ef
```

Create the first migration from the solution root:

```powershell
dotnet ef migrations add InitialCreate `
  --project src/Gymnastics.Infrastructure/Gymnastics.Infrastructure.csproj `
  --startup-project src/Gymnastics.WebApi/Gymnastics.WebApi.csproj `
  --output-dir Persistence/Migrations
```

Apply the migration:

```powershell
dotnet ef database update `
  --project src/Gymnastics.Infrastructure/Gymnastics.Infrastructure.csproj `
  --startup-project src/Gymnastics.WebApi/Gymnastics.WebApi.csproj
```

If design-time creation fails, verify that:

- The WebApi project is the startup project.
- The connection string exists in the active environment.
- Infrastructure references the EF Core provider.
- The `DbContext` can be created from the startup project's dependency injection configuration.
- `dotnet-ef` and EF Core packages are compatible.

For production, prefer a controlled migration/deployment process rather than automatically applying schema changes on every application startup.

## 13. Add exception handling

Create application exceptions such as:

- `NotFoundException` — requested resource does not exist.
- `BadRequestException` — a known application rule prevents processing.
- `ValidationException` — one or more input rules failed.

A custom `NotFoundException` can look like this:

```csharp
namespace Gymnastics.Application.Exceptions;

public sealed class NotFoundException(string message)
    : Exception(message);
```

A production error-handling middleware or ASP.NET Core exception handler should translate known errors into consistent Problem Details responses:

| Error | Typical HTTP response |
|---|---:|
| Invalid request / validation failure | 400 or 422, by API convention |
| Resource not found | 404 |
| Unexpected unhandled error | 500 |

Choose and document one validation response convention. Do not expose stack traces, connection strings, SQL errors, or sensitive internal details to clients in production. Log unexpected exceptions on the server with a correlation/trace identifier.

## 14. Add tests

Run the generated tests:

```powershell
dotnet test
```

Recommended test coverage:

- **Domain unit tests:** invalid duration, invalid fee, required session name.
- **Validator tests:** empty name, excessive length, invalid duration, non-positive fee.
- **Handler tests:** repository call on valid request; no save on validation or conflict.
- **Infrastructure integration tests:** EF Core mapping, migrations, database constraints.
- **API tests:** status codes, response shape, validation errors, and not-found behavior.

Keep most Domain and Application tests fast and isolated. Use a real test database or a suitable disposable database for EF Core integration tests where database-specific behavior matters.

## 15. Build, run, and verify

From the solution root:

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project src/Gymnastics.WebApi/Gymnastics.WebApi.csproj
```

Use the HTTPS URL printed by the application. If OpenAPI is configured, open the corresponding `/openapi/v1.json` endpoint or the UI configured by your template.

Example request body for `POST /api/class-sessions`:

```json
{
  "name": "Saturday Beginners",
  "startsAtUtc": "2026-11-07T09:00:00Z",
  "durationMinutes": 60,
  "monthlyFee": 35.00
}
```

The example uses a fee value of `35.00` only as sample data; confirm the currency and billing model in your own requirements. The example API request will not provide complete CRUD functionality until you add the other handlers and endpoints.

## 16. Suggested feature-folder structure

```text
GymnasticsManagement/
├── GymnasticsManagement.slnx   # or .sln, depending on SDK
├── src/
│   ├── Gymnastics.Domain/
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   └── DomainEvents/
│   ├── Gymnastics.Application/
│   │   ├── Abstractions/
│   │   ├── Exceptions/
│   │   ├── Features/
│   │   │   ├── ClassSessions/
│   │   │   ├── Attendees/
│   │   │   ├── Attendance/
│   │   │   └── Payments/
│   │   └── DependencyInjection.cs
│   ├── Gymnastics.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── Configurations/
│   │   │   ├── Migrations/
│   │   │   └── Repositories/
│   │   └── DependencyInjection.cs
│   └── Gymnastics.WebApi/
│       ├── Controllers/
│       ├── Middleware/
│       ├── Program.cs
│       └── appsettings.json
└── tests/
    ├── Gymnastics.UnitTests/
    ├── Gymnastics.IntegrationTests/
    └── Gymnastics.ArchitectureTests/
```

Within each Application feature, group the request, handler, validator, and feature-specific response model together. Avoid a giant generic `Services` folder containing unrelated use cases.

## 17. Recommended implementation order

1. Confirm business requirements and terminology.
2. Create the solution and project references.
3. Define the first Domain entity and its invariants.
4. Define an Application command/query and repository interface.
5. Add a handler and FluentValidation rules.
6. Implement the repository and EF Core mapping in Infrastructure.
7. Configure dependency injection and the WebApi endpoint.
8. Add migrations and apply them to a development database.
9. Add centralized exception handling.
10. Write unit, integration, and API tests.
11. Add authentication, authorization, logging, health checks, and deployment configuration as needed.

## 18. Common setup problems

| Problem | Check |
|---|---|
| `dotnet` command not found | Install .NET 10 SDK and reopen the terminal |
| Wrong SDK version | Run `dotnet --list-sdks`; install SDK 10 |
| Visual Studio cannot target `net10.0` | Use a supported Visual Studio version for .NET 10 |
| Project reference error | Check relative paths and project-reference direction |
| MediatR cannot find a handler | Ensure Application assembly registration runs and handler is public |
| Validator never runs | Register validators and a validation pipeline behavior, or call validation explicitly |
| SQL connection failure | Check server name, service status, authentication, database access, and connection string |
| EF migration cannot create DbContext | Check startup project, provider package, configuration, and design-time setup |
| Duplicate records under concurrent requests | Add the appropriate database unique constraint and handle constraint violations |
| Validation returns HTTP 500 | Add centralized exception mapping for validation errors |
| Secrets committed to Git | Move development secrets to user secrets/environment variables and rotate exposed credentials |

## 19. Official references

- [.NET 10 downloads and SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Install .NET on Windows — Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/install/windows)
- [ASP.NET Core documentation](https://learn.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core documentation](https://learn.microsoft.com/en-us/ef/core/)
- [Clean Architecture Solution Template documentation](https://cleanarchitecture.jasontaylor.dev/docs/getting-started/installation/)

---

## Scope note

This README is a **step-by-step starter guide**, not a complete production-ready repository. The snippets illustrate the architecture and first use case; you must add the remaining features, centralized validation behavior, error handling, security configuration, database constraints, and tests before production deployment.
'''

pypandoc.convert_text(md, 'md', format='md', outputfile=out, extra_args=['--standalone'])
print(f"Created: {out}")
print(f"Size: {os.path.getsize(out):,} bytes")
