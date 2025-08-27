using SnowrunnerMerger.Api.Models.Auth.Dtos;
using SnowrunnerMerger.Api.Models.Auth.Tokens;

namespace SnowrunnerMerger.Api.Models.Auth.OAuth;

public abstract record OAuthSignInResult
{
    public record OAuthSignInSuccess(LoginResponseDto data) : OAuthSignInResult;
    public record OAuthSignInLinkRequired(AccountLinkingToken linkingToken) : OAuthSignInResult;
    public record OAuthSignInAccountSetupRequired(AccountCompletionToken completionToken) : OAuthSignInResult;
};