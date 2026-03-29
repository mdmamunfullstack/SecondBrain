using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RefreshTokenDemo.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RefreshTokenDemo.Services
{
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
}
