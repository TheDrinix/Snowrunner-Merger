using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using SnowrunnerMerger.Desktop.Data;
using SnowrunnerMerger.Desktop.Extensions;
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

        serviceCollection.ConfigureServices();
        
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // After building the provider, initialize the router to the Login page to avoid circular DI
        var router = serviceProvider.GetRequiredService<IRouterService>();
        router.NavigateTo(PageName.Login);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var authService = serviceProvider.GetRequiredService<IAuthService>();
            
            _ = Task.Run(async () =>
            {
                try
                {
                    var isAuthenticated = await authService.IsAuthenticatedAsync();

                    // marshal back to the UI thread to navigate
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        if (!isAuthenticated)
                        {
                            router.NavigateTo(PageName.Login);
                        }
                        else
                        {
                            router.NavigateTo(PageName.Home);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Auth check failed: {ex}");
                    // on failure just navigate to login (safest default)
                    await Dispatcher.UIThread.InvokeAsync(() => router.NavigateTo(PageName.Login));
                }
            });
            
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