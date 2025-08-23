using System.Diagnostics;
using AnimusReforged.Altair.Views.Pages;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AnimusReforged.Altair.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Title { get; } = $"AnimusReforged (Altair) v{App.Settings.GetVersion()}";
    
    [ObservableProperty] 
    private Control? currentPage;

    public MainWindowViewModel()
    {
        if (App.Settings.SetupCompleted)
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
    public void DonateButton()
    {
        Logger.Debug("Opening donation link in browser");
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://example.com/",
            UseShellExecute = true
        });
    }
}