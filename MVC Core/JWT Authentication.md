
## Access Token & Refresh Token

![[Pasted image 20260322110330.png]]
![[Pasted image 20260322110346.png]]

In this example, we’ll use the following files:

- LoginModel (This model is used to store user credentials to login to the application)    
- RegisterModel (This model stores user data required to register a new user)  
- TokenModel (This model contains the access and refresh token and is used to send these tokens in the response)  
      
    
- ApplicationUser (This class extends the functionality of the IdentityUser class of the ASP.NET Core Identity Framework)  
- ApplicationDbContext (This represents the DbContext used to interact with the underlying database)  
      
    
- MessageCode (This record type contains a list of message codes.)  
- MessageProvider (This record type contains a list of notification and error messages.)  

- JwtOptions (This type is used to read configuration data.)  
- Response (This represents the custom response format we’ll use for sending formatted response out of the controller action methods.)  
      
    
- IAuthenticationService  
- AuthenticationService (This class represents the Authentication Service that wraps all logic for registering a new user, logging in an existing user, refreshing tokens, etc.)  
      

- AuthenticationController (This represents the API that contains action methods to register a new user, login an existing user, refresh tokens, etc. It calls the methods of the AuthenticationService class to perform each of these operations.) 


## Install the Nuget Packages

```cs
Microsoft.AspNetCore.Authentication.JwtBearer
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.Design
```

## Create the Models

```cs
public record ![[AuthenticationService.cs]]
{
   public string Username { get; set; }
   public string Password { get; set; }
}

public record RegisterModel
{
   public string Username { get; set; }
   public string Email { get; set; }
   public string Password { get; set; }
}

public record TokenModel
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
} 

public class ApplicationUser : IdentityUser
{

    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

}
```

## Message Provider
```cs 
public enum MessageCode
{
    LoginSuccess,
    InvalidCredentials,
    UserAlreadyExists,
    UserCreationFailed,
    UserCreatedSuccessfully,
    InvalidRequest,
    InvalidTokenPair,
    AccessTokenSuccess,
    RefreshTokenSuccess,
    UnexpectedError
} 


public record MessageProvider
 {
     public static string GetMessage(MessageCode code)
     {
       switch (code)
       {
          case MessageCode.LoginSuccess:
              return "User logged in successfully.";
          case MessageCode.InvalidCredentials:
              return "Invalid credentials.";
          case MessageCode.UserAlreadyExists:
              return "User already exists.";
          case MessageCode.UserCreationFailed:
              return "User creation failed.";
          case MessageCode.UserCreatedSuccessfully:
              return "User created successfully.";
          case MessageCode.InvalidRequest:
              return "Invalid request.";
          case MessageCode.InvalidTokenPair:
              return "Invalid access token or refresh token.";
          case MessageCode.RefreshTokenSuccess:
              return "Token refreshed successfully.";
          case MessageCode.UnexpectedError:
              return "An unexpected error occurred.";
          default:
              throw new ArgumentOutOfRangeException
              ("Invalid message code.");
         }
     }
 }

```

### Create the response type
```cs 

public record Response<T>
{
    public string? Message { get; set; }
    public T? Data { get; set; }
    public HttpStatusCode StatusCode { get; set; }
 
    public static Response<T> Create(
        HttpStatusCode statusCode,
        T? data = default,
        MessageCode? messageCode = null)
    {
        return new Response<T>
        {
            StatusCode = statusCode,
            Data = data,
            Message = messageCode.HasValue
                ?  
            MessageProvider.GetMessage(messageCode.Value)
                : null
        };
    }
}
```

 ##  Create the JWT section in the configuration file

```cs
"JWT": {
  "ValidAudience": "http://localhost:4200",
  "ValidIssuer": "http://localhost:5000",
  "SecretKey": "Specify your custom secret key here",
  "AccessTokenValidityInMinutes": 1,
  "RefreshTokenValidityInMinutes": 60
}

public sealed record JwtOptions
{
  public string SecretKey { get; init; } = string.Empty;
  public string ValidIssuer { get; init; } = string.Empty;
  public string ValidAudience { get; init; } = string.Empty;
  public int AccessTokenValidityInMinutes { get; init; } = 0;
  public int RefreshTokenValidityInMinutes { get; init; } = 0;
}
```

### Create the data context 
```cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
   public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
   { }

   protected override void OnModelCreating(ModelBuilder builder)
   {
       base.OnModelCreating(builder);
   }
}
```

###  Create the authentication service 

```cs
 public interface IAuthenticationService
    {
        Task<Response<object>> 
            LoginAsync(LoginRequest request, 
            CancellationToken cancellationToken = default);
        Task<Response<object>> 
            RegisterAsync(RegisterRequest request, 
            CancellationToken cancellationToken = default);
        Task<Response<object>> 
        RefreshTokensAsync(RefreshTokenRequest request, 
            CancellationToken cancellationToken = default);
    } 


public sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtOptions _jwtOptions;
 
    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value ?? 
        throw new ArgumentNullException(nameof(jwtOptions));
        _userManager = userManager ?? 
        throw new ArgumentNullException(nameof(userManager));
 
        if (string.IsNullOrWhiteSpace(_jwtOptions.SecretKey))
        {
            throw new InvalidOperationException
            ("The Secret Key is not configured.");
        }
    }
}
```

## Add Authentication Service
```cs
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public AuthenticationService(UserManager<ApplicationUser> userManager,IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

            if (string.IsNullOrWhiteSpace(_jwtOptions.SecretKey))
            {
                throw new InvalidOperationException("The Secret Key is not configured.");
            }
        }

        public async Task<Response<object>> LoginAsync(RefreshTokenDemo.Models.LoginRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Response<object>.Create(
                  
                    null, HttpStatusCode.BadRequest,
                    MessageCode.InvalidCredentials);
            }

            var tokens = await GenerateTokensAsync(user, cancellationToken);

            return Response<object>.Create(
            
                new { tokens.AccessToken, tokens.RefreshToken }, HttpStatusCode.OK,
                MessageCode.LoginSuccess);
        }

       

     

        public async Task<Response<object>> RegisterAsync(RegisterModel request, CancellationToken cancellationToken = default)
        {
            var existingUser = await _userManager.FindByNameAsync(request.Username);
            if (existingUser != null)
            {
                return Response<object>.Create(
                 
                    null, HttpStatusCode.BadRequest,
                    MessageCode.UserAlreadyExists);
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = request.Username
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return Response<object>.Create(null,
                    HttpStatusCode.BadRequest,
               
                    MessageCode.UserCreationFailed);
            }

            return Response<object>.Create(null,
                HttpStatusCode.OK,

                MessageCode.UserCreatedSuccessfully);
        }

        public async Task<Response<object>> RefreshTokensAsync(RefreshTokenRequest request,
            CancellationToken cancellationToken = default)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken ?? string.Empty);
            var username = principal.Identity?.Name;

            var user = await _userManager.Users
                .FirstOrDefaultAsync(
                    u => u.UserName == username && u.RefreshToken == request.RefreshToken,
                    cancellationToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Response<object>.Create(   null,
                    HttpStatusCode.BadRequest,
                    MessageCode.InvalidTokenPair);
            }

            var tokens = await GenerateTokensAsync(user, cancellationToken);

            return Response<object>.Create(
                new { tokens.AccessToken, tokens.RefreshToken },
                HttpStatusCode.OK,
                MessageCode.RefreshTokenSuccess);
        }

        private async Task<TokenModel> GenerateTokensAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            // Build claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            // Add role claims if any
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Signing credentials
            var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Token lifetime
            var accessTokenValidity = TimeSpan.FromMinutes(Math.Max(1, _jwtOptions.AccessTokenValidityInMinutes));
            var refreshTokenValidity = TimeSpan.FromMinutes(Math.Max(1, _jwtOptions.RefreshTokenValidityInMinutes));

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: string.IsNullOrWhiteSpace(_jwtOptions.ValidIssuer) ? null : _jwtOptions.ValidIssuer,
                audience: string.IsNullOrWhiteSpace(_jwtOptions.ValidAudience) ? null : _jwtOptions.ValidAudience,
                claims: claims,
                notBefore: now,
                expires: now.Add(accessTokenValidity),
                signingCredentials: signingCredentials
            );

            var handler = new JwtSecurityTokenHandler();
            var accessToken = handler.WriteToken(jwt);

            // Generate secure refresh token
            var refreshTokenBytes = new byte[64];
            RandomNumberGenerator.Fill(refreshTokenBytes);
            var refreshToken = Convert.ToBase64String(refreshTokenBytes);

            // Persist refresh token and expiry on the user
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.Add(refreshTokenValidity);
            // UserManager.UpdateAsync does not accept a CancellationToken
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                // If update failed, throw or handle accordingly. Here we throw to let caller surface an error.
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Unable to persist refresh token for user {user.Id}. Errors: {errors}");
            }

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Token must be provided", nameof(token));
            }

            var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

                // Validate issuer/audience only when configured
                ValidateIssuer = !string.IsNullOrWhiteSpace(_jwtOptions.ValidIssuer),
                ValidIssuer = string.IsNullOrWhiteSpace(_jwtOptions.ValidIssuer) ? null : _jwtOptions.ValidIssuer,

                ValidateAudience = !string.IsNullOrWhiteSpace(_jwtOptions.ValidAudience),
                ValidAudience = string.IsNullOrWhiteSpace(_jwtOptions.ValidAudience) ? null : _jwtOptions.ValidAudience,

                // We want to get claims from an expired token as well
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                return principal;
            }
            catch (Exception ex) when (ex is SecurityTokenException || ex is ArgumentException || ex is FormatException)
            {
                // Re-throw a SecurityTokenException for callers to handle
                throw new SecurityTokenException("Invalid token", ex);
            }
        }
    }
```

## ## How to create migrations using Entity Framework (EF) Core 
```cs
Add-Migration RefreshTokenDemoMigration
Update-Databas

```

## Create the authentication controller

```cs
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authService)
        {
            _authenticationService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var response = Response<object>.Create(null,
                    System.Net.HttpStatusCode.BadRequest,
              
                    MessageCode.InvalidCredentials);

                return BadRequest(response);
            }

            var responseFromService = await _authenticationService.LoginAsync(request);

            if (responseFromService != null)
            {
                if (responseFromService.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest(responseFromService);
                }
            }

            return Ok(responseFromService);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel   request)
        {
            if (!ModelState.IsValid)
            {
                var response = Response<object>.Create(null,
                    System.Net.HttpStatusCode.BadRequest,
                 
                    MessageCode.UserCreationFailed);

                return BadRequest(response);
            }

            var responseFromService = await _authenticationService.RegisterAsync(request);

            if (responseFromService != null)
            {
                if (responseFromService.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest(responseFromService);
                }
            }

            return Ok(responseFromService);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var responseFromService = await _authenticationService.RefreshTokensAsync(request);

            if (responseFromService != null)
            {
                if (responseFromService.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return BadRequest(responseFromService);
                }
            }

            return Ok(responseFromService);
        }
    }
```

###  What is the Program.cs file?

```cs
   public static void Main(string[] args)
   {
       var builder = WebApplication.CreateBuilder(args);
       builder.Services.AddDbContext<ApplicationDbContext>(options =>
       {
           options.UseSqlServer(
               builder.Configuration.GetConnectionString("DefaultConnection"));
       });

       builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
       .AddEntityFrameworkStores<ApplicationDbContext>()
       .AddDefaultTokenProviders(); //This is required to provide user management capabilities in your application.

       // Add services to the container.

       builder.Services.AddControllers();
       // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
       builder.Services.AddOpenApi();

       builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
       builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));


       var jwtOptions = builder.Configuration.GetSection("JWT").Get<JwtOptions>() ?? new JwtOptions();

       // Add Authentication with JWT Bearer
       var keyBytes = Encoding.UTF8.GetBytes(jwtOptions.SecretKey ?? string.Empty);

       builder.Services.AddAuthentication(options =>
       {
           options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
           options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
       })
       .AddJwtBearer(options =>
       {
           options.RequireHttpsMetadata = true;
           options.SaveToken = true;

           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
               ValidateIssuer = !string.IsNullOrWhiteSpace(jwtOptions.ValidIssuer),
               ValidIssuer = string.IsNullOrWhiteSpace(jwtOptions.ValidIssuer) ? null : jwtOptions.ValidIssuer,
               ValidateAudience = !string.IsNullOrWhiteSpace(jwtOptions.ValidAudience),
               ValidAudience = string.IsNullOrWhiteSpace(jwtOptions.ValidAudience) ? null : jwtOptions.ValidAudience,
               ValidateLifetime = true
           };
       });

       // Swagger / OpenAPI + JWT security definition
         




       var app = builder.Build();

       // Configure the HTTP request pipeline.
       if (app.Environment.IsDevelopment())
       {
           app.MapOpenApi();
           // Available at https://localhost:{port}/swagger
           //app.UseSwagger();
           //app.UseSwaggerUI(options =>
           //{
           //    options.SwaggerEndpoint("/swagger/v1/swagger.json", "RefreshTokenDemo v1");
           //    options.RoutePrefix = "swagger"; // available at /swagger
           //});
           app.UseSwagger();
           app.UseSwaggerUI(options =>
           {
               options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1");
           });

       }

       app.UseHttpsRedirection();

       app.UseAuthentication();
       app.UseAuthorization();


       app.MapControllers();

       app.Run();
   }
```


## Bearer Token Using Scalar

```cs
 internal sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
 {
     private readonly IAuthenticationSchemeProvider _schemeProvider;

     public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider schemeProvider)
     {
         _schemeProvider = schemeProvider ?? throw new ArgumentNullException(nameof(schemeProvider));
     }
     public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
     {
         var schemes = await _schemeProvider.GetAllSchemesAsync();

         if (!schemes.Any(s => s.Name == "Bearer"))
             return;

         document.Components ??= new OpenApiComponents();

         if (document.Components.SecuritySchemes == null)
             document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>();

         var schemeId = "Bearer";

         document.Components.SecuritySchemes[schemeId] = new OpenApiSecurityScheme
         {
             Type = SecuritySchemeType.Http,
             Scheme = "bearer",
             BearerFormat = "JWT",
             In = ParameterLocation.Header,
             Description = "JWT Authorization"
         };

         document.Security ??= new List<OpenApiSecurityRequirement>();

         document.Security.Add(new OpenApiSecurityRequirement
         {
             [new OpenApiSecuritySchemeReference(schemeId)] = new List<string>()
         });
     }
 } 
 
 
    builder.Services.AddOpenApi("v1",options=> { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });
 
 
     app.MapScalarApiReference(options => options
     .AddPreferredSecuritySchemes("BearerAuth")
     .AddHttpAuthentication("BearerAuth", auth =>
     {
         auth.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
     }).EnablePersistentAuthentication()
     );
 
 
```