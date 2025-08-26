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
            Logger.Error("Couldn't find main window view model.");
            await MessageBox.ShowAsync("Couldn't find main window view model. Window won't be disabled.", "Error");
            return;
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

            // Backup executable
            Logger.Info("Backing up the game executable");
            StatusText = "Backing up the game executable";
            if (File.Exists(AppPaths.AltairGameExecutable))
            {
                File.Copy(AppPaths.AltairGameExecutable, AppPaths.AltairGameExecutable + ".bak", true);
            }

            // Applying 4GB Patch (Large Address Aware)
            StatusText = "Applying 4GB Patch (Large Address Aware)";
            UniversalPatcher.LargeAddressAwarePatch(AppPaths.AltairGameExecutable);

            // Cleanup
            Logger.Info("Cleaning up");
            StatusText = "Cleaning up";
            
            // Delete the downloads directory recursively
            if (Directory.Exists(AppPaths.Downloads))
            {
                Logger.Debug("Deleting downloads directory");
                Directory.Delete(AppPaths.Downloads, true);
            }
            Logger.Info("Setup completed");
            App.Settings.SetupCompleted = true;
            StatusText = "Download Complete.";

            mainVM.SetupCompleted = true;
            mainVM.Navigate("Default");
            await MessageBox.ShowAsync("Installation completed.", "Success");
        }
        catch (Exception ex)
        {
            await MessageBox.ShowAsync(ex.Message);
        }
    }
}