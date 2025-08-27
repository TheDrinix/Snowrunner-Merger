namespace SnowrunnerMerger.Api.Models.Auth.OAuth;

public record OAuthCredentials
{
    public string ClientId { get; init; }
    public string ClientSecret { get; init; }
    public string? RedirectUrl { get; init; }
};