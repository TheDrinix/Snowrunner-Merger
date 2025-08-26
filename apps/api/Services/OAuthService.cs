using System.Net;
using Microsoft.EntityFrameworkCore;
using SnowrunnerMerger.Api.Data;
using SnowrunnerMerger.Api.Exceptions;
using SnowrunnerMerger.Api.Models.Auth;
using SnowrunnerMerger.Api.Models.Auth.Dtos;
using SnowrunnerMerger.Api.Models.Auth.OAuth;
using SnowrunnerMerger.Api.Models.Auth.Tokens;
using SnowrunnerMerger.Api.Services.Interfaces;

namespace SnowrunnerMerger.Api.Services;

public abstract class OAuthService(
    AppDbContext dbContext, 
    ITokenService tokenService, 
    IConfiguration config,
    ILogger<OAuthService> logger
)
{
    public abstract string ProviderName { get; }

    /// <summary>
    ///     Attempts to sign in or register a user using OAuth2 code.
    ///     If the user does not exist, returns a token for account setup.
    /// </summary>
    /// <param name="code">
    ///     The authorization code received from the OAuth provider.
    /// </param>
    /// <param name="redirectUri">
    ///     The redirect URI used in the OAuth flow.
    /// </param>
    /// <returns>
    ///     An <see cref="OAuthSignInResult"/> indicating the outcome of the sign-in attempt.
    /// </returns>
    /// <exception cref="HttpResponseException"></exception>
    public async Task<OAuthSignInResult> OAuthSignIn(string code, string redirectUri)
    {
        var userData = await GetOAuthAccountData(code, redirectUri);

        if (userData is null)
        {
            throw new HttpResponseException(HttpStatusCode.Unauthorized, "Failed to retrieve user data from OAuth provider");
        }
        
        var existingConnection = await dbContext.OAuthConnections
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Provider == ProviderName && c.ProviderAccountId == userData.Id);

        if (existingConnection is null)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.NormalizedEmail == userData.Email.ToUpper());

            if (user is not null)
            {
                var accountLinkingToken = await tokenService.GenerateLinkingToken(user, ProviderName, userData.Id);
                
                return new OAuthSignInResult.OAuthSignInLinkRequired(accountLinkingToken);
            }
            
            var accountCompletionToken = await tokenService.GenerateCompletionToken(userData.Email, ProviderName, userData.Id);
            
            return new OAuthSignInResult.OAuthSignInAccountSetupRequired(accountCompletionToken);
        }
        
        var existingUser = existingConnection.User;

        if (!existingUser.EmailConfirmed)
        {
            existingUser.EmailConfirmed = true;
            dbContext.Users.Update(existingUser);
            await dbContext.SaveChangesAsync();
        }

        var refreshTokenData = await tokenService.GenerateRefreshToken(existingUser);
        
        var jwt = tokenService.GenerateJwt(new JwtData()
        {
            Id = existingUser.Id,
            Email = existingUser.Email,
            Username = existingUser.Username,
            SessionId = refreshTokenData.Session.Id
        });
        
        return new OAuthSignInResult.OAuthSignInSuccess(new LoginResponseDto()
        {
            AccessToken = jwt,
            ExpiresIn = ITokenService.AccessTokenLifetime,
            User = existingUser
        });
    }

    /// <summary>
    ///     Attempts to link an OAuth provider to an existing user account using a linking token.
    ///     User gets signed in if the linking is successful.
    /// </summary>
    /// <param name="linkingToken">
    ///     The linking token used to link the OAuth provider.
    /// </param>
    /// <returns>
    ///     A <see cref="LoginResponseDto"/> object containing the access token, expiration time, and user information on success.
    /// </returns>
    /// <exception cref="HttpResponseException">
    ///     Thrown with different HTTP status codes depending on the validation failure:
    ///     <list type="bullet">
    ///         <item>
    ///             HttpStatusCode.BadRequest (400): If the user already has a linked account for the provider.
    ///         </item>
    ///         <item>
    ///             HttpStatusCode.Unauthorized (401): If the linking token is invalid or expired.
    ///         </item>
    ///         <item>
    ///             HttpStatusCode.Conflict (409): If the OAuth account is already linked to another user.
    ///         </item>
    ///     </list>
    /// </exception>
    public async Task<LoginResponseDto> LinkOAuthProvider(string linkingToken)
    {
        var linkingTokenEntry = await dbContext.UserTokens
            .OfType<AccountLinkingToken>()
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == linkingToken);

        if (linkingTokenEntry is null || linkingTokenEntry.ExpiresAt < DateTime.UtcNow ||
            linkingTokenEntry.Provider != ProviderName)
        {
            throw new HttpResponseException(HttpStatusCode.Unauthorized, "Failed to link OAuth provider");
        }
        
        dbContext.UserTokens.Remove(linkingTokenEntry);
        var user = await LinkOAuthProvider(linkingTokenEntry.ProviderAccountId, linkingTokenEntry.User);
        
        var refreshTokenData = await tokenService.GenerateRefreshToken(user);
        
        var jwt = tokenService.GenerateJwt(new JwtData()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            SessionId = refreshTokenData.Session.Id
        });

        return new LoginResponseDto()
        {
            AccessToken = jwt,
            ExpiresIn = ITokenService.AccessTokenLifetime,
            User = user
        };
    }
    
    /// <summary>
    ///     Attempts to link an OAuth provider to an existing user account using the provided OAuth2 code.
    /// </summary>
    /// <param name="user">
    ///     The user to link the OAuth provider account to.
    /// </param>
    /// <param name="code">
    ///     The OAuth2 code used to exchange for an access token.
    /// </param>
    /// <param name="redirectUri">
    ///     The redirect URI used in the OAuth flow.
    /// </param>
    /// <returns>The updated user.</returns>
    /// <exception cref="HttpResponseException">
    ///     Thrown with different HTTP status codes depending on the validation failure:
    ///     <list type="bullet">
    ///         <item>
    ///             HttpStatusCode.BadRequest (400): If the user already has a linked account for the provider,
    ///             or there was an error trying to receive the OAuth account data.
    ///         </item>
    ///         <item>
    ///             HttpStatusCode.Conflict (409): If the OAuth account is already linked to another user.
    ///         </item>
    ///     </list>
    /// </exception>
    public async Task<User> LinkOAuthProvider(User user, string code, string redirectUri)
    {
        var userData = await GetOAuthAccountData(code, redirectUri);

        if (userData is null)
        {
            throw new HttpResponseException(HttpStatusCode.Unauthorized, "Failed to retrieve user data from OAuth provider");
        }

        return await LinkOAuthProvider(userData.Id, user);
    }
    
    public async Task UnlinkOAuthProvider(User user)
    {
        var existingConnection = await dbContext.OAuthConnections
            .FirstOrDefaultAsync(c => c.Provider == ProviderName && c.UserId == user.Id);
        
        if (existingConnection is null)
        {
            return;
        }

        dbContext.OAuthConnections.Remove(existingConnection);
        await dbContext.SaveChangesAsync();
    }

    public string GetCallbackUrl(string baseUrl)
    {
        var credentials = GetOAuthCredentials();
        
        if (credentials.RedirectUrl is not null)
            return credentials.RedirectUrl;
     
        var url = new Uri(new Uri(baseUrl), $"/api/auth/{ProviderName}/callback");
        
        return url.ToString();
    }

    public OAuthCredentials GetOAuthCredentials()
    {
        var credentials = config
            .GetSection("Authentication:OAuth")
            .GetSection(ProviderName)
            .Get<OAuthCredentials>();

        if (credentials is null || string.IsNullOrEmpty(credentials.ClientId) ||
            string.IsNullOrEmpty(credentials.ClientSecret))
        {
            logger.LogError("Failed to retrieve OAuth credentials for provider {ProviderName}", ProviderName);
            throw new ArgumentException("Failed to retrieve OAuth credentials for provider {ProviderName}", ProviderName);
        }
        
        return credentials;
    }
    
    public abstract string GetSignInUrl(string baseUrl, string state, string? callbackUrl = null);
    protected abstract Task<OAuthUserData?> GetOAuthAccountData(string code, string redirectUri);

    /// <summary>
    ///     Links an OAuth provider account to the specified user.
    /// </summary>
    /// <param name="providerAccountId">
    ///     The unique identifier of the OAuth provider account.
    /// </param>
    /// <param name="user">The user to link the OAuth provider account to.</param>
    /// <returns>The updated user.</returns>
    /// <exception cref="HttpResponseException">
    ///     Thrown with different HTTP status codes depending on the validation failure:
    ///     <list type="bullet">
    ///         <item>
    ///             HttpStatusCode.BadRequest (400): If the user already has a linked account for the provider.
    ///         </item>
    ///         <item>
    ///             HttpStatusCode.Conflict (409): If the OAuth account is already linked to another user.
    ///         </item>
    ///     </list>
    /// </exception>
    public async Task<User> LinkOAuthProvider(string providerAccountId, User user)
    {
        var existingConnection = await dbContext.OAuthConnections
            .FirstOrDefaultAsync(c => c.Provider == ProviderName && c.UserId == user.Id);
        
        if (existingConnection is not null)
        {
            throw new HttpResponseException(HttpStatusCode.BadRequest, $"User already has a linked account for {ProviderName}");
        }
        
        try
        {
            var newConnection = new OAuthConnection()
            {
                Provider = ProviderName,
                ProviderAccountId = providerAccountId,
                UserId = user.Id
            };

            dbContext.OAuthConnections.Add(newConnection);
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            if (e.InnerException is Npgsql.PostgresException { SqlState: "23505" })
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, "This OAuth account is already linked to another user");
            }
            
            throw;
        }
        
        return user;
    }
}