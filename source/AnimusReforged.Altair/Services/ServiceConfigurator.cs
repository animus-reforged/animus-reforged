using System;
using AnimusReforged.Altair.ViewModels;
using AnimusReforged.Altair.ViewModels.Pages;
using AnimusReforged.Altair.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.Services;

/// <summary>
/// Provides centralized service configuration and registration for the Altair application,
/// managing dependency injection container setup and service lifecycle management.
/// </summary>
public static class ServiceConfigurator
{
    /// <summary>
    /// Configures and registers all application services with the dependency injection container.
    /// This method sets up the service collection with all required services and returns
    /// a built service provider ready for use.
    /// </summary>
    /// <returns>An IServiceProvider instance with all configured services.</returns>
    public static IServiceProvider ConfigureServices()
    {
        ServiceCollection services = new ServiceCollection();

        // Register Services
        services.AddSingleton<NavigationService>();

        // Register ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<WelcomePageViewModel>();
        services.AddSingleton<CreditsPageViewModel>();

        // Register Views/Windows
        services.AddSingleton<MainWindow>();

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }
}