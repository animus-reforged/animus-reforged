using System.Threading.Tasks;
using AnimusReforged.Altair.Services.UI;
using AnimusReforged.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.ViewModels.Pages;

public partial class WelcomePageViewModel : ViewModelBase
{
    // Variables
    private MainWindowViewModel _mainWindowViewModel  { get; set; }
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
        // TODO: This is currently just an emulated installation
        _mainWindowViewModel.DisableWindow = true;
        for (int i = 0; i < 101; i++)
        {
            await Task.Delay(100);
            ProgressBarValue = i;
        }
        _mainWindowViewModel.DisableWindow = false;
        _settings.Settings.SetupCompleted = true;
        await _settings.SaveSettingsAsync();
        await _messageBoxService.ShowInfoAsync("Installation Completed!", "AnimusReforged has been successfully installed!");
    }
}