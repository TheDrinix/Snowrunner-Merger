namespace SnowrunnerMerger.Api.Services.Interfaces;

public interface IOAuthServiceFactory
{
    public string[] ProviderNames { get; }
    OAuthService GetService(string providerName);
}