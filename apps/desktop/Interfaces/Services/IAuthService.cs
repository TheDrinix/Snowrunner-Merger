using System.Threading.Tasks;

namespace SnowrunnerMerger.Desktop.Interfaces.Services;

public interface IAuthService
{
    Task<bool> IsAuthenticatedAsync();
    Task<bool> LoginAsync();
    Task<string?> GetAccessTokenAsync();
    Task LogoutAsync();
}