using System;
using AnimusReforged.Altair.Services.UI;
using AnimusReforged.Altair.ViewModels;
using AnimusReforged.Altair.ViewModels.Pages;
using AnimusReforged.Altair.Views;
using AnimusReforged.Logging;
using AnimusReforged.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.Services;

/// <summary>
/// Provides centralized service configuration and registration for the Altair application,
/// managing dependency injection container setup and service lifecycle management.
/// </summary>
public class ServiceConfigurator
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
        services.AddSingleton<IUpdateNotificationService, UpdateNotificationService>();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();

        // Register AltairSettings with automatic initialization
        services.AddSingleton<AltairSettings>(serviceProvider =>
        {
            AltairSettings settings = new AltairSettings();

            // Initialize settings during registration
            try
            {
                // Load settings at startup (this will create defaults if file doesn't exist)
                AltairSettings.AltairSettingsStore loadedSettings = settings.Settings; // This triggers lazy loading
            }
            catch (Exception ex)
            {
                Logger.Error<ServiceConfigurator>("Failed to initialize settings during registration");
                Logger.LogExceptionDetails<ServiceConfigurator>(ex);
                // Settings will fall back to defaults, which is handled by the settings system
            }

            return settings;
        });

        // Register ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<WelcomePageViewModel>();
        services.AddSingleton<ManagePageViewModel>();
        services.AddSingleton<SettingsPageViewModel>();
        services.AddSingleton<CreditsPageViewModel>();

        // Register Views/Windows
        services.AddSingleton<MainWindow>();

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }
}