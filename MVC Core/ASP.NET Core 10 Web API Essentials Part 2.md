# Complete Study Notes

> A comprehensive reference covering Logging, Security (JWT), API Versioning, OpenAPI Documentation, and Testing Tools based on hands-on demos.

---
## Table of Contents

1. [Logging](#1-logging)
2. [Security – JWT Authentication & Authorization](#2-security--jwt-authentication--authorization)
3. [API Versioning](#3-api-versioning)
4. [OpenAPI & Documentation](#4-openapi--documentation)
5. [Testing Tools](#5-testing-tools)

---

## 1. Logging

### 1.1 Basics

ASP.NET Core includes built-in logging providers out of the box:
- **Console** – logs to the terminal
- **Debug** – logs to the debug output window

#### Configuration (`appsettings.{Environment}.json`)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "CityInfo.API.Controllers": "Information"
    }
  }
}
```

- `Default` sets the baseline log level.
- Category overrides allow fine-grained control per namespace.

#### Using `ILogger<T>`

```csharp
public class CitiesController : ControllerBase
{
    private readonly ILogger<CitiesController> _logger;

    public CitiesController(ILogger<CitiesController> logger)
    {
        _logger = logger;
    }
}
```

- Inject via constructor (preferred pattern).
- The category is automatically set to the fully qualified type name.

#### Structured Logging

```csharp
_logger.LogInformation("Getting points of interest for city {cityId}", cityId);
```

- Use **placeholders** (not string interpolation) for structured log output.

---

### 1.2 Global Exception Handling

| Environment | Behavior |
|---|---|
| Development | Detailed error pages, stack traces shown |
| Production | Generic 500 returned; exception still logged server-side |

#### Exception Handler Middleware

```csharp
app.UseExceptionHandler();
```

#### ProblemDetails (RFC 7807)

```csharp
builder.Services.AddProblemDetails();
```

- **Production**: returns `type`, `title`, `status`, `traceId`
- **Development**: includes full exception details and stack trace

---

### 1.3 File Logging with Serilog

#### Packages Required

- `Serilog.AspNetCore`
- `Serilog.Sinks.Console`
- `Serilog.Sinks.File`

#### Configuration (before `CreateBuilder`)

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

#### Host Integration

```csharp
builder.Host.UseSerilog();
```

**Benefits:**
- No changes needed to existing `ILogger<T>` usage.
- Logs automatically go to both console and rolling file.

---

## 2. Security – JWT Authentication & Authorization

### 2.1 Creating a JWT Token

#### Authentication Controller

```csharp
[HttpPost("authenticate")]
public ActionResult<string> Authenticate(AuthenticationRequestDto request)
{
    // 1. Validate credentials
    // 2. Build claims
    // 3. Create and return token
}
```

#### DTO

```csharp
public class AuthenticationRequestDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}
```

#### User Model (Demo)

```csharp
public class CityInfoUser
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string City { get; set; }
}
```

#### App Settings

```json
{
  "Authentication": {
    "SecretForKey": "your-base64-secret",
    "Issuer": "https://localhost:7169",
    "Audience": "cityinfoapi"
  }
}
```

#### Token Generation

```csharp
var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(secret));
var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

var claims = new List<Claim>
{
    new Claim("sub", user.UserId.ToString()),
    new Claim("given_name", user.FirstName),
    new Claim("family_name", user.LastName),
    new Claim("city", user.City)
};

var token = new JwtSecurityToken(
    issuer, audience, claims,
    DateTime.UtcNow, DateTime.UtcNow.AddHours(1),
    signingCredentials);

return new JwtSecurityTokenHandler().WriteToken(token);
```

> ⚠️ JWT is **encoded**, not **encrypted**. Always use **HTTPS**.

---

### 2.2 Requiring & Validating a Token

#### Package

```
Microsoft.AspNetCore.Authentication.JwtBearer
```

#### Configuration (`Program.cs`)

```csharp
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Authentication:Issuer"],
            ValidAudience = config["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String(config["Authentication:SecretForKey"]))
        };
    });
```

#### Pipeline (order matters)

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

#### Protect Endpoints

```csharp
[Authorize]
public class CitiesController : ControllerBase { }
```

- Leave `AuthenticationController` open (no `[Authorize]`).
- No token → **401 Unauthorized**
- Valid token → access granted

---

### 2.3 `dotnet user-jwts` (Dev Tokens)

#### Create Tokens

```bash
# Default token
dotnet user-jwts create

# With specific issuer, audience and custom claim
dotnet user-jwts create --issuer https://localhost:7169 --audience cityinfoapi --claim city=Antwerp
```

#### Default Behavior

- Audience is taken from `launchSettings.json` HTTPS URLs.
- Issuer defaults to `dotnet user-jwts`.
- Won't validate against custom settings → **401** until aligned.

#### Get Signing Key

```bash
dotnet user-jwts key --issuer https://localhost:7169
```

Copy the key into `appsettings.Development.json` as `SecretForKey`.

> **Note:** Running `user-jwts` commands auto-adds a `Schemes:Bearer` section to `appsettings.Development.json`.

#### Token Management

```bash
dotnet user-jwts list           # list all tokens for this project
dotnet user-jwts print <id>     # print a specific token
dotnet user-jwts remove <id>    # remove a specific token
dotnet user-jwts clear          # remove all tokens
```

#### Security Note

> You **cannot** forge tokens from a real identity provider using `user-jwts` because you don't have the private signing key. It is safe to use for local development only.

---

### 2.4 Authorization Policies (Claims-Based)

#### Define a Policy (`Program.cs`)

```csharp
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("MustBeFromAntwerp", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Antwerp");
    });
```

#### Apply the Policy

```csharp
[Authorize(Policy = "MustBeFromAntwerp")]
public class PointsOfInterestController : ControllerBase { }
```

#### Outcomes

| Token `city` claim | Result |
|---|---|
| `Brussels` | **403 Forbidden** (authenticated but not authorized) |
| `Antwerp` | ✅ Access granted |

> **Authentication** = "Who are you?" | **Authorization** = "Are you allowed?"

---

## 3. API Versioning

### 3.1 Setup

#### Package

```
Asp.Versioning.Mvc
```

#### Configuration (`Program.cs`)

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
}).AddMvc();
```

- `ReportApiVersions = true` → response headers include supported versions.
- `AssumeDefaultVersionWhenUnspecified = true` → unversioned requests use v1.

---

### 3.2 Versioning Resources

#### Attribute-Based Versioning

```csharp
[ApiVersion(1)]
[ApiVersion(2)]             // support multiple versions
public class CitiesController : ControllerBase { }

[ApiVersion(2)]
public class PointsOfInterestController : ControllerBase { }
```

#### Action-Level with Deprecation

```csharp
[ApiVersion("0.1", Deprecated = true)]
public IActionResult GetFile(int fileId) { }
```

#### Query String Versioning

```
GET /api/cities?api-version=2
```

---

### 3.3 URI-Based Versioning (Recommended)

Update route templates to embed the version:

```csharp
[Route("api/v{apiVersion:apiVersion}/cities")]
public class CitiesController : ControllerBase { }

[Route("api/v{apiVersion:apiVersion}/cities/{cityId}/pointsofinterest")]
public class PointsOfInterestController : ControllerBase { }
```

**Example URLs:**
```
GET /api/v1/cities
GET /api/v2/cities/{cityId}/pointsofinterest
```

> ✅ URI-based versioning is clear, explicit, and easy for API consumers to use.

---

## 4. OpenAPI & Documentation

### 4.1 Why Document Your API?

- Reduces repeated support questions.
- Prevents teams from building duplicate APIs.
- Provides a machine-readable spec for code generation and tooling.

### 4.2 Key Terms

| Term | Meaning |
|---|---|
| **OpenAPI Specification** | Standard JSON/YAML format describing your API |
| **Swagger Specification** | Same as OpenAPI (historical name) |
| **Swagger (tools)** | Ecosystem of tools: UI, Editor, Code Generators |
| **Scalar** | Modern OpenAPI UI recommended by Microsoft for .NET |

---

### 4.3 Microsoft Built-In OpenAPI Support (.NET 9+)

#### Package
```
Microsoft.AspNetCore.OpenApi
```

#### Setup (`Program.cs`)

```csharp
// Register services
builder.Services.AddOpenApi();

// Map endpoint
app.MapOpenApi();
```

#### Versioned OpenAPI Docs

Install additional package:
```
Asp.Versioning.Mvc.ApiExplorer
```

Update versioning setup:
```csharp
builder.Services.AddApiVersioning(...)
    .AddMvc()
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });
```

Register separate docs per version:
```csharp
builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v2");
```

Map with dynamic document name:
```csharp
app.MapOpenApi("/openapi/{documentName}.json");
```

---

### 4.4 Scalar UI

#### Package
```
Scalar.AspNetCore
```

#### Setup (`Program.cs`)

```csharp
app.MapScalarApiReference(options =>
{
    options.WithTitle("City Info API")
           .WithTheme(ScalarTheme.Solarized)
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
           .AddPreferredSecuritySchemes("Bearer");
});
```

#### Access URLs
```
/scalar/v1    → interactive docs for API v1
/scalar/v2    → interactive docs for API v2
```

---

### 4.5 XML Comments in Docs

#### Step 1: Add XML comments to actions and DTOs

```csharp
/// <summary>
/// Get a city by id.
/// </summary>
/// <param name="id">The id of the city to get.</param>
/// <returns>A city without points of interest.</returns>
[HttpGet("{id}")]
public ActionResult<CityWithoutPointsOfInterestDto> GetCity(int id) { }
```

#### Step 2: Enable XML doc generation

Project Properties → Build → Output → ✅ **Generate a file containing API documentation**

> ✅ Microsoft's built-in OpenAPI support automatically picks up the XML file — no extra configuration needed.

#### Important Gotcha

The XML comment source generator **requires literal strings** as document names:

```csharp
// ✅ Works
builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v2");

// ❌ Does NOT work for XML comments
foreach (var version in apiVersions)
    builder.Services.AddOpenApi(version);
```

---

### 4.6 Documenting Response Types

```csharp
[HttpGet("{id}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public ActionResult<CityWithoutPointsOfInterestDto> GetCity(int id) { }
```

> API consumers need to know all possible responses so their code can handle each case correctly.

---

### 4.7 Customizing the OpenAPI Document

```csharp
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, _) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "City Info API",
            Version = context.DocumentName,
            Description = "An API for accessing city and points of interest data."
        };
        return Task.CompletedTask;
    });
});
```

---

### 4.8 Adding Authentication to OpenAPI Docs

```csharp
options.AddDocumentTransformer((document, context, _) =>
{
    document.Components ??= new OpenApiComponents();
    document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",        // lowercase is important
        BearerFormat = "JWT",
        Description = "Enter your JWT Bearer token."
    };

    document.SecurityRequirements.Add(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()   // no scopes for Bearer
        }
    });

    return Task.CompletedTask;
});
```

**Result in Scalar:**
- Authentication section appears in the UI.
- Paste a token once → Scalar automatically sends `Authorization: Bearer <token>` on all requests.

---

## 5. Testing Tools

### 5.1 HTTP REPL

#### Install

```bash
dotnet tool install -g microsoft.dotnet-httprepl
```

#### OpenAPI Compatibility Fix

Force OpenAPI 3.0 (REPL can't parse 3.1 at time of demo):

```csharp
builder.Services.AddOpenApi("v1", options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
});
```

#### Start REPL

```bash
httprepl https://localhost:7164 --openapi https://localhost:7164/openapi/v2.json
```

#### Navigation Commands

```bash
ls                  # list endpoints at current path
cd api              # navigate into a path segment
cd v2/cities        # navigate multiple levels
get                 # send GET to current resource
post                # send POST (opens editor for body)
help get            # show options for a command
```

#### Headers

```bash
# Per-request header
get -h "Accept:application/xml"

# Global header (persists for session)
set header Authorization "Bearer <your-token>"
```

#### POST Workflow

```bash
# Set default editor first
pref set editor.command.default notepad

# Send POST (editor opens with sample body from OpenAPI spec)
post -h "Content-Type:application/json"
```

#### Common Gotcha

Inconsistent route parameter names (`id` vs `cityId`) across controllers confuse REPL navigation.

✅ **Fix:** Use consistent parameter naming across all controllers.

---

### 5.2 Endpoints Explorer (Visual Studio)

| Feature | Details |
|---|---|
| **Location** | View → Endpoints Explorer (tool window) |
| **What it shows** | Detected endpoints, parameters, controller links |
| **Best feature** | Generate starter `.http` requests (right-click → Generate Request) |
| **Limitation** | May show non-existent endpoints (e.g., false DELETE actions) |

> **Tip:** GitHub Copilot can also generate `.http` files from your existing endpoints, making Endpoints Explorer less essential over time.

---

## Quick Reference: Key Packages

| Package | Purpose |
|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT Bearer token validation |
| `System.IdentityModel.Tokens.Jwt` | JWT token creation |
| `Serilog.AspNetCore` | Serilog integration for ASP.NET Core |
| `Serilog.Sinks.File` | File sink for Serilog |
| `Asp.Versioning.Mvc` | API versioning for MVC |
| `Asp.Versioning.Mvc.ApiExplorer` | Versioned API Explorer metadata |
| `Microsoft.AspNetCore.OpenApi` | Built-in OpenAPI spec generation (.NET 9+) |
| `Scalar.AspNetCore` | Interactive OpenAPI UI |

---

## Quick Reference: Key Concepts

```
JWT Flow:
  Client → POST /authenticate → API returns JWT
  Client → GET /protected-resource + Authorization: Bearer <token> → API validates → response

Versioning Strategy:
  URI-based: /api/v{version}/resource  ← recommended
  Query string: /api/resource?api-version=2

OpenAPI Pipeline:
  AddOpenApi("v1") → generates spec
  MapOpenApi("/openapi/{documentName}.json") → exposes spec endpoint
  MapScalarApiReference() → renders interactive UI

Authentication in Docs:
  SecurityScheme (Bearer/JWT) → SecurityRequirement (global) → Scalar shows auth UI
```

---

*Notes generated from CityInfo.API demo walkthroughs covering ASP.NET Core Web API development.*