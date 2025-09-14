using System;
using System.Collections.ObjectModel;
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
    [ObservableProperty] private string downloadStatus = string.Empty;
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
                    DownloadStatus = "Downloading Ultimate ASI Loader";
                    await ModManager.DownloadAsiLoader(progress => DownloadProgress = progress);
                    DownloadStatus = "Installing Ultimate ASI Loader";
                    ModManager.InstallAsiLoader();
                    break;
                case "AltairFix":
                    DownloadStatus = "Downloading AltairFix";
                    await ModManager.DownloadAltairFix(progress => DownloadProgress = progress);
                    DownloadStatus = "Installing AltairFix";
                    ModManager.InstallAltairFix();
                    break;
                case "EaglePatch":
                    DownloadStatus = "Downloading EaglePatch mod";
                    await ModManager.DownloadEaglePatch(progress => DownloadProgress = progress);
                    DownloadStatus = "Installing EaglePatch mod";
                    ModManager.InstallEaglePatch();
                    break;
                case "uMod":
                    DownloadStatus = "Downloading uMod";
                    await ModManager.DownloaduMod(progress => DownloadProgress = progress);
                    DownloadStatus = "Installing uMod";
                    ModManager.InstalluMod();
                    break;
                case "Overhaul":
                    DownloadStatus = "Downloading Overhaul mod";
                    await ModManager.DownloadOverhaul(progress => DownloadProgress = progress);
                    DownloadStatus = "Installing Overhaul mod";
                    ModManager.InstallOverhaul();
                    break;
                case "ReShade":
                    DownloadStatus = "Downloading ReShade";
                    await ModManager.DownloadReShade(progress => DownloadProgress = progress);
                    DownloadStatus = "Installing Reshade";
                    ModManager.InstallReShade();
                    App.Settings.Tweaks.Reshade = true;
                    App.AppSettings.SaveSettings();
                    break;
            }
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

    // TODO: Check for Launcher Updates
    // TODO: Update Launcher
    [RelayCommand]
    private void CheckForLauncherUpdates()
    {
        LauncherUpdateAvailable = !LauncherUpdateAvailable;
    }
}