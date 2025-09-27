using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using AnimusReforged.Altair.Views;
using AnimusReforged.Mods.Altair;
using AnimusReforged.Mods.Utilities;
using AnimusReforged.Paths;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AnimusReforged.Altair.ViewModels.Pages;

public partial class ManagePageViewModel : ViewModelBase
{
    // Variables
    // Download Progress Bar
    [ObservableProperty] private bool isDownloading;
    [ObservableProperty] private string downloadStatus = "Waiting for download...";
    [ObservableProperty] private int downloadProgress;

    // Mods
    [ObservableProperty] private int selectedModIndex;
    [ObservableProperty] private string selectedMod;
    [ObservableProperty] private ObservableCollection<string> installedMods = ["Ultimate ASI Loader", "AltairFix", "EaglePatch", "uMod", "Overhaul", "ReShade"];

    // Launcher Updater
    [ObservableProperty] private bool launcherUpdateAvailable;

    // Constructor
    public ManagePageViewModel()
    {
        IsDownloading = false;
        DownloadProgress = 0;
        SelectedModIndex = 0;
        SelectedMod = InstalledMods[SelectedModIndex];
        LauncherUpdateAvailable = false;
    }

    // Methods
    [RelayCommand]
    private async Task Download()
    {
        IsDownloading = true;
        MainWindowViewModel? mainVM = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow?.DataContext as MainWindowViewModel;
        if (mainVM == null)
        {
            Logger.Error("Couldn't find main window view model.");
            await MessageBox.ShowAsync("Couldn't find main window view model. Window won't be disabled.", MessageBoxButtons.Ok);
            return;
        }
        mainVM.DisableWindow = false;
        Logger.Info($"Selected Mod: {SelectedMod}");
        try
        {
            switch (SelectedMod)
            {
                case "Ultimate ASI Loader":
                    Logger.Info("Downloading Ultimate ASI Loader");
                    DownloadStatus = "Downloading Ultimate ASI Loader";
                    await ModManager.DownloadAsiLoader(progress => DownloadProgress = progress);
                    Logger.Info("Installing Ultimate ASI Loader");
                    DownloadStatus = "Installing Ultimate ASI Loader";
                    ModManager.InstallAsiLoader();
                    break;
                case "AltairFix":
                    Logger.Info("Downloading AltairFix");
                    DownloadStatus = "Downloading AltairFix";
                    await ModManager.DownloadAltairFix(progress => DownloadProgress = progress);
                    Logger.Info("Installing AltairFix");
                    DownloadStatus = "Installing AltairFix";
                    ModManager.InstallAltairFix();
                    break;
                case "EaglePatch":
                    Logger.Info("Downloading EaglePatch mod");
                    DownloadStatus = "Downloading EaglePatch mod";
                    await ModManager.DownloadEaglePatch(progress => DownloadProgress = progress);
                    Logger.Info("Installing EaglePatch mod");
                    DownloadStatus = "Installing EaglePatch mod";
                    ModManager.InstallEaglePatch();
                    break;
                case "uMod":
                    Logger.Info("Downloading uMod");
                    DownloadStatus = "Downloading uMod";
                    await ModManager.DownloaduMod(progress => DownloadProgress = progress);
                    Logger.Info("Installing uMod");
                    DownloadStatus = "Installing uMod";
                    ModManager.InstalluMod();
                    break;
                case "Overhaul":
                    Logger.Info("Downloading Overhaul mod");
                    DownloadStatus = "Downloading Overhaul mod";
                    await ModManager.DownloadOverhaul(progress => DownloadProgress = progress);
                    Logger.Info("Installing Overhaul mod");
                    DownloadStatus = "Installing Overhaul mod";
                    ModManager.InstallOverhaul();
                    break;
                case "ReShade":
                    Logger.Info("Downloading ReShade");
                    DownloadStatus = "Downloading ReShade";
                    await ModManager.DownloadReShade(progress => DownloadProgress = progress);
                    Logger.Info("Installing ReShade");
                    DownloadStatus = "Installing Reshade";
                    ModManager.InstallReShade();
                    App.Settings.Tweaks.Reshade = true;
                    App.AppSettings.SaveSettings();
                    break;
            }
            if (Directory.Exists(AppPaths.Downloads))
            {
                Logger.Info("Deleting downloads");
                Directory.Delete(AppPaths.Downloads, true);
            }
            Logger.Info("Downloading complete.");
            await MessageBox.ShowAsync("Download Complete.", MessageBoxButtons.Ok);
        }
        catch (Exception ex)
        {
            Logger.Error($"Error during download of {SelectedMod}");
            Logger.LogExceptionDetails(ex);
            await MessageBox.ShowAsync(ex.Message, MessageBoxButtons.Ok);
        }
        mainVM.DisableWindow = true;
        IsDownloading = false;
    }

    [RelayCommand]
    private async Task FixUModPaths()
    {
        Logger.Info("Redoing uMod paths");
        await UModManager.SetupAppdata(AppPaths.AltairGameExecutable);
        await UModManager.SetupSaveFile(AppPaths.AltairGameExecutable, "ac1.txt");
    }

    [RelayCommand]
    private async Task Uninstall()
    {
        // Uninstall mods
        Logger.Info("Uninstalling mods");
        ModManager.UninstallAsiLoader();
        
        // Ask the user if he wants to delete uMod configuration folder
        bool deleteUModConfig = await MessageBox.ShowAsync("Do you want to remove uMod configuration folder?", MessageBoxButtons.YesNo, "Uninstall uMod configuration folder?");
        ModManager.UninstalluMod(deleteUModConfig);
        
        // Restore the original game executable if the backup exists
        if (File.Exists(AppPaths.AltairGameExecutable + ".bak"))
        {
            Logger.Info("Restoring the original game executable without LAA patch applied");
            File.Copy(AppPaths.AltairGameExecutable + ".bak", AppPaths.AltairGameExecutable, true);
            File.Delete(AppPaths.AltairGameExecutable + ".bak");
        }
        else
        {
            Logger.Warning("Couldn't find backup of the original game executable");
        }
        App.Settings.SetupCompleted = false;
        App.AppSettings.SaveSettings();
        await MessageBox.ShowAsync("AnimusReforged has been successfully uninstalled.\nNow you can remove the AnimusReforged executable.", MessageBoxButtons.Ok, "Successful uninstallation");
        
        // Close the launcher
        Environment.Exit(0);
    }

    // TODO: Check for Launcher Updates
    // TODO: Update Launcher
    [RelayCommand]
    private void CheckForLauncherUpdates()
    {
        LauncherUpdateAvailable = !LauncherUpdateAvailable;
    }
}