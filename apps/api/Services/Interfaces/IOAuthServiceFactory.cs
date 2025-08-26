namespace SnowrunnerMerger.Api.Services.Interfaces;

public interface IOAuthServiceFactory
{
    OAuthService GetService(string providerName);
}