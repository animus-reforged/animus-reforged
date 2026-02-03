using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AnimusReforged.Altair.Services;
using AnimusReforged.Models;
using AnimusReforged.Mods.Altair;
using AnimusReforged.Mods.Core;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using AnimusReforged.Altair.ViewModels;
using AnimusReforged.Logging;
using AnimusReforged.Settings;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using AnimusReforged.Models.Mods;

namespace AnimusReforged.Altair.Views;

public partial class MainView : UserControl
{
    // Properties
    private MainViewModel _viewModel { get; set; }
    private AltairSettings _settings { get; set; }
    private readonly NavigationService _navigationService;
    private readonly IUpdateNotificationService _updateNotificationService;
    private readonly MainWindowViewModel _mainWindowViewModel;

    // Constructor
    public MainView()
    {
        InitializeComponent();
        _viewModel = App.Services.GetRequiredService<MainViewModel>();
        _updateNotificationService = App.Services.GetRequiredService<IUpdateNotificationService>();
        _mainWindowViewModel = App.Services.GetRequiredService<MainWindowViewModel>();
        DataContext = _viewModel;
        Loaded += async (_, _) =>
        {
            await _viewModel.CheckInstallation();
        };

        // Set up navigation service
        _navigationService = App.Services.GetRequiredService<NavigationService>();
        _navigationService.SetContentFrame(ContentFrame);
        _navigationService.SetNavigationView(NavView);

        _settings = App.Services.GetRequiredService<AltairSettings>();
        _viewModel.FirstSetup = _settings.Settings.Setup.Completed;
        _settings.SettingsChanged += (_, _) =>
        {
            if (!_viewModel.FirstSetup)
            {
                _viewModel.FirstSetup = _settings.Settings.Setup.Completed;
                string targetPage = _settings.Settings.Setup.Completed ? "Default" : "Welcome";
                Dispatcher.UIThread.Post(() =>
                {
                    _navigationService.NavigateToTag(targetPage).Wait();
                });
            }
        };

        string initialPage = _settings.Settings.Setup.Completed ? "Default" : "Welcome";
        _navigationService.NavigateToTag(initialPage).Wait();

        Loaded += async (_, _) =>
        {
            if (_settings.Settings.Setup.Completed)
            {
                await _viewModel.CheckInstallation();

                if (DateTime.Now - _settings.Settings.LastUpdateCheckDate > TimeSpan.FromDays(1))
                {
                    // Check for updates on startup
                    await CheckForUpdatesOnStartup();
                }
            }
        };
    }

    private async Task CheckForUpdatesOnStartup()
    {
        try
        {
            Logger.Info<MainView>("Checking for mod updates on startup");

            // Initialize mod manager to load manifest
            await ModManager.InitializeAsync();

            // Get the current manifest
            ModManifest manifest = await ManifestService.GetManifestAsync(ManifestType.Altair);

            // Create a collection to store updatable mods
            ObservableCollection<UpdatableMod> updatableMods = new ObservableCollection<UpdatableMod>();

            // Check each mod for updates
            foreach (KeyValuePair<string, ModDefinition> modEntry in manifest.Mods)
            {
                string modId = modEntry.Key;
                ModDefinition mod = modEntry.Value;

                // Get the currently installed version
                string? installedVersion = _settings.GetInstalledModVersion(modId);

                // If the mod is installed and the version differs, it can be updated
                if (installedVersion != null && installedVersion != mod.Version)
                {
                    UpdatableMod updatableMod = new UpdatableMod(modId, mod.Name, installedVersion, mod.Version);
                    updatableMods.Add(updatableMod);
                }
            }

            // Notify the global update service about available updates
            _updateNotificationService.NotifyUpdates(updatableMods);
            
            // Update the last update check date
            _settings.Settings.LastUpdateCheckDate = DateTime.Now;
            await _settings.SaveSettingsAsync();
        }
        catch (Exception ex)
        {
            Logger.Error<MainView>($"Error checking for updates on startup: {ex.Message}");
        }
    }

    // Functions
    private async void NavView_OnItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        try
        {
            if (e.InvokedItemContainer is NavigationViewItem selectedItem)
            {
                await _navigationService.Navigate(selectedItem, ContentFrame);
            }
        }
        catch (Exception ex)
        {
            Logger.Error<MainView>("Failed to navigate to page");
            Logger.LogExceptionDetails<MainView>(ex);
        }
    }
}