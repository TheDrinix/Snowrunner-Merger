namespace SnowrunnerMerger.Api.Models.Auth.OAuth;

public enum OAuthResultTokenType
{
    AccessToken = 1 << 0,
    LinkingToken = 1 << 1,
    CompletionToken = 1 << 2
}