using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using SnowrunnerMerger.Desktop.Data;
using SnowrunnerMerger.Desktop.Services.Interfaces;

namespace SnowrunnerMerger.Desktop.ViewModels;

public partial class LoginViewModel : PageViewModel
{
    private readonly IAuthService _authService;
    private readonly IServiceProvider _serviceProvider;
    
    public LoginViewModel(IAuthService authService, IServiceProvider serviceProvider)
    {
        Name = PageName.Login;
        
        _authService = authService;
        _serviceProvider = serviceProvider;
        // _routerService = routerService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        var res = await _authService.LoginAsync();

        if (res)
        {
            var routerService = _serviceProvider.GetRequiredService<IRouterService>();

            routerService.NavigateTo(PageName.Home);
        }
    }
}