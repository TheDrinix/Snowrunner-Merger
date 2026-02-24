using SnowrunnerMerger.Api.Models.Auth;
using SnowrunnerMerger.Api.Models.Auth.Dtos;
using SnowrunnerMerger.Api.Models.Auth.Tokens;
using SnowrunnerMerger.Shared.DTOs.Auth;

namespace SnowrunnerMerger.Api.Services.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Registers a new user using the provided data.
    /// </summary>
    /// <param name="data">A <see cref="RegisterDto"/> object containing the user's registration details.</param>
    /// <returns>A <see cref="AccountConfirmationToken"/> object containing the confirmation token for the user.</returns>
    Task<AccountConfirmationToken> Register(RegisterDto data);
    /// <summary>
    /// Attempts to log in a user with the provided credentials.
    /// </summary>
    /// <param name="data">A <see cref="LoginDto"/> object containing the user's email and password.</param>
    /// <returns>A <see cref="LoginResponseDto"/> object containing the access token, expiration time, and user information on success.</returns>
    Task<LoginResponseDto> Login(LoginDto data);
    /// <summary>
    ///  Attempts to refresh the access token using the provided refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token used to generate a new access token.</param>
    /// <param name="isCookieToken">A boolean indicating whether the refresh token is stored in a cookie.</param>
    /// <param name="extendedLifespan">A boolean indicating whether the new refresh token should have an extended lifespan.</param>
    /// <returns>A <see cref="RefreshResponseDto"/> object containing the new access token, expiration time, and user information on success.</returns>
    Task<RefreshResponseDto> RefreshToken(string refreshToken, bool isCookieToken = true);
    /// <summary>
    ///     Retrieves a long-lived refresh token for the user.
    /// </summary>
    /// <param name="userId">The ID of the user to generate the refresh token for.</param>
    /// <returns>A <see cref="RefreshTokenDto"/> object containing the long-lived refresh token and expiration time on success.</returns>
    Task<RefreshTokenDto> GetLongLivedRefreshToken(Guid userId);
    /// <summary>
    ///     Generates a new OAuth2 state token and stores it in a cookie.
    /// </summary>
    /// <returns>The hashed OAuth2 state token.</returns>
    string GenerateOauthStateToken();
    /// <summary>
    ///     Validates the OAuth2 state token against the stored cookie.
    /// </summary>
    /// <param name="state">The OAuth2 state token to validate.</param>
    /// <returns>True if the token is valid, false otherwise.</returns>
    bool ValidateOauthStateToken(string state);
    /// <summary>
    ///     Attempts to finish the account for a user using the provided data.
    ///     User gets signed in if the account setup is successful.
    /// </summary>
    /// <param name="data">A <see cref="FinishAccountSetupDto"/> object containing the user's account details.</param>
    /// <param name="oauthService">
    ///     The OAuth service used to handle the OAuth2 flow (e.g., Google, Facebook).
    /// </param>
    /// <returns>A <see cref="LoginResponseDto"/> object containing the access token, expiration time, and user information on success.</returns>
    Task<LoginResponseDto> FinishAccountSetup(FinishAccountSetupDto data, OAuthService oauthService);
    /// <summary>
    ///     Verifies the email of a user using the provided confirmation token.
    /// </summary>
    /// <param name="token">The confirmation token used to verify the email.</param>
    /// <returns>True if the email was successfully verified, false otherwise.</returns>
    Task<bool> VerifyEmail(string token);
    /// <summary>
    ///     Logs out the current user by removing the session from the database and deleting the refresh token cookie.
    /// </summary>
    /// <param name="refreshToken">The refresh token to invalidate. This is used to ensure that the token cannot be used to generate new access tokens after the user has logged out. </param>
    /// <param name="isCookieToken">A boolean indicating whether the refresh token is stored in a cookie. If true, the cookie will be deleted after logout.</param>
    Task Logout(string refreshToken, bool isCookieToken = true);
    /// <summary>
    ///     Generates a confirmation token for the user with the provided email.
    /// </summary>
    /// <param name="email">The email of the user to generate the confirmation token for.</param>
    /// <returns>A <see cref="AccountConfirmationToken"/> object containing the confirmation token on success, null otherwise.</returns>
    Task<AccountConfirmationToken?> GenerateConfirmationToken(string email);
    /// <summary>
    ///     Generates a password reset token for the user with the provided email.
    /// </summary>
    /// <param name="email">The email of the user to generate the password reset token for.</param>
    /// <returns>A <see cref="PasswordResetToken"/> object containing the password reset token on success, null otherwise.</returns>
    Task<PasswordResetToken?> GeneratePasswordResetToken(string email);
    /// <summary>
    ///     Resets the password of a user using the provided data.
    /// </summary>
    /// <param name="data">A <see cref="ResetPasswordDto"/> object containing the user's ID, token, and new password.</param>
    Task ResetPassword(ResetPasswordDto data);

    /// <summary>
    ///     Updates the password of the user.
    /// </summary>
    /// <param name="user">The user whose password is being updated.</param>
    /// <param name="data">A <see cref="UpdatePasswordDto"/> object containing the user's current password and new password.</param>
    /// <returns>The updated user.</returns>
    Task<User> UpdatePassword(User user, UpdatePasswordDto data);
    
    /// <summary>
    ///     Generates an authorization code for the user with the provided ID and code challenge.
    /// </summary>
    /// <param name="userId">The ID of the user to generate the authorization code for.</param>
    /// <param name="codeChallenge"></param>
    /// <returns></returns>
    string GenerateAuthCode(Guid userId, string codeChallenge);
    /// <summary>
    ///     Exchanges the provided authorization code for an access token and refresh token.
    /// </summary>
    /// <param name="code">The authorization code to retrieve the data for.</param>
    /// <param name="codeVerifier">The code verifier to validate against the code challenge stored with the authorization code. This is used to ensure that the request is coming from the same client that initiated the authorization request.</param>
    /// <returns>A <see cref="TokensDto"/> object containing the access token, refresh token, and expiration time on success.</returns>
    Task<TokensDto> ExchangeAuthCode(string code, string codeVerifier);
}

