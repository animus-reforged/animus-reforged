using System;
using System.Linq;
using AnimusReforged.Altair.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AnimusReforged.Altair.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair;

public partial class App : Application
{
    // Properties
    /// <summary>
    /// Desktop instance
    /// </summary>
    public static IClassicDesktopStyleApplicationLifetime? Desktop = Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
    
    /// <summary>
    /// Main Window instance
    /// </summary>
    public static Window? MainWindow => Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
    
    /// <summary>
    /// DI Services
    /// </summary>
    public static IServiceProvider Services { get; private set; } = null!;
    
    // Functions
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (Desktop is { } desktop)
        {
            // Disable Avalonia validation
            DisableAvaloniaDataAnnotationValidation();
            
            // Configure services
            try
            {
                Services = ServiceConfigurator.ConfigureServices();
            }
            catch (Exception ex)
            {
                throw;
            }
            
            MainWindow mainWindow = Services.GetRequiredService<MainWindow>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        DataAnnotationsValidationPlugin[] dataValidationPluginsToRemove = BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (DataAnnotationsValidationPlugin plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}