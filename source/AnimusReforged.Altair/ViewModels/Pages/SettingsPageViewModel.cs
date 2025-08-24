using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AnimusReforged.Altair.ViewModels.Pages;

public partial class SettingsPageViewModel : ViewModelBase
{
    // Variables
    [ObservableProperty] private bool isUModEnabled = App.Settings.Tweaks.UMod;

    [ObservableProperty] private bool isReShadeEnabled;

    [ObservableProperty] private bool isEaglePatchEnabled;

    // Keyboard Layouts
    [ObservableProperty] private string selectedKeyboardLayout = "Keyboard"; // Default value
    public ObservableCollection<string> KeyboardLayouts { get; } = ["KeyboardMouse2", "KeyboardMouse5", "Keyboard", "KeyboardAlt"];

    [ObservableProperty] private bool isPs3ControlsEnabled;

    [ObservableProperty] private bool isSkipIntroVideosEnabled;

    [ObservableProperty] private bool isStutterFixEnabled = App.Settings.Tweaks.StutterFix;

    [ObservableProperty] private bool isWindowedModePatchEnabled = App.Settings.Tweaks.WindowedModePatch;

    [ObservableProperty] private bool isBorderlessFullscreenEnabled = App.Settings.Tweaks.BorderlessFullScreen;

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
        App.Settings.Tweaks.UMod = newValue;
        App.AppSettings.SaveSettings();
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

    partial void OnIsStutterFixEnabledChanged(bool oldValue, bool newValue)
    {
        Logger.Debug($"Stutter Fix: {oldValue} -> {newValue}");
        App.Settings.Tweaks.StutterFix = newValue;
        App.AppSettings.SaveSettings();
    }

    partial void OnIsWindowedModePatchEnabledChanged(bool oldValue, bool newValue)
    {
        Logger.Debug($"Windowed Mode Patch: {oldValue} -> {newValue}");
        App.Settings.Tweaks.WindowedModePatch = newValue;
        App.AppSettings.SaveSettings();
    }

    partial void OnIsBorderlessFullscreenEnabledChanged(bool oldValue, bool newValue)
    {
        Logger.Debug($"Borderless Fullscreen: {oldValue} -> {newValue}");
        App.Settings.Tweaks.BorderlessFullScreen = newValue;
        App.AppSettings.SaveSettings();
    }
}