using System.Diagnostics;
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
        //CurrentPage = new DefaultView();
    }

    [RelayCommand]
    private void Navigate(string destination)
    {
        /*
        CurrentPage = destination switch
        {
            "Credits" => new CreditsView(),
            "Settings" => new SettingsView(),
            _ => new DefaultView()
        };*/
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