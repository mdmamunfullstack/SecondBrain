# Clean Architecture: Entity Framework Core & Infrastructure Layer

A step-by-step implementation guide for adding **Entity Framework Core persistence** and an **Infrastructure layer** to a Clean Architecture solution using .NET 10, repositories, dependency injection, and an email service abstraction.

> **Example solution:** `GloboTicket.TicketManagement`  
> **Database provider in this guide:** SQLite  
> The examples are templates. Adjust namespaces, entity properties, repository contracts, and command-handler return types to match your actual solution.

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [Project Structure](#2-project-structure)
3. [Create the Projects and References](#3-create-the-projects-and-references)
4. [Install EF Core Packages](#4-install-ef-core-packages)
5. [Create the DbContext](#5-create-the-dbcontext)
6. [Configure Entities](#6-configure-entities)
7. [Add Seed Data](#7-add-seed-data)
8. [Implement Audit Properties](#8-implement-audit-properties)
9. [Implement the Generic Repository](#9-implement-the-generic-repository)
10. [Implement Specific Repositories](#10-implement-specific-repositories)
11. [Register Persistence Services](#11-register-persistence-services)
12. [Add the Email Service Contract](#12-add-the-email-service-contract)
13. [Implement the Infrastructure Email Service](#13-implement-the-infrastructure-email-service)
14. [Register Infrastructure Services](#14-register-infrastructure-services)
15. [Configure the API](#15-configure-the-api)
16. [Create and Apply Migrations](#16-create-and-apply-migrations)
17. [Test the API End to End](#17-test-the-api-end-to-end)
18. [Troubleshooting](#18-troubleshooting)
19. [Implementation Checklist](#19-implementation-checklist)
20. [Design Notes](#20-design-notes)

---

## 1. Architecture Overview

The Application layer defines what the system needs through interfaces (contracts). Persistence and Infrastructure provide concrete implementations.

```text
Presentation / API
  ├── Controllers
  ├── Program.cs
  └── appsettings.json
          |
          v
Application
  ├── Features / Commands / Queries / Handlers
  ├── Validators
  └── Contracts
       ├── Persistence
       └── Infrastructure
          ^                  ^
          |                  |
Persistence              Infrastructure
  ├── DbContext             └── Email / external services
  ├── EF Core
  └── Repositories
```

### Dependency rules

- **Domain** must not depend on Persistence or Infrastructure.
- **Application** depends on Domain and defines the abstractions it needs.
- **Persistence** implements Application's persistence contracts.
- **Infrastructure** implements Application's external-service contracts.
- **API** acts as the composition root and registers the implementations with dependency injection.

## 2. Project Structure

A suggested solution structure:

```text
GloboTicket.TicketManagement/
├── GloboTicket.TicketManagement.Domain/
│   ├── Entities/
│   │   ├── Event.cs
│   │   ├── Category.cs
│   │   └── Order.cs
│   └── Common/
│       └── AuditableEntity.cs
│
├── GloboTicket.TicketManagement.Application/
│   ├── Contracts/
│   │   ├── Persistence/
│   │   │   ├── IAsyncRepository.cs
│   │   │   ├── IEventRepository.cs
│   │   │   ├── ICategoryRepository.cs
│   │   │   └── IOrderRepository.cs
│   │   └── Infrastructure/
│   │       └── IEmailService.cs
│   ├── Models/
│   │   └── Mail/
│   │       └── Email.cs
│   └── Features/
│       └── Events/
│           └── Commands/
│               └── CreateEvent/
│
├── GloboTicket.TicketManagement.Persistence/
│   ├── GloboTicketDbContext.cs
│   ├── Configurations/
│   │   ├── EventConfiguration.cs
│   │   └── CategoryConfiguration.cs
│   ├── Repositories/
│   │   ├── BaseRepository.cs
│   │   ├── EventRepository.cs
│   │   ├── CategoryRepository.cs
│   │   └── OrderRepository.cs
│   ├── Migrations/
│   └── PersistenceServiceRegistration.cs
│
├── GloboTicket.TicketManagement.Infrastructure/
│   ├── Mail/
│   │   └── EmailService.cs
│   └── InfrastructureServiceRegistration.cs
│
└── GloboTicket.TicketManagement.Api/
    ├── Controllers/
    ├── appsettings.json
    └── Program.cs
```

## 3. Create the Projects and References

Create two .NET class libraries:

- `GloboTicket.TicketManagement.Persistence`
- `GloboTicket.TicketManagement.Infrastructure`

Using PowerShell or a terminal from the solution directory:

```powershell
dotnet new classlib -n GloboTicket.TicketManagement.Persistence
dotnet new classlib -n GloboTicket.TicketManagement.Infrastructure
```

Add project references. These commands assume the projects are in the current directory; update paths if your solution uses separate folders.

```powershell
dotnet add GloboTicket.TicketManagement.Persistence reference GloboTicket.TicketManagement.Application
dotnet add GloboTicket.TicketManagement.Persistence reference GloboTicket.TicketManagement.Domain

dotnet add GloboTicket.TicketManagement.Infrastructure reference GloboTicket.TicketManagement.Application
```

The Persistence project needs access to the Application contracts and Domain entities. Infrastructure needs access to Application contracts and models.

## 4. Install EF Core Packages

For SQLite:

```powershell
dotnet add GloboTicket.TicketManagement.Persistence package Microsoft.EntityFrameworkCore
dotnet add GloboTicket.TicketManagement.Persistence package Microsoft.EntityFrameworkCore.Sqlite
dotnet add GloboTicket.TicketManagement.Persistence package Microsoft.EntityFrameworkCore.Design
dotnet add GloboTicket.TicketManagement.Persistence package Microsoft.Extensions.Options.ConfigurationExtensions
```

Install the EF Core CLI tool if it is not already installed:

```powershell
dotnet tool install --global dotnet-ef
```

To update an existing global installation:

```powershell
dotnet tool update --global dotnet-ef
```

Keep the EF Core packages and `dotnet-ef` on compatible major versions.

## 5. Create the DbContext

Create `GloboTicketDbContext.cs` in the Persistence project:

```csharp
using GloboTicket.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.TicketManagement.Persistence;

public class GloboTicketDbContext : DbContext
{
    public GloboTicketDbContext(
        DbContextOptions<GloboTicketDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events => Set<Event>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(GloboTicketDbContext).Assembly);
    }
}
```

`DbContext` is EF Core's main unit for working with a database. It manages entity sets, query translation, change tracking, and saving changes.

> Ensure that the `Event`, `Category`, and `Order` entities already exist and that their namespaces match your solution.

## 6. Configure Entities

Keep database mapping out of Domain entities by using `IEntityTypeConfiguration<T>`.

Create `Configurations/EventConfiguration.cs`:

```csharp
using GloboTicket.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GloboTicket.TicketManagement.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}
```

This example assumes `Event` has `Id` and `Name` properties. Adapt it to your actual model.

A one-to-many relationship can be configured like this if the corresponding navigation and foreign-key properties exist:

```csharp
builder.HasMany(c => c.Events)
    .WithOne(e => e.Category)
    .HasForeignKey(e => e.CategoryId);
```

Add configuration for the other entities as needed. Configure keys, relationships, required properties, maximum lengths, decimal precision, indexes, and delete behavior explicitly where appropriate.

## 7. Add Seed Data

You can configure initial data with `HasData()` in `OnModelCreating()` or in an entity configuration:

```csharp
modelBuilder.Entity<Category>().HasData(
    new Category { Id = 1, Name = "Music" },
    new Category { Id = 2, Name = "Sports" });
```

This is an example only. Supply all required properties from your actual `Category` entity.

Notes:

- Seed entities need stable primary keys.
- Changes to `HasData()` are represented through migrations.
- `HasData()` is best for fixed reference data, not user-generated or frequently changing data.

## 8. Implement Audit Properties

If your entities inherit from a common audit base class, you can update audit fields when saving changes.

Example base class:

```csharp
public abstract class AuditableEntity
{
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}
```

Add this override to `GloboTicketDbContext`:

```csharp
using GloboTicket.TicketManagement.Domain.Common;
using Microsoft.EntityFrameworkCore;

public override async Task<int> SaveChangesAsync(
    CancellationToken cancellationToken = default)
{
    var now = DateTime.UtcNow;

    foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
    {
        if (entry.State == EntityState.Added)
        {
            entry.Entity.CreatedDate = now;
        }
        else if (entry.State == EntityState.Modified)
        {
            entry.Entity.LastModifiedDate = now;
            entry.Property(x => x.CreatedDate).IsModified = false;
        }
    }

    return await base.SaveChangesAsync(cancellationToken);
}
```

Use the actual property names from your Domain model. Consider whether your application needs UTC timestamps, a different clock abstraction, or audit handling for deletes.

## 9. Implement the Generic Repository

The Application project should define a repository contract. If your solution already has `IAsyncRepository<T>`, keep its existing method signatures and implement those instead of introducing a conflicting interface.

Example contract:

```csharp
public interface IAsyncRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

Create `Repositories/BaseRepository.cs` in Persistence:

```csharp
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.TicketManagement.Persistence.Repositories;

public class BaseRepository<T> : IAsyncRepository<T>
    where T : class
{
    protected readonly GloboTicketDbContext _context;

    public BaseRepository(GloboTicketDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}
```

This is a minimal example. Match return types, cancellation tokens, and other signatures to the Application contract in your solution.

### Repository design note

Saving inside each repository method is simple for a starter implementation. For workflows that change multiple entities, consider a Unit of Work or an explicit transaction boundary so related changes can be committed together. Avoid duplicating transaction management across repositories.

## 10. Implement Specific Repositories

Generic repositories handle common operations. Specific repositories handle entity-specific queries.

### 10.1 Event repository

Example Application contract:

```csharp
public interface IEventRepository : IAsyncRepository<Event>
{
    Task<bool> IsEventNameAndDateUnique(string name, DateTime date);
}
```

Example Persistence implementation:

```csharp
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.TicketManagement.Persistence.Repositories;

public class EventRepository : BaseRepository<Event>, IEventRepository
{
    public EventRepository(GloboTicketDbContext context) : base(context)
    {
    }

    public async Task<bool> IsEventNameAndDateUnique(
        string name,
        DateTime date)
    {
        return !await _context.Events.AnyAsync(e =>
            e.Name == name && e.Date == date);
    }
}
```

This assumes the entity has `Name` and `Date` properties. Use the actual property names in your project.

For concurrent requests, application-level validation alone may not prevent duplicates. Consider a matching unique database index where the business rules permit it.

### 10.2 Category repository

Example Application contract:

```csharp
public interface ICategoryRepository : IAsyncRepository<Category>
{
    Task<IReadOnlyList<Category>> GetCategoriesWithEvents(
        bool includePastEvents);
}
```

Example implementation:

```csharp
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloboTicket.TicketManagement.Persistence.Repositories;

public class CategoryRepository
    : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(GloboTicketDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Category>> GetCategoriesWithEvents(
        bool includePastEvents)
    {
        var query = _context.Categories.AsQueryable();

        if (includePastEvents)
        {
            query = query.Include(c => c.Events);
        }
        else
        {
            query = query.Include(c =>
                c.Events.Where(e => e.Date >= DateTime.UtcNow));
        }

        return await query.ToListAsync();
    }
}
```

This assumes `Category.Events` is a navigation collection and `Event.Date` exists. Check provider translation and the desired date/time semantics. For read-only use cases, consider `AsNoTracking()` and projection to DTOs.

### 10.3 Order repository

Create `IOrderRepository` in Application and `OrderRepository` in Persistence. Put Order-specific query contracts in the interface, such as looking up orders by customer or payment status, and implement those queries using EF Core in Persistence.

Keep business rules in Application/Domain rather than embedding them in the database repository.

## 11. Register Persistence Services

Create `PersistenceServiceRegistration.cs`:

```csharp
using GloboTicket.TicketManagement.Application.Contracts.Persistence;
using GloboTicket.TicketManagement.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GloboTicket.TicketManagement.Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<GloboTicketDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped(
            typeof(IAsyncRepository<>),
            typeof(BaseRepository<>));

        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
```

Make sure all listed interfaces and implementations exist. `AddDbContext()` uses a scoped lifetime by default, which is appropriate for typical ASP.NET Core request processing. A `DbContext` is not thread-safe; do not run parallel operations on the same instance.

## 12. Add the Email Service Contract

Create `Contracts/Infrastructure/IEmailService.cs` in Application:

```csharp
using GloboTicket.TicketManagement.Application.Models.Mail;

namespace GloboTicket.TicketManagement.Application.Contracts.Infrastructure;

public interface IEmailService
{
    Task SendEmail(Email email);
}
```

Create `Models/Mail/Email.cs` in Application:

```csharp
namespace GloboTicket.TicketManagement.Application.Models.Mail;

public class Email
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
```

`Email` is an Application model, not a Domain entity. It describes the data the Application needs to pass to the email service.

## 13. Implement the Infrastructure Email Service

Add the logging abstractions package:

```powershell
dotnet add GloboTicket.TicketManagement.Infrastructure package Microsoft.Extensions.Logging.Abstractions
```

Create `Mail/EmailService.cs`:

```csharp
using GloboTicket.TicketManagement.Application.Contracts.Infrastructure;
using GloboTicket.TicketManagement.Application.Models.Mail;
using Microsoft.Extensions.Logging;

namespace GloboTicket.TicketManagement.Infrastructure.Mail;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmail(Email email)
    {
        _logger.LogInformation(
            "Email notification. To: {To}, Subject: {Subject}",
            email.To,
            email.Subject);

        // This demo implementation logs the notification;
        // it does not send a real email.
        return Task.CompletedTask;
    }
}
```

Do not log sensitive email content or personal data unnecessarily. In production, replace this fake implementation with an actual email provider or queue-based sender.

## 14. Register Infrastructure Services

Create `InfrastructureServiceRegistration.cs`:

```csharp
using GloboTicket.TicketManagement.Application.Contracts.Infrastructure;
using GloboTicket.TicketManagement.Infrastructure.Mail;
using Microsoft.Extensions.DependencyInjection;

namespace GloboTicket.TicketManagement.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddTransient<IEmailService, EmailService>();
        return services;
    }
}
```

The email service is registered as transient. Choose a lifetime appropriate to the real provider and its dependencies.

## 15. Configure the API

In the API project's `appsettings.json`, add a connection string:

```json
{
  "ConnectionStrings": {
    "GloboTicketConnectionString": "Data Source=GloboTicket.db"
  }
}
```

For production, store secrets and environment-specific settings securely rather than committing sensitive values to source control.

Example `Program.cs` setup:

```csharp
using GloboTicket.TicketManagement.Infrastructure;
using GloboTicket.TicketManagement.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString =
    builder.Configuration.GetConnectionString(
        "GloboTicketConnectionString")
    ?? throw new InvalidOperationException(
        "Database connection string is missing.");

builder.Services.AddPersistenceServices(connectionString);
builder.Services.AddInfrastructureServices();

// Register your existing Application services too.
// For example: builder.Services.AddApplicationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

Keep your existing authentication, authorization, MediatR, AutoMapper, and other service registrations. The snippet shows only the relevant setup.

## 16. Create and Apply Migrations

Run commands from the solution directory. The examples assume the Persistence and API project folders are located directly under the current directory.

Create the initial migration:

```powershell
dotnet ef migrations add InitialCreate `
  --project GloboTicket.TicketManagement.Persistence `
  --startup-project GloboTicket.TicketManagement.Api `
  --context GloboTicketDbContext `
  --output-dir Migrations
```

Apply it to the database:

```powershell
dotnet ef database update `
  --project GloboTicket.TicketManagement.Persistence `
  --startup-project GloboTicket.TicketManagement.Api `
  --context GloboTicketDbContext
```

Parameters:

- `--project`: project that contains the DbContext and migration files.
- `--startup-project`: executable project that provides configuration and dependency injection.
- `--context`: DbContext to use.
- `--output-dir`: folder for generated migrations.

If the projects are nested in different folders, update the paths. If you have multiple DbContexts, specify the intended context for each migration command.

### Using SQL Server instead of SQLite

Install the SQL Server provider:

```powershell
dotnet add GloboTicket.TicketManagement.Persistence package Microsoft.EntityFrameworkCore.SqlServer
```

Change registration to:

```csharp
services.AddDbContext<GloboTicketDbContext>(options =>
    options.UseSqlServer(connectionString));
```

Use a SQL Server connection string in configuration. Keep the provider package and EF Core version aligned. Do not switch providers on an existing production database without planning schema compatibility and migration strategy.

## 17. Test the API End to End

Start the API:

```powershell
dotnet run --project GloboTicket.TicketManagement.Api
```

Use Swagger or an HTTP client to call the actual event-creation endpoint. For example, if your route is `POST /api/event`:

```http
POST /api/event
Content-Type: application/json
```

Example body (adapt to your command's required fields):

```json
{
  "name": "Summer Music Festival",
  "date": "2026-11-15T10:00:00",
  "categoryId": 1
}
```

Verify:

| Test | Expected result |
|---|---|
| API starts | Application starts without dependency-injection errors |
| Database migration | Expected tables are created |
| Create event | Event is persisted |
| Validation | Invalid requests are rejected |
| Duplicate event | Business validation and database constraints behave as expected |
| Category query | Categories and matching events are returned |
| Audit properties | Created/modified timestamps are correct |
| Email notification | A log entry confirms the fake service was invoked |
| Email failure | The chosen error-handling strategy behaves as expected |

A log entry confirms that the fake email service was invoked; it does **not** prove that a real email was delivered.

## 18. Troubleshooting

| Problem | What to check |
|---|---|
| `No DbContext was found` | Verify `--project`, `--startup-project`, and `--context`. |
| Unable to create a `DbContext` at design time | Check the constructor, startup configuration, provider setup, and design-time creation requirements. |
| Missing connection string | Check `appsettings.json`, environment variables, and configuration loading. |
| Tables are missing | Confirm the migration was created and applied to the intended database. |
| Dependency injection cannot resolve a service | Verify the interface, implementation, project reference, and registration method. |
| EF Core package version conflict | Align EF Core packages and `dotnet-ef` versions. |
| SQLite foreign-key errors | Check relationship mappings and referenced IDs. |
| Duplicate records appear | Check validation and consider a suitable unique database index. |

## 19. Implementation Checklist

- [ ] Create Persistence and Infrastructure projects.
- [ ] Add the necessary project references.
- [ ] Install compatible EF Core packages and CLI tools.
- [ ] Create the DbContext and DbSets.
- [ ] Add entity configuration classes.
- [ ] Add seed data where appropriate.
- [ ] Implement audit-field handling.
- [ ] Implement the generic repository.
- [ ] Implement entity-specific repositories.
- [ ] Register Persistence services.
- [ ] Define the `IEmailService` contract and `Email` model.
- [ ] Implement and register the fake email service.
- [ ] Configure the API connection string and service registrations.
- [ ] Create and apply migrations.
- [ ] Test API persistence and notification behavior.
- [ ] Review logging, error handling, and production configuration.

## 20. Design Notes

1. **Separation of concerns:** Domain rules, application workflows, database access, and external integrations have separate responsibilities.
2. **Dependency inversion:** Application code depends on interfaces, not concrete database or email implementations.
3. **Repository boundaries:** Generic repositories provide common operations; specific repositories provide domain-relevant queries.
4. **Database configuration:** Entity configuration classes keep mapping details out of Domain entities.
5. **Dependency injection:** The API composes the implementations at startup.
6. **Transactions:** If a workflow changes multiple records, use a suitable transaction or Unit of Work strategy.
7. **Reliable notifications:** For critical email notifications, consider an Outbox Pattern and background processing rather than relying only on an in-process call.
8. **Testing:** Test Application handlers with mocked contracts and test Persistence mappings/queries against an appropriate test database.

---

## Summary

This guide implements EF Core persistence and an Infrastructure email service while keeping the Application layer independent of concrete providers.

The key principle is:

> **Application defines what it needs; Persistence and Infrastructure implement how those needs are fulfilled.**
