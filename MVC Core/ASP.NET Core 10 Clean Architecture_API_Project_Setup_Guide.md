# ASP.NET Core Web API Project Setup Guide

A step-by-step guide to setting up an ASP.NET Core Web API using Clean Architecture, MediatR, Entity Framework Core, SQLite, OpenAPI, Swagger UI, and SonarQube.

> **Version note:** Examples target modern ASP.NET Core (.NET 8/9/10). Verify package compatibility with your target framework and use matching EF Core major versions. Namespaces, project names, DbContext names, and extension-method signatures may differ in your solution.

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [Create the API Project](#2-create-the-api-project)
3. [Add Project References](#3-add-project-references)
4. [Add the Controllers Folder](#4-add-the-controllers-folder)
5. [Configure the Connection String](#5-configure-the-connection-string)
6. [Create StartupExtensions.cs](#6-create-startupextensionscs)
7. [Configure Program.cs](#7-configure-programcs)
8. [Configure EF Core and SQLite](#8-configure-ef-core-and-sqlite)
9. [Create and Apply a Migration](#9-create-and-apply-a-migration)
10. [Add a Controller Using MediatR](#10-add-a-controller-using-mediatr)
11. [Choose Response Models and HTTP Status Codes](#11-choose-response-models-and-http-status-codes)
12. [Configure OpenAPI and Swagger UI](#12-configure-openapi-and-swagger-ui)
13. [Add SonarQube Code Analysis](#13-add-sonarqube-code-analysis)
14. [Test the API](#14-test-the-api)
15. [Troubleshooting](#15-troubleshooting)
16. [Final Checklist](#16-final-checklist)

---

## 1. Architecture Overview

A typical solution may have these projects:

```text
GloboTicket/
├── GloboTicket.sln
├── GloboTicket.TicketManagement.Domain/
├── GloboTicket.TicketManagement.Application/
├── GloboTicket.TicketManagement.Infrastructure/
├── GloboTicket.TicketManagement.Persistence/
└── GloboTicket.TicketManagement.API/
    ├── Controllers/
    ├── Program.cs
    ├── StartupExtensions.cs
    └── appsettings.json
```

The exact project names can vary.

### Request flow

```text
Client / Blazor UI
        |
        | HTTP + JSON
        v
ASP.NET Core API Controller
        |
        | MediatR Send(Command or Query)
        v
Application Handler
        |
        | Repository / abstraction
        v
Persistence / Infrastructure
        |
        v
Database or external service
        |
        v
DTO / ViewModel -> HTTP response
```

**Responsibilities**

- **API:** HTTP endpoints, authentication/authorization, HTTP status codes, request/response contracts, and middleware.
- **Application:** commands, queries, handlers, validation, DTOs/ViewModels, and application workflows.
- **Domain:** business entities and domain rules.
- **Persistence:** `DbContext`, EF Core configuration, migrations, and repository implementations.
- **Infrastructure:** external integrations such as email services and other infrastructure concerns.

Keep dependencies pointing inward toward the core. The Application layer should not depend on API-specific HTTP types.

## 2. Create the API Project

In Visual Studio:

1. Right-click the solution.
2. Select **Add → New Project**.
3. Choose **ASP.NET Core Web API**.
4. Name it `GloboTicket.TicketManagement.API`.
5. Select the framework used by your solution (for example, .NET 10 if that is your target).
6. Choose **Use controllers**.
7. OpenAPI support can be enabled here or configured in the steps below.
8. Click **Create**.

If the template creates sample Minimal API endpoints and you intend to use controllers, remove the sample endpoints you do not need.

## 3. Add Project References

Right-click the API project → **Add → Project Reference** and select:

- `GloboTicket.TicketManagement.Application`
- `GloboTicket.TicketManagement.Infrastructure`
- `GloboTicket.TicketManagement.Persistence`

Why these references?

| Reference | Purpose |
|---|---|
| Application | Exposes MediatR request types, DTOs, queries, and commands used by API endpoints |
| Infrastructure | Exposes infrastructure service-registration extensions |
| Persistence | Exposes database/repository service-registration extensions |

Do not add references mechanically: keep the dependency direction intentional. The API is the composition root, so it commonly references Application and the projects that register runtime infrastructure.

## 4. Add the Controllers Folder

Create a `Controllers` folder in the API project. Controllers will define HTTP routes and delegate work to Application requests through MediatR.

## 5. Configure the Connection String

Add the connection string to the API project's `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "GloboTicketDatabase": "Data Source=GloboTicket.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

The key `GloboTicketDatabase` must match the key read by your Persistence project's `DbContext` registration.

SQLite resolves a relative database path from the application's working directory. For deployed applications, choose a stable writable path and protect the database file appropriately. Do not store production secrets in source-controlled settings files.

## 6. Create `StartupExtensions.cs`

Create a static class in the API project to keep service registration and middleware configuration separate from `Program.cs`.

The following is a **template**. Adapt namespaces and method signatures to the extension methods already defined in your solution.

```csharp
using GloboTicket.TicketManagement.Application;
using GloboTicket.TicketManagement.Infrastructure;
using GloboTicket.TicketManagement.Persistence;

namespace GloboTicket.TicketManagement.API;

public static class StartupExtensions
{
    public static WebApplicationBuilder ConfigureServices(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddApplicationServices();

        // Adapt these calls to the signatures in your solution.
        builder.Services.AddInfrastructureServices(
            builder.Configuration);

        builder.Services.AddPersistenceServices(
            builder.Configuration);

        return builder;
    }

    public static WebApplication ConfigurePipeline(
        this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi("/openapi/v1.json");
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        return app;
    }
}
```

### Important: match existing extension-method signatures

Your projects may define methods such as:

```csharp
services.AddApplicationServices();
services.AddInfrastructureServices(configuration);
services.AddPersistenceServices(configuration);
```

If your existing methods do not accept `IConfiguration`, call them without that argument. Do not create duplicate registrations just to match the example.

Typical registrations include:

- Application: MediatR and AutoMapper.
- Infrastructure: implementations such as `IEmailService`.
- Persistence: EF Core `DbContext` and repository implementations.

Use the `AddOpenApi()` method only when the corresponding OpenAPI package is installed and supported by your target framework.

## 7. Configure `Program.cs`

Use `Program.cs` as the application entry point:

```csharp
using GloboTicket.TicketManagement.API;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

app.ConfigurePipeline();

app.Run();
```

The startup sequence is:

1. Create the builder.
2. Register services.
3. Build the application.
4. Configure middleware and endpoints.
5. Run the application.

## 8. Configure EF Core and SQLite

In the Persistence project, verify that the SQLite provider and design-time dependencies are installed. Install only packages that are missing, and keep EF Core packages on compatible versions.

Package Manager Console examples:

```powershell
Install-Package Microsoft.EntityFrameworkCore.Sqlite
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Microsoft.EntityFrameworkCore.Tools
```

Select the **Persistence project** as the Package Manager Console's Default project when installing or running migrations for that project.

Your `DbContext` registration should read the connection string configured in the API project. For example, if the Persistence extension method accepts `IConfiguration`:

```csharp
services.AddDbContext<GloboTicketDbContext>(options =>
    options.UseSqlite(
        configuration.GetConnectionString("GloboTicketDatabase")));
```

This snippet belongs in your Persistence service-registration code, not necessarily in the API's `StartupExtensions.cs`. Replace `GloboTicketDbContext` with your actual context name.

## 9. Create and Apply a Migration

### 9.1 Set the startup project

In Visual Studio, right-click the API project and select **Set as Startup Project**. The API provides application configuration for the context when the EF tooling needs to create it.

### 9.2 Select the default project

Open **Tools → NuGet Package Manager → Package Manager Console** and set the Default project to:

`GloboTicket.TicketManagement.Persistence`

### 9.3 Create the migration

```powershell
Add-Migration InitialMigration -Context GloboTicketDbContext -OutputDir Migrations
```

Replace `GloboTicketDbContext` with the actual `DbContext` class name. If the solution contains only one context, the `-Context` option may be omitted.

### 9.4 Apply the migration

```powershell
Update-Database -Context GloboTicketDbContext
```

Creating a migration and applying it are separate actions:

- `Add-Migration` generates migration code.
- `Update-Database` applies pending migrations to the database.

Review generated migrations before applying them, especially in production. Production database changes should follow your deployment and backup process.

## 10. Add a Controller Using MediatR

MediatR lets a controller send a request without depending directly on a concrete handler class.

Create `Controllers/CategoryController.cs`:

```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
// Add the namespace containing GetCategoriesListQuery
// and CategoryListVm in your solution.

namespace GloboTicket.TicketManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ISender _sender;

    public CategoryController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("all", Name = "GetAllCategories")]
    [ProducesResponseType(
        typeof(List<CategoryListVm>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryListVm>>>
        GetAllCategories(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(
            new GetCategoriesListQuery(),
            cancellationToken);

        return Ok(result);
    }
}
```

This assumes your Application project defines `GetCategoriesListQuery` and `CategoryListVm`, and that the query returns a compatible list. Add the correct `using` directives for their actual namespaces.

`ISender` is a focused MediatR abstraction for sending requests. You may use `IMediator` instead if that matches your project's existing convention.

### How the endpoint works

1. A client sends `GET /api/category/all`.
2. ASP.NET Core routes the request to `GetAllCategories()`.
3. The controller sends `GetCategoriesListQuery` through MediatR.
4. MediatR dispatches it to the registered query handler.
5. The handler retrieves and maps the data.
6. The controller returns `200 OK` with the result.

### Example: create a resource

A POST action can accept an Application command:

```csharp
[HttpPost]
[ProducesResponseType(
    typeof(CreateCategoryDto),
    StatusCodes.Status201Created)]
[ProducesResponseType(
    typeof(ValidationProblemDetails),
    StatusCodes.Status400BadRequest)]
public async Task<ActionResult<CreateCategoryDto>> Create(
    [FromBody] CreateCategoryCommand command,
    CancellationToken cancellationToken)
{
    var result = await _sender.Send(command, cancellationToken);

    // Use CreatedAtAction when you have a suitable GET endpoint.
    // Otherwise, return an appropriate response for your API contract.
    return Ok(result);
}
```

This is illustrative: if you document `201 Created`, the implementation should actually return that status, for example via `CreatedAtAction(...)` when appropriate. Do not leave the documented status inconsistent with the actual response.

## 11. Choose Response Models and HTTP Status Codes

Return purpose-built DTOs/ViewModels rather than exposing EF Core entities directly.

Common examples:

- `EventListVm`: only the fields needed for an event list.
- `EventDetailVm`: fields needed for the detail page.
- `CreateCategoryDto`: information returned after category creation.

Common status codes:

| Status | Typical use |
|---|---|
| `200 OK` | Successful read or operation returning a body |
| `201 Created` | A new resource was created |
| `204 No Content` | Successful operation with no response body |
| `400 Bad Request` | Invalid input or validation failure |
| `404 Not Found` | Requested resource does not exist |
| `500 Internal Server Error` | Unexpected server failure |

### ProblemDetails

ASP.NET Core supports a standard error response format, `ProblemDetails`, and a validation-specific format, `ValidationProblemDetails`.

A validation response may look like this:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": [
      "The Name field is required."
    ]
  }
}
```

The exact response depends on your validation and exception-handling configuration. Response-type attributes document the API contract; they do not create error responses by themselves.

Implement centralized exception handling to translate application exceptions into consistent HTTP responses. Never expose raw stack traces, secrets, or internal database details to clients.

## 12. Configure OpenAPI and Swagger UI

OpenAPI describes the API contract. Swagger UI is a browser interface for viewing and testing that contract.

### 12.1 Install packages

For the built-in OpenAPI document plus Swagger UI approach:

```powershell
Install-Package Microsoft.AspNetCore.OpenApi
Install-Package Swashbuckle.AspNetCore.SwaggerUI
```

Package availability and compatibility vary by target framework. If your template already added OpenAPI support, do not install duplicate or incompatible packages.

### 12.2 Register OpenAPI

In `ConfigureServices()`:

```csharp
builder.Services.AddOpenApi();
```

### 12.3 Map the document and Swagger UI

In `ConfigurePipeline()`:

```csharp
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/v1.json");

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "GloboTicket API v1");

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.MapControllers();
```

This configuration exposes the OpenAPI document and UI only in Development. Keep documentation exposure intentional in production.

### 12.4 Open the endpoints

Start the API and use the actual port shown in your launch settings:

- Swagger UI: `https://localhost:<port>/swagger`
- OpenAPI JSON: `https://localhost:<port>/openapi/v1.json`
- Category endpoint: `https://localhost:<port>/api/category/all`

In Swagger UI:

1. Find `GET /api/Category/all`.
2. Select **Try it out**.
3. Select **Execute**.
4. Check the HTTP status and JSON response.

If your project uses the traditional Swashbuckle setup instead, it may use `AddSwaggerGen()`, `UseSwagger()`, and `UseSwaggerUI()` with `/swagger/v1/swagger.json`. Do not mix the two approaches without configuring the corresponding services and document URL.

## 13. Add SonarQube Code Analysis

SonarQube analyzes code quality and can report bugs, code smells, vulnerabilities, and security hotspots. This is separate from Swagger.

### 13.1 Start a local SonarQube server (optional)

If Docker is installed, a basic local test setup can start a SonarQube Community image:

```powershell
docker run -d --name sonarqube `
  -p 9000:9000 `
  sonarqube:community
```

Then open `http://localhost:9000` and follow the initial setup instructions. Check the current SonarQube documentation for supported Docker images, system requirements, and production configuration. A simple container command is for local evaluation, not a production deployment recipe.

Official documentation: https://docs.sonarsource.com/sonarqube-server/

### 13.2 Create a SonarQube project

1. Create a project in the SonarQube dashboard.
2. Set a project key, for example `GloboTicket-TicketManagement`.
3. Generate an analysis token.
4. Store the token securely. Do not commit it to source control.

### 13.3 Install SonarScanner for .NET

```powershell
dotnet tool install --global dotnet-sonarscanner
```

If already installed:

```powershell
dotnet tool update --global dotnet-sonarscanner
```

Official guide: https://docs.sonarsource.com/sonarqube-server/analyzing-source-code/scanners/dotnet/introduction/

### 13.4 Run analysis

Open a suitable .NET developer terminal at the solution root, where the `.sln` file is located. Replace the project key and token with your own values.

```powershell
dotnet sonarscanner begin `
  /k:"GloboTicket-TicketManagement" `
  /d:sonar.host.url="http://localhost:9000" `
  /d:sonar.token="YOUR_SONAR_TOKEN"

dotnet build GloboTicket.sln

dotnet sonarscanner end `
  /d:sonar.token="YOUR_SONAR_TOKEN"
```

Replace `GloboTicket.sln` with your actual solution filename.

Run `begin`, build, and `end` sequentially in the same terminal session. The build must succeed for a meaningful analysis. Use a secure secret/environment variable or your CI/CD secret store for tokens rather than saving them in source-controlled scripts.

### 13.5 Review results

Open the project dashboard and review:

- Bugs
- Vulnerabilities
- Code Smells
- Security Hotspots
- Maintainability and Reliability
- Quality Gate status

Analyze from the solution root if you want SonarScanner to include all relevant projects. Follow your team's rules for handling findings rather than treating every finding as automatically valid or invalid.

## 14. Test the API

### Browser

Open:

`https://localhost:<port>/api/category/all`

A successful request should return JSON with HTTP `200 OK`, assuming the database and handler are configured and contain data.

### Postman

1. Create a request with method `GET`.
2. Enter `https://localhost:<port>/api/category/all`.
3. Click **Send**.
4. Inspect the status code and response body.

### `curl`

```powershell
curl.exe -k https://localhost:<port>/api/category/all
```

`-k` bypasses certificate validation and should be used only for local development with a development certificate issue. Prefer trusting/configuring the development certificate instead. Do not use this option as a production security workaround.

## 15. Troubleshooting

| Problem | Things to check |
|---|---|
| `404 Not Found` | Verify `[Route("api/[controller]")]`, `[HttpGet("all")]`, and `app.MapControllers()` |
| `500 Internal Server Error` | Check application logs, DI registrations, handler dependencies, and database configuration |
| `Unable to resolve service for type ...` | Ensure the relevant service and MediatR handler assembly are registered |
| Database or table missing | Confirm the connection string, startup project, migration project, and `Update-Database` result |
| Migration cannot create the context | Verify design-time configuration, provider packages, startup project, and actual `DbContext` name |
| Swagger UI shows no endpoints | Check controller discovery, route attributes, OpenAPI configuration, and response metadata |
| Swagger UI cannot load the document | Open `/openapi/v1.json` directly and verify the URL matches `SwaggerEndpoint` |
| SonarScanner cannot connect | Check that the SonarQube server is running and the host URL is reachable |
| Sonar analysis fails | Check scanner output, project key, token, build success, and supported .NET/Scanner versions |

## 16. Final Checklist

- [ ] API project created with controller support.
- [ ] Application, Infrastructure, and Persistence references added.
- [ ] Connection string configured and its key matches Persistence.
- [ ] Dependency injection registrations are correct.
- [ ] EF Core provider/tooling packages are compatible.
- [ ] Migration created and applied to the intended database.
- [ ] Category endpoint returns the expected response.
- [ ] DTO/ViewModel response types are used instead of returning EF entities.
- [ ] HTTP status codes and response metadata match actual behavior.
- [ ] ProblemDetails and validation error handling are configured and tested.
- [ ] OpenAPI JSON and Swagger UI work in Development.
- [ ] SonarScanner analysis completes and the dashboard has a result.
- [ ] Secrets and production configuration are handled securely.

---

## Recommended implementation order

1. Build and run the API with `MapControllers()`.
2. Confirm the `GET /api/category/all` endpoint works.
3. Confirm EF Core migrations and the database are correct.
4. Add OpenAPI and Swagger UI.
5. Add centralized exception handling and validation responses.
6. Add SonarQube analysis locally.
7. Automate build, tests, and code analysis in CI/CD.

**Important:** Treat code samples in this guide as templates. Match the namespaces, project names, context name, extension-method signatures, package versions, and route conventions to your actual solution.
