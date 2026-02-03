using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AnimusReforged.Models.Mods;

namespace AnimusReforged.Altair.Services;

/// <summary>
/// Interface for the update notification service that handles mod update notifications
/// across the application.
/// </summary>
public interface IUpdateNotificationService
{
    /// <summary>
    /// Gets whether there are any updates available
    /// </summary>
    bool HasUpdates { get; }

    /// <summary>
    /// Gets the count of available updates
    /// </summary>
    int UpdateCount { get; }

    /// <summary>
    /// Gets the collection of updatable mods
    /// </summary>
    ObservableCollection<UpdatableMod> UpdatableMods { get; }

    /// <summary>
    /// Event raised when updates change
    /// </summary>
    event EventHandler UpdatesChanged;

    /// <summary>
    /// Notifies the service of available updates
    /// </summary>
    /// <param name="updatableMods">Collection of updatable mods</param>
    void NotifyUpdates(ObservableCollection<UpdatableMod> updatableMods);

    /// <summary>
    /// Clears all update notifications
    /// </summary>
    void ClearNotifications();
}

/// <summary>
/// Provides centralized update notification functionality for the application,
/// managing mod update notifications and broadcasting them to interested parties.
/// </summary>
public class UpdateNotificationService : IUpdateNotificationService, INotifyPropertyChanged
{
    private bool _hasUpdates;

    /// <summary>
    /// Gets whether there are any updates available
    /// </summary>
    public bool HasUpdates
    {
        get => _hasUpdates;
        private set
        {
            if (_hasUpdates != value)
            {
                _hasUpdates = value;
                OnPropertyChanged();
            }
        }
    }

    private int _updateCount;

    /// <summary>
    /// Gets the count of available updates
    /// </summary>
    public int UpdateCount
    {
        get => _updateCount;
        private set
        {
            if (_updateCount != value)
            {
                _updateCount = value;
                OnPropertyChanged();
            }
        }
    }

    private ObservableCollection<UpdatableMod> _updatableMods = new ObservableCollection<UpdatableMod>();

    /// <summary>
    /// Gets the collection of updatable mods
    /// </summary>
    public ObservableCollection<UpdatableMod> UpdatableMods
    {
        get => _updatableMods;
        private set
        {
            _updatableMods = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Event raised when updates change
    /// </summary>
    public event EventHandler? UpdatesChanged;

    /// <summary>
    /// Event raised when a property value changes
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies the service of available updates
    /// </summary>
    /// <param name="updatableMods">Collection of updatable mods</param>
    public void NotifyUpdates(ObservableCollection<UpdatableMod> updatableMods)
    {
        UpdatableMods = updatableMods ?? [];
        UpdateCount = UpdatableMods.Count;
        HasUpdates = UpdateCount > 0;

        UpdatesChanged?.Invoke(this, EventArgs.Empty);
        OnPropertyChanged(nameof(HasUpdates));
        OnPropertyChanged(nameof(UpdateCount));
        OnPropertyChanged(nameof(UpdatableMods));
    }

    /// <summary>
    /// Clears all update notifications
    /// </summary>
    public void ClearNotifications()
    {
        UpdatableMods = new ObservableCollection<UpdatableMod>();
        UpdateCount = 0;
        HasUpdates = false;

        UpdatesChanged?.Invoke(this, EventArgs.Empty);
        OnPropertyChanged(nameof(HasUpdates));
        OnPropertyChanged(nameof(UpdateCount));
        OnPropertyChanged(nameof(UpdatableMods));
    }

    /// <summary>
    /// Raises the PropertyChanged event
    /// </summary>
    /// <param name="propertyName">Name of the property that changed</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}