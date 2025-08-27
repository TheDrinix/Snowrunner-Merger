using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Extensions;
using NuGet.Protocol;
using SnowrunnerMerger.Api.Data;
using SnowrunnerMerger.Api.Models.Auth.Google;
using SnowrunnerMerger.Api.Models.Auth.OAuth;
using SnowrunnerMerger.Api.Services.Interfaces;

namespace SnowrunnerMerger.Api.Services.OAuthProviders;

public class GoogleOAuthService(
    AppDbContext dbContext,
    ITokenService tokenService,
    IConfiguration config,
    ILogger<GoogleOAuthService> logger)
    : OAuthService(dbContext, tokenService, config, logger)
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://www.googleapis.com/"),
    };

    public override string ProviderName => "google";

    public override string GetSignInUrl(string baseUrl, string state, string? callbackUrl = null)
    {
        var credentials = GetOAuthCredentials();
        var redirectUrl = callbackUrl ?? GetCallbackUrl(baseUrl);
        
        const string scopes = "openid https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile";

        var queryBuilder = new QueryBuilder();
        queryBuilder.Add("client_id", credentials.ClientId);
        queryBuilder.Add("redirect_uri", redirectUrl);
        queryBuilder.Add("response_type", "code");
        queryBuilder.Add("scope", scopes);
        queryBuilder.Add("state", WebUtility.UrlEncode(state));
        queryBuilder.Add("include_granted_scopes", "true");
        queryBuilder.Add("prompt", "consent");
        
        
        
        var url = new UriBuilder("https://accounts.google.com/o/oauth2/v2/auth")
        {
            Query = queryBuilder.ToString()
        }.ToString();
        
        return url;
    }

    protected override async Task<OAuthUserData?> GetOAuthAccountData(string code, string redirectUri)
    {
        var credentials = GetOAuthCredentials();
        
        // Send http request to exchange code for token
        var url = new UriBuilder("https://oauth2.googleapis.com/token")
            .ToString();
        
        var body = new
        {
            client_id = credentials.ClientId,
            client_secret = credentials.ClientSecret,
            code = code,
            grant_type = "authorization_code",
            redirect_uri = redirectUri
        }.ToJson();
        
        var response = await _httpClient.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
        var data = await response.Content.ReadFromJsonAsync<GoogleTokenData>();
        
        if (data is null || string.IsNullOrEmpty(data.AccessToken))
        {
            return null;
        }
        
        var userData = await _httpClient.GetFromJsonAsync<GoogleUserInfo>($"https://www.googleapis.com/oauth2/v2/userinfo?access_token={data.AccessToken}");

        return userData?.ToOAuthUserData();
    }

    private sealed class GoogleUserInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("verified_email")]
        public bool VerifiedEmail { get; set; }
        [JsonPropertyName("name")] 
        public string Name { get; set; }
        [JsonPropertyName("given_name")]
        public string? FirstName { get; set; }
        
        public OAuthUserData? ToOAuthUserData()
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(Email))
            {
                return null;
            }

            return new OAuthUserData(Id, Email, FirstName ?? Name);
        }
    }
}