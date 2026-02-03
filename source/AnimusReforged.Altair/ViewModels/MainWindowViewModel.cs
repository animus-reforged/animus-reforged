using AnimusReforged.Altair.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private bool disableWindow;
    [ObservableProperty] private bool hasUpdateNotifications;
    [ObservableProperty] private string updateNotificationMessage;

    private readonly IUpdateNotificationService _updateNotificationService;

    public MainWindowViewModel()
    {
        disableWindow = false;
        hasUpdateNotifications = false;
        updateNotificationMessage = string.Empty;

        // Get the update notification service
        _updateNotificationService = App.Services.GetRequiredService<IUpdateNotificationService>();

        // Subscribe to update notifications
        _updateNotificationService.UpdatesChanged += OnUpdatesChanged;
    }

    private void OnUpdatesChanged(object? sender, System.EventArgs e)
    {
        bool hasUpdates = _updateNotificationService.HasUpdates;
        int updateCount = _updateNotificationService.UpdateCount;

        HasUpdateNotifications = hasUpdates;
        UpdateNotificationMessage = hasUpdates
            ? $"Update available: {updateCount} mod(s) can be updated. Go to Manage page to update."
            : string.Empty;
    }

    public void ClearUpdateNotifications()
    {
        HasUpdateNotifications = false;
        UpdateNotificationMessage = string.Empty;
        _updateNotificationService.ClearNotifications();
    }
}