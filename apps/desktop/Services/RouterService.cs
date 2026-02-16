using System;
using CommunityToolkit.Mvvm.ComponentModel;
using SnowrunnerMerger.Desktop.Data;
using SnowrunnerMerger.Desktop.Factories;
using SnowrunnerMerger.Desktop.Services.Interfaces;
using SnowrunnerMerger.Desktop.ViewModels;

namespace SnowrunnerMerger.Desktop.Services;

public partial class RouterService(PageFactory pageFactory) : ObservableObject, IRouterService
{
    [ObservableProperty]
    private PageViewModel _currentView = pageFactory.GetPageModel(PageName.Home);

    public void NavigateTo(PageName pageName)
    {
        Console.WriteLine("Navigating to " + pageName);
        CurrentView = pageFactory.GetPageModel(pageName);
    }
}