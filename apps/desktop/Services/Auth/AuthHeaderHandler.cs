using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SnowrunnerMerger.Desktop.Interfaces.Services;

namespace SnowrunnerMerger.Desktop.Services.Auth;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IAuthService _authService;
    
    public AuthHeaderHandler(IAuthService authService)
    {
        _authService = authService;
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = await _authService.GetAccessTokenAsync();
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }
        
        return await base.SendAsync(request, cancellationToken);
    }
}