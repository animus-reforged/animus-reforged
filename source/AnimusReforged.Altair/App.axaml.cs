using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AnimusReforged.Altair.ViewModels;
using AnimusReforged.Altair.Views;
using AnimusReforged.Settings;
using AnimusReforged.Utilities;
using Avalonia.Controls;

namespace AnimusReforged.Altair;

public class App : Application
{
    // Variables
    public static readonly AltairSettings AppSettings = new AltairSettings();
    public static AltairSettings.AltairSettingsStore Settings => AppSettings.Settings;
    public static Window? MainWindow => Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : null;
    public static IClassicDesktopStyleApplicationLifetime? Desktop;
    
    // Functions
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Desktop = desktop;
            DisableAvaloniaDataAnnotationValidation();
            StartupInitialization(ArgumentParser.Contains(Desktop.Args, "-console"));
            Desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
            ArgumentChecker();
            Desktop.Exit += OnExit;
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

    private void StartupInitialization(bool showConsole)
    {
        // Logger initialization
        Logger.Initialize(showConsole);
        Logger.Info($"Animus Reforged (Altair) v{Settings.GetVersion()} is starting up");
        
        // Unhandled exception handler
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                Logger.Error(ex, "Unhandled exception. Terminating");
                Logger.LogExceptionDetails(ex);
            }
        };
    }

    private void ArgumentChecker()
    {
        if (ArgumentParser.Contains(Desktop?.Args, "-skiplauncher"))
        {
            Desktop?.MainWindow?.Hide();
            Logger.Debug("Launching the game without UI");
            Launcher.Altair.Launch(Settings.Tweaks.UMod);
            Logger.Debug("Closing the application");
            Environment.Exit(0);
        }
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        AppSettings.SaveSettings();
    }
}