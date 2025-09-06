using System.Diagnostics;
using AnimusReforged.Altair.Views.Pages;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AnimusReforged.Altair.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Title { get; } = $"AnimusReforged (Altair) v{App.Settings.GetVersion()}";

    [ObservableProperty] private Control? currentPage;

    [ObservableProperty] private bool working;

    [ObservableProperty] private bool setupCompleted;

    public MainWindowViewModel()
    {
        SetupCompleted = App.Settings.SetupCompleted;
        Working = false;
        if (SetupCompleted)
        {
            CurrentPage = new DefaultPage();
        }
        else
        {
            CurrentPage = new WelcomePage();
        }
    }
    
    [RelayCommand]
    public void Navigate(string destination)
    {
        CurrentPage = destination switch
        {
            "Credits" => new CreditsPage(),
            "Settings" => new SettingsPage(),
            "Welcome" => new WelcomePage(),
            _ => new DefaultPage()
        };
        Logger.Debug($"Navigating to {destination} page");
    }

    [RelayCommand]
    private void NavigateInvoked(string contentTag)
    {
        switch (contentTag)
        {
            case "Donate":
                Logger.Debug("Donate button clicked");
                DonateButton();
                break;
            case "Play":
                // TODO: Launch game and uMod (if enabled)
                break;
            default:
                Navigate(contentTag);
                break;
        }
    }

    [RelayCommand]
    private void DonateButton()
    {
        Logger.Debug("Opening donation link in browser");
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://example.com/",
            UseShellExecute = true
        });
    }
}