using SnowrunnerMerger.Api.Services.Interfaces;

namespace SnowrunnerMerger.Api.Services;

public class OAuthServiceFactory(IServiceProvider provider) : IOAuthServiceFactory
{
    public string[] ProviderNames => ["google", "discord"];
    public OAuthService GetService(string providerName)
    {
        return providerName.ToLower() switch
        {
            "google" => provider.GetRequiredService<OAuthProviders.GoogleOAuthService>(),
            "discord" => provider.GetRequiredService<OAuthProviders.DiscordOAuthService>(),
            _ => throw new NotSupportedException($"OAuth provider '{providerName}' is not supported.")
        };
    }
}