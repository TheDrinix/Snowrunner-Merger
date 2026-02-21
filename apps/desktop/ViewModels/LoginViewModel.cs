using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using SnowrunnerMerger.Desktop.Services.Interfaces;

namespace SnowrunnerMerger.Desktop.ViewModels;

public partial class LoginViewModel(IAuthService authService, IRouterService routerService) : PageViewModel
{
    [RelayCommand]
    private async Task LoginAsync()
    {
        var res = await authService.LoginAsync();

        if (res)
        {
            routerService.NavigateTo<HomeViewModel>();
        }
    }
}