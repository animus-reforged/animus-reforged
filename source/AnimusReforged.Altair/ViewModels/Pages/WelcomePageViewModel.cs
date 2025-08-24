using System;
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

public partial class WelcomePageViewModel : ViewModelBase
{
    // Variables
    [ObservableProperty] private string statusText = string.Empty;

    [ObservableProperty] private int progressBarValue;

    // Methods
    [RelayCommand]
    private async Task Install()
    {
        Logger.Debug("Installing AnimusReforged (Altair) mods");
        MainWindowViewModel? mainVM = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow?.DataContext as MainWindowViewModel;
        if (mainVM == null)
        {
            Logger.Error("Couldn't find main window view model. Window won't be disabled.");
        }
        try
        {
            // Install ASI Loader (https://github.com/ThirteenAG/Ultimate-ASI-Loader/releases/latest/download/Ultimate-ASI-Loader.zip)
            StatusText = "Downloading Ultimate ASI Loader";
            await ModManager.DownloadAsiLoader(progress => ProgressBarValue = progress);
            StatusText = "Installing Ultimate ASI Loader";
            ModManager.InstallAsiLoader();

            // Install EaglePatch (https://github.com/Sergeanur/EaglePatch/releases/latest/download/EaglePatchAC1.rar)
            StatusText = "Downloading EaglePatch mod";
            await ModManager.DownloadEaglePatch(progress => ProgressBarValue = progress);
            StatusText = "Installing EaglePatch mod";
            ModManager.InstallEaglePatch();

            // Install uMod (https://github.com/animus-reforged/uMod/releases/latest/download/uMod.zip)
            StatusText = "Downloading uMod";
            await ModManager.DownloaduMod(progress => ProgressBarValue = progress);
            StatusText = "Installing uMod";
            ModManager.InstalluMod();

            // Install Overhaul (https://github.com/animus-reforged/mods/releases/download/altair/Overhaul.zip)
            StatusText = "Downloading Overhaul mod";
            await ModManager.DownloadOverhaul(progress => ProgressBarValue = progress);
            StatusText = "Installing Overhaul mod";
            ModManager.InstallOverhaul();

            // Setup uMod and Overhaul
            StatusText = "Setting up uMod and Overhaul mod";
            await UModManager.SetupAppdata(AppPaths.AltairGameExecutable);
            await UModManager.SetupSaveFile(AppPaths.AltairGameExecutable, "ac1.txt");

            // Cleanup
            StatusText = "Cleaning up";
            Logger.Info("Cleaning up");
            // Delete the downloads directory recursively
            if (Directory.Exists(AppPaths.Downloads))
            {
                Logger.Debug("Deleting downloads directory");
                Directory.Delete(AppPaths.Downloads, true);
            }
            Logger.Info("Setup completed");
            App.Settings.SetupCompleted = true;
            StatusText = "Download Complete.";
            await MessageBox.ShowAsync("Installation completed.", "Success");

            // Navigate to a different page
            if (mainVM != null)
            {
                Logger.Debug("Navigating to default page");
                mainVM.SetupCompleted = true;
                mainVM.Navigate("Default");
            }
        }
        catch (Exception ex)
        {
            await MessageBox.ShowAsync(ex.Message);
        }
    }
}