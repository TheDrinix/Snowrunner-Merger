using SnowrunnerMerger.Api.Models.Auth.Tokens;
using SnowrunnerMerger.Shared.DTOs.Auth;

namespace SnowrunnerMerger.Api.Models.Auth.OAuth;

public abstract record OAuthSignInResult
{
    public record OAuthSignInSuccess(LoginResponseDto data) : OAuthSignInResult;
    public record OAuthSignInLinkRequired(AccountLinkingToken linkingToken) : OAuthSignInResult;
    public record OAuthSignInAccountSetupRequired(AccountCompletionToken completionToken) : OAuthSignInResult;
};