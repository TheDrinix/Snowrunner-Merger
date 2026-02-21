using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using SnowrunnerMerger.Desktop.Services.Interfaces;
using SnowrunnerMerger.Desktop.ViewModels;

namespace SnowrunnerMerger.Desktop.Services;

public partial class RouterService : ObservableObject, IRouterService
{
    private readonly IServiceProvider _serviceProvider;
    
    [ObservableProperty]
    private PageViewModel _currentView;

    public RouterService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<TViewModel>() where TViewModel : PageViewModel
    {
        CurrentView = _serviceProvider.GetRequiredService<TViewModel>();
    }
}