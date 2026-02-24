using System;
using System.Buffers.Text;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using SnowrunnerMerger.Api.Models.Auth.Dtos;
using SnowrunnerMerger.Desktop.Interfaces.Services;
using SnowrunnerMerger.Desktop.Models.Auth;
using SnowrunnerMerger.Desktop.Utils.Browser;
using SnowrunnerMerger.Shared.DTOs.Auth;

namespace SnowrunnerMerger.Desktop.Services.Auth;

public class AuthService(IHttpClientFactory httpClientFactory, IConfiguration config) : IAuthService
{
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
    private static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SnowrunnerMerger");
    private AccessTokenData? _accessTokenData = null;
    

    public async Task<bool> IsAuthenticatedAsync() => !string.IsNullOrEmpty(await GetAccessTokenAsync());

    public async Task<bool> LoginAsync()
    {
        var state = GenerateRandomKey();
        var pkce = new Pkce(GenerateRandomKey());

        var port = 7890;
        // Check if the port is available
        while (true)
        {
            try
            {
                using var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, port);
                listener.Start();
                break; // Port is available
            }
            catch (System.Net.Sockets.SocketException)
            {
                port++; // Try the next port
            }
        }
        
        var redirectUri = $"http://127.0.0.1:{port}";

        var baseFrontendUrl = config.GetValue<string>("FrontendBaseUrl") ?? "http://localhost:5173/";
        
        var uriBuilder = new UriBuilder($"{baseFrontendUrl}oauth/authorize")
        {
            Query = "response_type=code" +
                    "&client_id=smd" +
                    $"&redirect_uri={redirectUri}" +
                    "&scope=openid profile offline_access" +
                    $"&state={state}" +
                    $"&code_challenge={pkce.CodeChallenge}"
        };


        var browserOptions = new BrowserOptions(uriBuilder.ToString(), redirectUri);
        var browser = new SystemBrowser();

        var browserResult = await browser.InvokeAsync(browserOptions);
        
        if (string.IsNullOrEmpty(browserResult))
        {
            return false;
        }
        
        var responseData = ParseResponseQueryData(browserResult);
        
        if (responseData.State != state)
        {
            return false;
        }
        
        var tokens = await RedeemCodeAsync(responseData.Code, pkce);
        if (tokens == null)
        {
            return false;
        }
        
        _accessTokenData = new AccessTokenData(tokens.AccessToken, DateTime.Now.AddSeconds(tokens.ExpiresIn));

        var provider = DataProtectionProvider.Create(
            new DirectoryInfo(Path.Combine(AppDataPath, "keys"))
        );
        
        var tokenStore = new SecureTokenStore(provider, AppDataPath);
        
        tokenStore.Save(tokens.RefreshToken);
        
        return true;
    }
    
    public async Task<string?> GetAccessTokenAsync()
    {
        if (_accessTokenData is null || _accessTokenData.ExpiresAt <= DateTime.Now)
        {
            var httpClient = httpClientFactory.CreateClient("api");
            
            var provider = DataProtectionProvider.Create(
                new DirectoryInfo(Path.Combine(AppDataPath, "keys"))
            );

            var tokenStore = new SecureTokenStore(provider, AppDataPath);
            
            var refreshToken = tokenStore.Load();
            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            var data = new RefreshDto()
            {
                Token = refreshToken
            };
            
            var content = JsonContent.Create(data);

            var response = await httpClient.PostAsync("auth/refresh", content);
            
            if (!response.IsSuccessStatusCode)
            {
                tokenStore.Clear();
                return null;
            }
            
            var tokens = await response.Content.ReadFromJsonAsync<RefreshResponseDto>();
            if (tokens == null)
            {
                tokenStore.Clear();
                return null;
            }
            
            _accessTokenData = new AccessTokenData(tokens.AccessToken, DateTime.Now.AddSeconds(tokens.ExpiresIn));
            tokenStore.Save(tokens.RefreshToken!);
        }
        
        return _accessTokenData?.Token;
    }

    public async Task LogoutAsync()
    {
        var provider = DataProtectionProvider.Create(
            new DirectoryInfo(Path.Combine(AppDataPath, "keys"))
        );
        
        var tokenStore = new SecureTokenStore(provider, AppDataPath);
        
        var refreshToken = tokenStore.Load();

        if (string.IsNullOrEmpty(refreshToken))
        {
            _accessTokenData = null;
            return;
        }
        
        var httpClient = httpClientFactory.CreateClient("api");
        var data = new RefreshDto()
        {
            Token = refreshToken
        };
        
        var content = JsonContent.Create(data);
        
        await httpClient.PostAsync("auth/logout", content);
        
        tokenStore.Clear();
        _accessTokenData = null;
    }

    private string GenerateRandomKey(int length = 32)
    {
        var bytes = new byte[length];
        Rng.GetBytes(bytes);
        
        return Base64Url.EncodeToString(bytes);
    }

    private CodeResponseData ParseResponseQueryData(string responseUrl)
    {
        var uri = new Uri(responseUrl);
        
        var query = uri.Query;
        
        var queryParams = System.Web.HttpUtility.ParseQueryString(query);
        
        var code = queryParams["code"];
        var state = queryParams["state"];
        var scope = queryParams["scope"];
        
        if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
        {
            throw new InvalidOperationException("Invalid response URL");
        }
        
        return new CodeResponseData(code, state, scope ?? string.Empty);
    }

    private async Task<TokensDto?> RedeemCodeAsync(string code, Pkce pkce)
    {
        var tokenRequest = new TokenRequestDto(
            "smd", 
            "authorization_code", 
            code,
            pkce.CodeVerifier
        );
        
        var httpClient = httpClientFactory.CreateClient("api");
        
        var response = await httpClient.PostAsJsonAsync("auth/oauth/token", tokenRequest);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Token request failed with status code {response.StatusCode}");
        }
        
        var tokens = await response.Content.ReadFromJsonAsync<TokensDto>();

        return tokens;
    }

    private class CodeResponseData
    {
        public string Code { get; }
        public string State { get; }
        public string Scope { get; }
        
        public CodeResponseData(string code, string state, string scope)
        {
            Code = code;
            State = state;
            Scope = scope;
        }
    }
    
    private class Pkce
    {
        public string CodeVerifier { get; }
        public string CodeChallenge { get; }

        
        public Pkce(string codeVerifier)
        {
            CodeVerifier = codeVerifier;

            var challengeBytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(codeVerifier));
            CodeChallenge = Base64Url.EncodeToString(challengeBytes);
        }
        
        public bool Validate(string codeVerifier)
        {
            var challengeBytes = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(codeVerifier));
            var codeChallenge = Base64Url.EncodeToString(challengeBytes);
            return codeChallenge == CodeChallenge;
        }
    }
}