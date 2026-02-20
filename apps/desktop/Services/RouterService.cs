using System;
using CommunityToolkit.Mvvm.ComponentModel;
using SnowrunnerMerger.Desktop.Data;
using SnowrunnerMerger.Desktop.Factories;
using SnowrunnerMerger.Desktop.Services.Interfaces;
using SnowrunnerMerger.Desktop.ViewModels;

namespace SnowrunnerMerger.Desktop.Services;

public partial class RouterService(Func<PageName, PageViewModel> pageFactory) : ObservableObject, IRouterService
{
    [ObservableProperty]
    private PageViewModel? _currentView = null;

    public void NavigateTo(PageName pageName)
    {
        Console.WriteLine("Navigating to " + pageName);
        CurrentView = pageFactory(pageName);
    }
}