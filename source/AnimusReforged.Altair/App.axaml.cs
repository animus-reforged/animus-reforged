using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AnimusReforged.Altair.ViewModels;
using AnimusReforged.Altair.Views;
using AnimusReforged.Settings;
using Avalonia.Controls;

namespace AnimusReforged.Altair;

public partial class App : Application
{
    // Variables
    public static readonly AltairSettings AppSettings = new AltairSettings();
    public static AltairSettings.AltairSettingsStore Settings => AppSettings.Settings;
    public static Window? MainWindow => Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
    
    // Functions
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            StartupInitialization();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
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

    private void StartupInitialization()
    {
        // Logger initialization
        Logger.Initialize();
        Logger.Info($"Animus Reforged (Altair) v{Settings.GetVersion()} is starting up");
        
        // Unhandled exception handler
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                Logger.Error(ex, "Unhandled exception. Terminating");
                Logger.LogExceptionDetails(ex);
            }
        };
    }
}