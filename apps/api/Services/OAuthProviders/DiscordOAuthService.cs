using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Extensions;
using SnowrunnerMerger.Api.Data;
using SnowrunnerMerger.Api.Models.Auth.OAuth;
using SnowrunnerMerger.Api.Services.Interfaces;

namespace SnowrunnerMerger.Api.Services.OAuthProviders;

public class DiscordOAuthService(
    AppDbContext dbContext,
    ITokenService tokenService,
    IConfiguration config,
    ILogger<OAuthService> logger) : OAuthService(dbContext, tokenService, config, logger)
{
    public override string ProviderName => "discord";

    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://discord.com/api/"),
    };

    public override string GetSignInUrl(string baseUrl, string state, string? callbackUrl = null)
    {
        var credentials = GetOAuthCredentials();
        var redirectUrl = callbackUrl ?? GetCallbackUrl(baseUrl);

        const string scopes = "identify email";

        var queryBuilder = new QueryBuilder();
        queryBuilder.Add("client_id", credentials.ClientId);
        queryBuilder.Add("redirect_uri", redirectUrl);
        queryBuilder.Add("response_type", "code");
        queryBuilder.Add("scope", scopes);
        queryBuilder.Add("state", WebUtility.UrlEncode(state));

        var url = new UriBuilder("https://discord.com/api/oauth2/authorize")
        {
            Query = queryBuilder.ToString()
        }.ToString();

        return url;
    }

    protected override async Task<OAuthUserData?> GetOAuthAccountData(string code, string redirectUri)
    {
        var credentials = GetOAuthCredentials();

        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", credentials.ClientId },
            { "client_secret", credentials.ClientSecret },
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUri },

        });

        var response = await _httpClient.PostAsync("oauth2/token", body);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Failed to exchange discord code for token. Status code: {StatusCode}",
                response.StatusCode);
            return null;
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<DiscordTokenResponse>();
        
        if (tokenResponse is null)
        {
            logger.LogWarning("Failed to parse discord token response");
            return null;
        }
        
        // Get user info
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            tokenResponse.TokenType, tokenResponse.AccessToken);
        var userResponse = await _httpClient.GetAsync("users/@me");
        
        if (!userResponse.IsSuccessStatusCode)
        {
            logger.LogWarning("Failed to get discord user info. Status code: {StatusCode}",
                userResponse.StatusCode);
            return null;
        }
        
        var userData = await userResponse.Content.ReadFromJsonAsync<DiscordUserInfo>();
        
        return userData?.ToOAuthUserData();
    }

    private sealed class DiscordTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; init; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; }
        [JsonPropertyName("scope")]
        public string Scope { get; init; }
    }
    
    private sealed class DiscordUserInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; init; }
        [JsonPropertyName("username")]
        public string Username { get; init; }
        [JsonPropertyName("discriminator")]
        public string Discriminator { get; init; }
        [JsonPropertyName("email")]
        public string Email { get; init; }
        [JsonPropertyName("verified")]
        public bool? Verified { get; init; }

        public OAuthUserData? ToOAuthUserData()
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(Username))
            {
                return null;
            }

            return new OAuthUserData(Id, Email, Username);
        }
    }
}