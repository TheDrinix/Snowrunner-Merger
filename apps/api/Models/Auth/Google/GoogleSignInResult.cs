using SnowrunnerMerger.Api.Models.Auth.Tokens;
using SnowrunnerMerger.Shared.DTOs.Auth;

namespace SnowrunnerMerger.Api.Models.Auth.Google;

public abstract record GoogleSignInResult
{
    public record GoogleSignInSuccess(LoginResponseDto data) : GoogleSignInResult;
    public record GoogleSignInLinkRequired(AccountLinkingToken linkingToken) : GoogleSignInResult;
    public record GoogleSignInAccountSetupRequired(AccountCompletionToken completionToken) : GoogleSignInResult;
};