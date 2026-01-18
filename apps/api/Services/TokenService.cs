using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SnowrunnerMerger.Api.Data;
using SnowrunnerMerger.Api.Exceptions;
using SnowrunnerMerger.Api.Models.Auth;
using SnowrunnerMerger.Api.Models.Auth.Tokens;
using SnowrunnerMerger.Api.Services.Interfaces;

namespace SnowrunnerMerger.Api.Services;

public class TokenService : ITokenService
{
    private readonly ILogger<AuthService> _logger;
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SameSiteMode _sameSiteMode = SameSiteMode.Lax;
    private const int MaxRetries = 5;

    public TokenService(
        ILogger<AuthService> logger,
        AppDbContext dbContext,
        IConfiguration config,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment webHostEnvironment
    )
    {
        _logger = logger;
        _dbContext = dbContext;
        _config = config;
        _httpContextAccessor = httpContextAccessor;
        
        if (webHostEnvironment.IsDevelopment())
        {
            _sameSiteMode = SameSiteMode.None;
        }
    }
    
    /// <summary>
    /// Generates a confirmation token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the confirmation token is generated.</param>
    /// <returns>A <see cref="AccountConfirmationToken"/> object containing the generated token.</returns>
    /// <remarks>
    /// This method generates a unique confirmation token for the user and stores it in the database.
    /// The token is valid for 1 hour from the time of generation.
    /// </remarks>
    public async Task<AccountConfirmationToken> GenerateConfirmationToken(User user)
    {
        using var rng = RandomNumberGenerator.Create();

        for (var retry = 0; retry < MaxRetries; retry++)
        {
            var token = GenerateRandomToken(rng);

            var userConfirmationToken = new AccountConfirmationToken
            {
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Token = token
            };

            try
            {
                _dbContext.UserTokens.Add(userConfirmationToken);
                await _dbContext.SaveChangesAsync();

                return userConfirmationToken;
            }
            catch (DbUpdateException e)
            {
                if(e.InnerException is Npgsql.PostgresException { SqlState: "23505" }) // Postgres unique constraint violation
                {
                    _logger.LogWarning("Token collision detected (retry {Retry}). Generating a new token.", retry + 1);
                }
                else
                {
                    throw;
                }
            }
        }

        _logger.LogError("Failed to generate confirmation token after {MaxRetries} attempts", MaxRetries);
        throw new HttpResponseException(HttpStatusCode.InternalServerError, "Failed to generate confirmation token");
    }

    /// <summary>
    ///     Generates a password reset token for the user with the provided email.
    /// </summary>
    /// <param name="user">The user for whom the password reset token is generated.</param>
    /// <returns>A <see cref="PasswordResetToken"/> object containing the password reset token on success, null otherwise.</returns>
    public async Task<PasswordResetToken?> GeneratePasswordResetToken(User user)
    {
        using var rng = RandomNumberGenerator.Create();

        for (var retry = 0; retry < MaxRetries; retry++)
        {
            var token = GenerateRandomToken(rng);

            var passwordResetToken = new PasswordResetToken
            {
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Token = token
            };

            try
            {
                _dbContext.UserTokens.Add(passwordResetToken);
                await _dbContext.SaveChangesAsync();

                return passwordResetToken;
            }
            catch (DbUpdateException e)
            {
                if(e.InnerException is Npgsql.PostgresException { SqlState: "23505" }) // Postgres unique constraint violation
                {
                    _logger.LogWarning("Token collision detected (retry {Retry}). Generating a new token.", retry + 1);
                }
                else
                {
                    throw;
                }
            }
        }

        _logger.LogError("Failed to generate password reset token after {MaxRetries} attempts", MaxRetries);
        throw new HttpResponseException(HttpStatusCode.InternalServerError, "Failed to generate password reset token");
    }

    /// <summary>
    ///     Generates an account completion token for the specified email and Google ID.
    /// </summary>
    /// <param name="email">
    ///     The email of the user for whom the account completion token is generated.
    /// </param>
    /// <param name="oAuthProvider">
    ///     The OAuth provider (e.g., "Google") associated with the account.
    /// </param>
    /// <param name="providerAccountId">
    ///     The unique identifier provided by the OAuth provider for the user's account.
    /// </param>
    /// <returns>A <see cref="AccountCompletionToken"/> object containing the generated token.</returns>
    /// <exception cref="HttpResponseException">
    ///     Thrown with an HTTP status code of HttpStatusCode.InternalServerError (500) if the token cannot be generated.
    /// </exception>
    public async Task<AccountCompletionToken> GenerateCompletionToken(string email, string oAuthProvider, string providerAccountId)
    {
        using var rng = RandomNumberGenerator.Create();

        for (var retry = 0; retry < MaxRetries; retry++)
        {
            var token = GenerateRandomToken(rng);

            var accountCompletionToken = new AccountCompletionToken
            {
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Token = token,
                Email = email,
                Provider = oAuthProvider,
                ProviderAccountId = providerAccountId
            };

            try
            {
                _dbContext.UserTokens.Add(accountCompletionToken);
                await _dbContext.SaveChangesAsync();

                return accountCompletionToken;
            }
            catch (DbUpdateException e)
            {
                if(e.InnerException is Npgsql.PostgresException { SqlState: "23505" }) // Postgres unique constraint violation
                {
                    _logger.LogWarning("Token collision detected (retry {Retry}). Generating a new token.", retry + 1);
                }
                else
                {
                    throw;
                }
            }
        }

        _logger.LogError("Failed to generate account completion token after {MaxRetries} attempts", MaxRetries);
        throw new HttpResponseException(HttpStatusCode.InternalServerError, "Failed to generate account completion token");
    }

    /// <summary>
    ///     Generates an account linking token for the specified user and Google ID.
    /// </summary>
    /// <param name="user">The user for whom the account linking token is generated.</param>
    /// <param name="oAuthProvider">
    ///     The OAuth provider (e.g., "Google") associated with the account.
    /// </param>
    /// <param name="providerAccountId">
    ///     The unique identifier provided by the OAuth provider for the user's account.
    /// </param>
    /// <returns>A <see cref="AccountLinkingToken"/> object containing the generated token.</returns>
    /// <exception cref="HttpResponseException">
    ///     Thrown with an HTTP status code of HttpStatusCode.InternalServerError (500) if the token cannot be generated.
    /// </exception>
    public async Task<AccountLinkingToken> GenerateLinkingToken(User user, string oAuthProvider, string providerAccountId)
    {
        using var rng = RandomNumberGenerator.Create();

        for (var retry = 0; retry < MaxRetries; retry++)
        {
            var token = GenerateRandomToken(rng);

            var accountLinkingToken = new AccountLinkingToken()
            {
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Token = token,
                Provider = oAuthProvider,
                ProviderAccountId = providerAccountId,
                User = user
            };

            try
            {
                _dbContext.UserTokens.Add(accountLinkingToken);
                await _dbContext.SaveChangesAsync();

                return accountLinkingToken;
            }
            catch (DbUpdateException e)
            {
                if(e.InnerException is Npgsql.PostgresException { SqlState: "23505" }) // Postgres unique constraint violation
                {
                    _logger.LogWarning("Token collision detected (retry {Retry}). Generating a new token.", retry + 1);
                }
                else
                {
                    throw;
                }
            }
        }

        _logger.LogError("Failed to generate account linking token after {MaxRetries} attempts", MaxRetries);
        throw new HttpResponseException(HttpStatusCode.InternalServerError, "Failed to generate account linking token");
    }

    /// <summary>
    ///     Generates a refresh token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the refresh token is generated.</param>
    /// <param name="extendedLifespan">A boolean indicating whether the refresh token should have an extended lifespan.</param>
    /// <param name="storeInCookie">A boolean indicating whether the refresh token should be stored in a cookie.</param>
    /// <returns>A <see cref="RefreshTokenData"/> object containing the generated refresh token.</returns>
    public async Task<RefreshTokenData> GenerateRefreshToken(User user, bool extendedLifespan = false, bool storeInCookie = true)
    {
        for (var retry = 0; retry < MaxRetries; retry++)
        {
            var token = Guid.NewGuid().ToString();
            var encryptedToken = EncryptRefreshToken(token);
            
            var session = new UserSession
            {
                User = user,
                RefreshToken = encryptedToken,
                ExpiresAt = DateTime.UtcNow.AddSeconds(ITokenService.RefreshTokenLifetime * (extendedLifespan ? 3 : 1)),
                HasLongLivedRefreshToken = extendedLifespan
            };

            try
            {
                _dbContext.UserSessions.Add(session);
                await _dbContext.SaveChangesAsync();
            } catch (DbUpdateException e)
            {
                if(e.InnerException is Npgsql.PostgresException { SqlState: "23505" }) // Postgres unique constraint violation
                {
                    _logger.LogWarning("Refresh token collision detected (retry {Retry}). Generating a new token.", retry + 1);
                    continue;
                }

                throw;
            }
            
            if (storeInCookie)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = _sameSiteMode,
                    Expires = session.ExpiresAt,
                    Path = "/api/auth"
                };

                _httpContextAccessor.HttpContext?.Response.Cookies.Append("refresh_token", token, cookieOptions);
            }
        
            return new RefreshTokenData
            {
                Session = session,
                Token = token
            };
        }
        
        // If we reach here, it means we failed to generate a unique token after max retries
        _logger.LogError("Failed to generate refresh token after {MaxRetries} attempts", MaxRetries);
        throw new HttpResponseException(HttpStatusCode.InternalServerError, "Failed to generate refresh token");
    }

    /// <summary>
    ///     Validates the refresh token and generates a new one if it is valid.
    /// </summary>
    /// <param name="token">The refresh token to validate.</param>
    /// <returns>A <see cref="RefreshTokenData"/> object containing the user session data if the token is valid, null otherwise.</returns>
    public async Task<RefreshTokenData?> ValidateRefreshToken(string token)
    {
        var encryptedToken = EncryptRefreshToken(token);

        var session = await _dbContext.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshToken == encryptedToken);
        
        if (session is null || session.ExpiresAt < DateTime.UtcNow)
        {
            return null;
        }

        for (var retry = 0; retry < MaxRetries; retry++)
        {
            var newRefreshToken = Guid.NewGuid().ToString();
            var newEncryptedToken = EncryptRefreshToken(newRefreshToken);
            
            if (newEncryptedToken.SequenceEqual(encryptedToken))
            {
                _logger.LogWarning("Generated refresh token matches the old one (retry {Retry}). Generating a new token.", retry + 1);
                continue;
            }

            try
            {
                session.ExpiresAt = DateTime.UtcNow.AddSeconds(ITokenService.RefreshTokenLifetime * (session.HasLongLivedRefreshToken ? 3 : 1));
                session.RefreshToken = newEncryptedToken;
                await _dbContext.SaveChangesAsync();

                return new RefreshTokenData()
                {
                    Session = session,
                    Token = newRefreshToken
                };
            } catch (DbUpdateException e)
            {
                if(e.InnerException is Npgsql.PostgresException { SqlState: "23505" }) // Postgres unique constraint violation
                {
                    _logger.LogWarning("Refresh token collision detected (retry {Retry}). Generating a new token.", retry + 1);
                }
                else
                {
                    throw;
                }
            }
        }
        
        // If we reach here, it means we failed to generate a unique token after max retries
        _logger.LogError("Failed to generate new refresh token after {MaxRetries} attempts", MaxRetries);
        throw new HttpResponseException(HttpStatusCode.InternalServerError, "Failed to generate new refresh token");
    }

    /// <summary>
    ///    Retrieves the user session associated with the provided refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token used to look up the user session.</param>
    /// <returns>A <see cref="UserSession"/> object containing the user session data if found, null otherwise.</returns>
    public async Task<UserSession?> GetUserSessionFromRefreshToken(string refreshToken)
    {
        var encryptedToken = EncryptRefreshToken(refreshToken);

        var session = await _dbContext.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshToken == encryptedToken);

        return session;
    }

    /// <summary>
    ///     Generates a JWT token using the provided data.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom the JWT token is generated.</param>
    /// <param name="role">The role of the user (default is "User").</param>
    /// <returns>The generated JWT token.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the JWT secret is not found in the configuration.</exception>
    public string GenerateJwt(Guid userId, string role = "User")
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var secret = _config.GetSection("Authentication:JwtSecret").Value;

        if (secret is null)
        {
            throw new ArgumentNullException(nameof(secret));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(ITokenService.AccessTokenLifetime),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return tokenString;
    }
    
    private byte[] EncryptRefreshToken(string token)
    {
        var encryptionKey = _config.GetSection("Authentication:RefreshSecret").Value;
        
        if (encryptionKey is null)
        {
            throw new ArgumentNullException(nameof(encryptionKey));
        }

        using var aes = Aes.Create();
        
        var key = Convert.FromBase64String(encryptionKey);
        aes.Key = key;
        aes.IV = new byte[16];

        var encryptor = aes.CreateEncryptor();
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var encryptedToken = encryptor.TransformFinalBlock(tokenBytes, 0, tokenBytes.Length);

        return encryptedToken;
    }

    private string GenerateRandomToken(RandomNumberGenerator? rng, int size = 32)
    {
        rng ??= RandomNumberGenerator.Create();
        
        var tokenBytes = new byte[size];
        rng.GetBytes(tokenBytes);
        return Convert.ToBase64String(tokenBytes);
    }
}