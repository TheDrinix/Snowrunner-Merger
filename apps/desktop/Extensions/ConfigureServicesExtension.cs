using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnowrunnerMerger.Desktop.Data;
using SnowrunnerMerger.Desktop.Factories;
using SnowrunnerMerger.Desktop.Models.Config;
using SnowrunnerMerger.Desktop.Services.Auth;
using SnowrunnerMerger.Desktop.ViewModels;

namespace SnowrunnerMerger.Desktop.Extensions;

public static class ConfigureServicesExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        var config = services.LoadConfiguration();
        
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddTransient<LoginViewModel>();

        // Routing
        services.RegisterPageFactory();
        services.AddSingleton<PageFactory>();
        // Http Clients
        var baseApiUrl = config.GetSection("ApiSettings:BaseUrl").Value ?? "https://localhost:44303/api/";
        services.RegisterHttpClients(baseApiUrl);

        return services;
    }

    private static IConfiguration LoadConfiguration(this IServiceCollection services)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();
        
        services.AddSingleton<IConfiguration>(config);
        services.Configure<ApiSettings>(config.GetSection("ApiSettings"));
        
        return config;
    }
    
    private static IServiceCollection RegisterPageFactory(this IServiceCollection services)
    {
        // Register the Page factory as before
        services.AddSingleton<Func<PageName, PageViewModel>>(provider => name =>
        {
            return name switch
            {
                PageName.Home => provider.GetRequiredService<HomeViewModel>(),
                PageName.Login => provider.GetRequiredService<LoginViewModel>(),
                PageName.Unknown => throw new ArgumentOutOfRangeException(nameof(name), name, null),
                _ => throw new ArgumentOutOfRangeException(nameof(name), name, null)
            };
        });

        return services;
    }

    private static IServiceCollection RegisterHttpClients(this IServiceCollection services, string baseApiUrl)
    {
        services.AddTransient<AuthHeaderHandler>();
        services.AddHttpClient("api", client =>
        {
            client.BaseAddress = new Uri(baseApiUrl);
        });
        services.AddHttpClient("api-auth", client =>
        {
            client.BaseAddress = new Uri(baseApiUrl);
        }).AddHttpMessageHandler<AuthHeaderHandler>();
        
        return services;
    }
}