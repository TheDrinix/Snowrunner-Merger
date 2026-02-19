using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using SnowrunnerMerger.Desktop.Data;
using SnowrunnerMerger.Desktop.Services.Interfaces;

namespace SnowrunnerMerger.Desktop.ViewModels;

public partial class LoginViewModel : PageViewModel
{
    private readonly IAuthService _authService;
    
    public LoginViewModel(IAuthService authService)
    {
        Name = PageName.Login;
        
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        var res = await _authService.LoginAsync();
    }
}