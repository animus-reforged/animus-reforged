using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AnimusReforged.Altair.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Title { get; } = $"AnimusReforged (Altair) v{App.Settings.GetVersion()}";
    public event EventHandler<string>? NavigationRequested;

    [ObservableProperty] private bool working;

    [ObservableProperty] private bool disableWindow;

    public MainWindowViewModel()
    {
        DisableWindow = App.Settings.SetupCompleted;
        Working = false;
    }

    [RelayCommand]
    public void Navigate(string destination)
    {
        Logger.Debug($"Navigating to {destination} page");
        NavigationRequested?.Invoke(this, destination);
    }
}