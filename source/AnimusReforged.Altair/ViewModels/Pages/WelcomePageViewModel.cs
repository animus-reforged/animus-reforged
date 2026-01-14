using System;
using System.IO;
using System.Threading.Tasks;
using AnimusReforged.Altair.Services.UI;
using AnimusReforged.Logging;
using AnimusReforged.Models.Mods;
using AnimusReforged.Mods.Altair;
using AnimusReforged.Mods.Core;
using AnimusReforged.Settings;
using AnimusReforged.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.ViewModels.Pages;

public partial class WelcomePageViewModel : ViewModelBase
{
    // Variables
    private MainWindowViewModel _mainWindowViewModel { get; set; }
    private AltairSettings _settings { get; set; }
    private readonly IMessageBoxService _messageBoxService;

    [ObservableProperty] private string statusText = string.Empty;

    [ObservableProperty] private int progressBarValue;

    public WelcomePageViewModel()
    {
        _mainWindowViewModel = App.Services.GetRequiredService<MainWindowViewModel>();
        _settings = App.Services.GetRequiredService<AltairSettings>();
        _messageBoxService = App.Services.GetRequiredService<IMessageBoxService>();
    }

    // Methods
    [RelayCommand]
    private async Task Install()
    {
        Logger.Info<WelcomePageViewModel>("Starting installation process");

        try
        {
            _mainWindowViewModel.DisableWindow = true;
            StatusText = "Downloading Mods List";
            Logger.Info<WelcomePageViewModel>("Initializing mod manager and downloading mods list");
            await ModManager.InitializeAsync();
            Logger.Info<WelcomePageViewModel>("Successfully downloaded mods list");

            // Install ASI Loader
            StatusText = "Downloading ASI Loader";
            Logger.Info<WelcomePageViewModel>("Starting ASI Loader download");
            await ModManager.DownloadAsiLoader(progress => ProgressBarValue = progress);
            Logger.Info<WelcomePageViewModel>("ASI Loader download completed");

            StatusText = "Installing ASI Loader";
            Logger.Info<WelcomePageViewModel>("Installing ASI Loader");
            ModManager.InstallAsiLoader();
            ModManager.UpdateInstalledModVersion(ModIdentifiers.AsiLoader, _settings);
            Logger.Info<WelcomePageViewModel>("ASI Loader installation completed");

            // Install EaglePatch
            StatusText = "Downloading EaglePatch mod";
            Logger.Info<WelcomePageViewModel>("Starting EaglePatch download");
            await ModManager.DownloadEaglePatch(progress => ProgressBarValue = progress);
            Logger.Info<WelcomePageViewModel>("EaglePatch download completed");

            StatusText = "Installing EaglePatch mod";
            Logger.Info<WelcomePageViewModel>("Installing EaglePatch mod");
            ModManager.InstallEaglePatch();
            ModManager.UpdateInstalledModVersion(ModIdentifiers.EaglePatch, _settings);
            Logger.Info<WelcomePageViewModel>("EaglePatch installation completed");

            // Install AltairFix
            StatusText = "Downloading AltairFix";
            Logger.Info<WelcomePageViewModel>("Starting AltairFix download");
            await ModManager.DownloadAltairFix(progress => ProgressBarValue = progress);
            Logger.Info<WelcomePageViewModel>("AltairFix download completed");

            StatusText = "Installing AltairFix";
            Logger.Info<WelcomePageViewModel>("Installing AltairFix");
            ModManager.InstallAltairFix();
            ModManager.UpdateInstalledModVersion(ModIdentifiers.AltairFix, _settings);
            Logger.Info<WelcomePageViewModel>("AltairFix installation completed");

            // Install ReShade
            StatusText = "Downloading ReShade";
            Logger.Info<WelcomePageViewModel>("Starting ReShade download");
            await ModManager.DownloadReShade(progress => ProgressBarValue = progress);
            Logger.Info<WelcomePageViewModel>("ReShade download completed");

            StatusText = "Installing Reshade";
            Logger.Info<WelcomePageViewModel>("Installing ReShade");
            ModManager.InstallReShade();
            _settings.Settings.Tweaks.Reshade = true;
            ModManager.UpdateInstalledModVersion(ModIdentifiers.ReShade, _settings);
            Logger.Info<WelcomePageViewModel>("ReShade installation completed");

            // Install uMod
            StatusText = "Downloading uMod";
            Logger.Info<WelcomePageViewModel>("Starting uMod download");
            await ModManager.DownloadUMod(progress => ProgressBarValue = progress);
            Logger.Info<WelcomePageViewModel>("uMod download completed");

            StatusText = "Installing uMod";
            Logger.Info<WelcomePageViewModel>("Installing uMod");
            ModManager.InstallUMod();
            ModManager.UpdateInstalledModVersion(ModIdentifiers.UMod, _settings);
            Logger.Info<WelcomePageViewModel>("uMod installation completed");

            // Install Overhaul
            StatusText = "Downloading Overhaul mod";
            Logger.Info<WelcomePageViewModel>("Starting Overhaul mod download");
            await ModManager.DownloadOverhaul(progress => ProgressBarValue = progress);
            Logger.Info<WelcomePageViewModel>("Overhaul mod download completed");

            StatusText = "Installing Overhaul mod";
            Logger.Info<WelcomePageViewModel>("Installing Overhaul mod");
            ModManager.InstallOverhaul();
            ModManager.UpdateInstalledModVersion(ModIdentifiers.Overhaul, _settings);
            Logger.Info<WelcomePageViewModel>("Overhaul mod installation completed");

            // Setup uMod and Overhaul
            StatusText = "Setting up uMod and Overhaul mod";
            Logger.Info<WelcomePageViewModel>("Setting up uMod and Overhaul mod");
            await ModManager.SetupUMod();
            Logger.Info<WelcomePageViewModel>("uMod and Overhaul setup completed");

            // Applying 4GB Patch (Large Address Aware)
            StatusText = "Applying 4GB Patch (Large Address Aware)";
            Logger.Info<WelcomePageViewModel>("Applying 4GB Patch (Large Address Aware) to AssassinsCreed_Dx9.exe");
            Patcher.LargeAddressAwarePatch(FilePaths.AltairExecutable);
            Logger.Info<WelcomePageViewModel>("4GB Patch applied successfully");

            // Delete the downloads directory recursively
            if (Directory.Exists(FilePaths.DownloadsDirectory))
            {
                Logger.Info<WelcomePageViewModel>("Deleting downloads directory");
                Directory.Delete(FilePaths.DownloadsDirectory, true);
                Logger.Info<WelcomePageViewModel>("Downloads directory deleted successfully");
            }
            else
            {
                Logger.Info<WelcomePageViewModel>("Downloads directory does not exist, skipping deletion");
            }

            _settings.Settings.SetupCompleted = true;
            await _settings.SaveSettingsAsync();
            Logger.Info<WelcomePageViewModel>("Settings saved and setup marked as completed");

            _mainWindowViewModel.DisableWindow = false;
            Logger.Info<WelcomePageViewModel>("Installation completed successfully, showing success message");
            await _messageBoxService.ShowInfoAsync("Installation Completed!", "AnimusReforged has been successfully installed!");
        }
        catch (Exception ex)
        {
            Logger.Error<WelcomePageViewModel>($"Installation failed with error: {ex.Message}");
            Logger.Error<WelcomePageViewModel>($"Stack trace: {ex.StackTrace}");
            _mainWindowViewModel.DisableWindow = false;
            await _messageBoxService.ShowErrorAsync("Installation Failed", $"An error occurred during installation: {ex.Message}\n\nPlease check the logs for more details.");
        }

        Logger.Info<WelcomePageViewModel>("Finished installation process");
    }
}