using SnowrunnerMerger.Api.Models.Auth;
using SnowrunnerMerger.Api.Models.Auth.Tokens;

namespace SnowrunnerMerger.Api.Services.Interfaces;

public interface ITokenService
{
    public const int AccessTokenLifetime = 60 * 30; // 30 minutes in seconds
    public const int RefreshTokenLifetime = 60 * 60 * 24 * 30; // 30 days in seconds

    /// <summary>
    /// Generates a confirmation token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the confirmation token is generated.</param>
    /// <returns>A <see cref="AccountConfirmationToken"/> object containing the generated token.</returns>
    /// <remarks>
    /// This method generates a unique confirmation token for the user and stores it in the database.
    /// The token is valid for 1 hour from the time of generation.
    /// </remarks>
    Task<AccountConfirmationToken> GenerateConfirmationToken(User user);
    /// <summary>
    ///     Generates a password reset token for the user with the provided email.
    /// </summary>
    /// <param name="user">The user for whom the password reset token is generated.</param>
    /// <returns>A <see cref="PasswordResetToken"/> object containing the password reset token on success, null otherwise.</returns>
    Task<PasswordResetToken?> GeneratePasswordResetToken(User user);
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
    Task<AccountCompletionToken> GenerateCompletionToken(string email, string oAuthProvider, string providerAccountId);
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
    Task<AccountLinkingToken> GenerateLinkingToken(User user, string oAuthProvider, string providerAccountId);
    /// <summary>
    ///     Generates a refresh token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the refresh token is generated.</param>
    /// <param name="extendedLifespan">A boolean indicating whether the refresh token should have an extended lifespan.</param>
    /// <param name="storeInCookie">A boolean indicating whether the refresh token should be stored in a cookie.</param>
    /// <returns>A <see cref="RefreshTokenData"/> object containing the generated refresh token.</returns>
    Task<RefreshTokenData> GenerateRefreshToken(User user, bool extendedLifespan = false, bool storeInCookie = true);
    /// <summary>
    ///     Validates the refresh token and generates a new one if it is valid.
    /// </summary>
    /// <param name="token">The refresh token to validate.</param>
    /// <returns>A <see cref="RefreshTokenData"/> object containing the user session data if the token is valid, null otherwise.</returns>
    Task<RefreshTokenData?> ValidateRefreshToken(string token);
    /// <summary>
    ///     Generates a JWT token using the provided data.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom the JWT token is generated.</param>
    /// <param name="role">The role of the user (default is "User").</param>
    /// <returns>The generated JWT token.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the JWT secret is not found in the configuration.</exception>
    string GenerateJwt(Guid userId, string role = "User");
}