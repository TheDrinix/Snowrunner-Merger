using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using SnowrunnerMerger.Desktop.Data;
using SnowrunnerMerger.Desktop.Factories;
using SnowrunnerMerger.Desktop.Services;
using SnowrunnerMerger.Desktop.Services.Interfaces;
using SnowrunnerMerger.Desktop.ViewModels;
using SnowrunnerMerger.Desktop.Views;

namespace SnowrunnerMerger.Desktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<MainWindowViewModel>();
        serviceCollection.AddTransient<HomeViewModel>();
        
        serviceCollection.AddSingleton<Func<PageName, PageViewModel>>(provider => name =>
        {
            return name switch
            {
                PageName.Home => provider.GetRequiredService<HomeViewModel>(),
                PageName.Unknown => throw new ArgumentOutOfRangeException(nameof(name), name, null),
                _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
            };
        });

        serviceCollection.AddSingleton<PageFactory>();
        serviceCollection.AddSingleton<IRouterService, RouterService>();
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}