using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AnimusReforged.Altair.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    // Variables
    [ObservableProperty] private bool isUModEnabled = true;

    [ObservableProperty] private bool isReShadeEnabled = true;

    [ObservableProperty] private bool isEaglePatchEnabled = true;

    // Keyboard Layouts
    [ObservableProperty] private string selectedKeyboardLayout = "Keyboard"; // Default value
    public ObservableCollection<string> KeyboardLayouts { get; } = ["KeyboardMouse2", "KeyboardMouse5", "Keyboard", "KeyboardAlt"];

    [ObservableProperty] private bool isPs3ControlsEnabled = false;

    [ObservableProperty] private bool isSkipIntroVideosEnabled = true;

    [ObservableProperty] private bool isWindowedModePatchEnabled = false;

    [ObservableProperty] private bool isBorderlessFullscreenEnabled = false;

    // Constructor
    public SettingsPageViewModel()
    {
        // TODO: Loading settings from files
    }

    // Methods
    // TODO: Saving settings
    partial void OnIsUModEnabledChanged(bool oldValue, bool newValue)
    {
        Logger.Debug($"UMod: {oldValue} -> {newValue}");
    }

    partial void OnIsReShadeEnabledChanged(bool oldValue, bool newValue)
    {
        Logger.Debug($"ReShade: {oldValue} -> {newValue}");
    }

    partial void OnIsEaglePatchEnabledChanged(bool oldValue, bool newValue)
    {
        Logger.Debug($"EaglePatch: {oldValue} -> {newValue}");
    }

    partial void OnSelectedKeyboardLayoutChanged(string? oldValue, string newValue)
    {
        Logger.Debug($"Keyboard Layout: {oldValue} -> {newValue}");
    }

    partial void OnIsPs3ControlsEnabledChanged(bool oldValue, bool newValue)
    {
        Logger.Debug($"PS3 Controls: {oldValue} -> {newValue}");
    }

    partial void OnIsSkipIntroVideosEnabledChanged(bool oldValue, bool newValue)
    {
        Logger.Debug($"Skip Intro Videos: {oldValue} -> {newValue}");
    }

    partial void OnIsWindowedModePatchEnabledChanged(bool oldValue, bool newValue)
    {
        Logger.Debug($"Windowed Mode Patch: {oldValue} -> {newValue}");
    }

    partial void OnIsBorderlessFullscreenEnabledChanged(bool oldValue, bool newValue)
    {
        Logger.Debug($"Borderless Fullscreen: {oldValue} -> {newValue}");
    }
}