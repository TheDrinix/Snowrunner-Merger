using SnowrunnerMerger.Api.Models.Auth;
using SnowrunnerMerger.Api.Models.Auth.Tokens;

namespace SnowrunnerMerger.Api.Services.Interfaces;

public interface ITokenService
{
    public const int AccessTokenLifetime = 60 * 30; // 30 minutes in seconds
    public const int RefreshTokenLifetime = 60 * 60 * 24 * 30; // 30 days in seconds

    Task<AccountConfirmationToken> GenerateConfirmationToken(User user);
    Task<PasswordResetToken?> GeneratePasswordResetToken(User user);
    Task<AccountCompletionToken> GenerateCompletionToken(string email, string oAuthProvider, string providerAccountId);
    Task<AccountLinkingToken> GenerateLinkingToken(User user, string oAuthProvider, string providerAccountId);
    Task<RefreshTokenData> GenerateRefreshToken(User user, bool extendedLifespan = false, bool storeInCookie = true);
    Task<RefreshTokenData?> ValidateRefreshToken(string token);
    string GenerateJwt(Guid userId, string role = "User");
}