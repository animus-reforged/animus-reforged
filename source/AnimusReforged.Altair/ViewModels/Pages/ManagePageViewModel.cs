using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using AnimusReforged.Models;
using AnimusReforged.Altair.Services.UI;
using AnimusReforged.Logging;
using AnimusReforged.Mods.Altair;
using AnimusReforged.Mods.Core;
using AnimusReforged.Models.Mods;
using AnimusReforged.Settings;
using AnimusReforged.Settings.Core;
using AnimusReforged.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.ViewModels.Pages;

public partial class ManagePageViewModel : ViewModelBase
{
    // Variables
    private MainWindowViewModel _mainWindowViewModel { get; set; }
    private AltairSettings _settings { get; set; }
    private readonly IMessageBoxService _messageBoxService;

    // Download Progress Bar
    [ObservableProperty] private bool isDownloading;
    [ObservableProperty] private string downloadStatus = "Waiting for download...";
    [ObservableProperty] private string updateCheckStatus = "No updates available";
    [ObservableProperty] private int downloadProgress;

    // Update Checking
    [ObservableProperty] private ObservableCollection<UpdatableMod> updatableMods = [];
    [ObservableProperty] private bool hasUpdatableMods;

    // Constructor
    public ManagePageViewModel()
    {
        _mainWindowViewModel = App.Services.GetRequiredService<MainWindowViewModel>();
        _settings = App.Services.GetRequiredService<AltairSettings>();
        _messageBoxService = App.Services.GetRequiredService<IMessageBoxService>();

        IsDownloading = false;
        DownloadProgress = 0;
        HasUpdatableMods = false;
    }

    [RelayCommand]
    private async Task CheckForUpdates()
    {
        try
        {
            Logger.Info<ManagePageViewModel>("Checking for mod updates");

            // Show download progress section while checking for updates
            IsDownloading = true;
            DownloadStatus = "Checking for updates...";
            DownloadProgress = 0;

            // Initialize mod manager to load manifest
            await ModManager.InitializeAsync();
            DownloadProgress = 20;

            // Get the current manifest
            ModManifest manifest = await ManifestService.GetManifestAsync(ManifestType.Altair);
            DownloadProgress = 40;

            // Clear previous updatable mods
            UpdatableMods.Clear();

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
                    UpdatableMods.Add(updatableMod);
                }
            }

            DownloadProgress = 100;
            HasUpdatableMods = UpdatableMods.Count > 0;

            DownloadStatus = !HasUpdatableMods ? "No updates available" : $"{UpdatableMods.Count} update(s) available";
            UpdateCheckStatus = !HasUpdatableMods ? "No updates available" : $"{UpdatableMods.Count} update(s) available. Click for more info";
            IsDownloading = false;
            await _messageBoxService.ShowInfoAsync("Update Check", DownloadStatus);
        }
        catch (Exception ex)
        {
            Logger.Error<ManagePageViewModel>($"Error checking for updates: {ex.Message}");
            IsDownloading = false;
            DownloadStatus = "Error checking for updates";
            UpdateCheckStatus = "Error checking for updates";
            await _messageBoxService.ShowErrorAsync("Update Check Failed", $"An error occurred while checking for updates: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task UpdateMod(UpdatableMod? updatableMod)
    {
        if (updatableMod == null)
        {
            return;
        }

        try
        {
            Logger.Info<ManagePageViewModel>($"Updating mod: {updatableMod.Name}");

            // Set the mod as updating
            updatableMod.IsUpdating = true;

            // Show download progress section
            IsDownloading = true;
            DownloadStatus = $"Updating {updatableMod.Name}...";
            DownloadProgress = 0;

            _mainWindowViewModel.DisableWindow = true;

            // Download the mod
            await ModManager.DownloadModByIdAsync(updatableMod.Id, progress => DownloadProgress = progress);

            // Extract and install the mod
            ModManager.ExtractModById(updatableMod.Id);

            // Update the installed version in settings
            ModManager.UpdateInstalledModVersion(updatableMod.Id, _settings);

            // Update the UI
            updatableMod.CurrentVersion = updatableMod.LatestVersion;
            UpdatableMods.Remove(updatableMod);

            HasUpdatableMods = UpdatableMods.Count > 0;
            DownloadStatus = $"{updatableMod.Name} updated successfully!";
            DownloadProgress = 100;

            // Hide progress section after a short delay
            await _settings.SaveSettingsAsync();
            IsDownloading = false;
            _mainWindowViewModel.DisableWindow = false;
            // Show success message
            await _messageBoxService.ShowInfoAsync("Update Successful", $"{updatableMod.Name} has been updated to version {updatableMod.LatestVersion}!");
        }
        catch (Exception ex)
        {
            Logger.Error<ManagePageViewModel>($"Error updating mod {updatableMod.Name}: {ex.Message}");
            IsDownloading = false;
            DownloadStatus = $"Error updating {updatableMod.Name}";
            _mainWindowViewModel.DisableWindow = false;
            await _messageBoxService.ShowErrorAsync("Update Failed", $"An error occurred while updating {updatableMod.Name}: {ex.Message}");
        }
        finally
        {
            updatableMod.IsUpdating = false;
        }
    }

    [RelayCommand]
    private async Task FixUModPaths()
    {
        Logger.Info<ManagePageViewModel>("Redoing uMod paths");
        try
        {
            await UModManager.SetupAppdata(FilePaths.AltairExecutable);
        }
        catch (ArgumentException)
        {
            Logger.Error<ManagePageViewModel>($"Invalid game executable path");
            await _messageBoxService.ShowErrorAsync("Failure", "Invalid game executable path");
        }
        try
        {
            await UModManager.SetupSaveFile(FilePaths.AltairExecutable, "ac1.txt");
        }
        catch (ArgumentException)
        {
            Logger.Error<ManagePageViewModel>($"Invalid game executable path");
            await _messageBoxService.ShowErrorAsync("Failure", "Invalid game executable path");
        }
        await _messageBoxService.ShowInfoAsync("Success", "uMod paths have been recreated.");
    }

    [RelayCommand]
    private async Task Uninstall()
    {
        // Confirmation
        bool uninstallConfirmation = await _messageBoxService.ShowConfirmationAsync("Confirmation", "Do you want to remove AnimusReforged?\nThis will remove everything and reset AnimusReforged to first time setup.");
        if (!uninstallConfirmation)
        {
            Logger.Info<ManagePageViewModel>("Uninstallation cancelled");
            return;
        }
        
        // Uninstall mods
        Logger.Info<ManagePageViewModel>("Uninstalling mods");
        ModManager.UninstallAsiLoader();

        // Uninstall uMod
        bool deleteuModConfig = await _messageBoxService.ShowConfirmationAsync("Confirmation", "Do you want to remove uMod configuration folder?");
        await ModManager.UninstalluMod(deleteuModConfig);

        // Restore original executable
        if (File.Exists(FilePaths.AltairExecutable + ".bak"))
        {
            Logger.Info<ManagePageViewModel>("Restoring the original game executable without LAA patch applied");
            File.Copy(FilePaths.AltairExecutable + ".bak", FilePaths.AltairExecutable, true);
            File.Delete(FilePaths.AltairExecutable + ".bak");
        }
        else
        {
            Logger.Warning<ManagePageViewModel>("Couldn't find backup of the original game executable");
        }

        // Resetting config file
        _settings.Settings.Setup = new SetupSettings();
        _settings.Settings.InstalledModVersions = new Dictionary<string, string>();
        await _settings.SaveSettingsAsync();
        await _messageBoxService.ShowInfoAsync("Success", "AnimusReforged has been successfully uninstalled!\nNow you can remove the AnimusReforged executable.");
        
        // Close the launcher
        Environment.Exit(0);
    }
}