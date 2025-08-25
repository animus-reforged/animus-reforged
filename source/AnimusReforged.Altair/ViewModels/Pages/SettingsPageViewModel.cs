using System.Collections.ObjectModel;
using System.IO;
using AnimusReforged.Altair.Views;
using AnimusReforged.Mods.Utilities;
using AnimusReforged.Paths;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AnimusReforged.Altair.ViewModels.Pages;

public partial class SettingsPageViewModel : ViewModelBase
{
    // Variables
    private bool _suppressUpdates;
    [ObservableProperty] private bool isUModEnabled = App.Settings.Tweaks.UMod;

    [ObservableProperty] private bool isReShadeEnabled;

    [ObservableProperty] private bool isEaglePatchEnabled;

    // Keyboard Layouts
    [ObservableProperty] private int selectedKeyboardLayoutIndex = 2; // Default Value
    public ObservableCollection<string> KeyboardLayouts { get; } = ["KeyboardMouse2", "KeyboardMouse5", "Keyboard", "KeyboardAlt"];

    // EaglePatch
    private IniParser _eaglePatchSettings = null!;
    [ObservableProperty] private bool isPs3ControlsEnabled;

    [ObservableProperty] private bool isSkipIntroVideosEnabled;

    [ObservableProperty] private bool isStutterFixEnabled = App.Settings.Tweaks.StutterFix;

    [ObservableProperty] private bool isWindowedModePatchEnabled = App.Settings.Tweaks.WindowedModePatch;

    [ObservableProperty] private bool isBorderlessFullscreenEnabled = App.Settings.Tweaks.BorderlessFullScreen;

    // Constructor
    public SettingsPageViewModel()
    {
        _suppressUpdates = true;
        // TODO: Loading settings from files
        // Load EaglePatch settings
        LoadEaglePatchSettings();
        _suppressUpdates = false;
    }

    // Methods

    private void LoadEaglePatchSettings()
    {
        if (File.Exists(Path.Combine(AppPaths.Scripts, "EaglePatchAC1.asi")))
        {
            Logger.Info("EaglePatch is enabled");
            IsEaglePatchEnabled = true;
        }
        else if (File.Exists(Path.Combine(AppPaths.Scripts, "EaglePatchAC1.asi.disabled")))
        {
            Logger.Info("EaglePatch is disabled");
            IsEaglePatchEnabled = false;
        }
        else
        {
            Logger.Warning("EaglePatch is not installed");
            IsEaglePatchEnabled = false;
        }
        _eaglePatchSettings = new IniParser(AppPaths.AltairEaglePatchIni);
        SelectedKeyboardLayoutIndex = _eaglePatchSettings.GetInt("EaglePatchAC1", "KeyboardLayout");
        Logger.Info($"Selected keyboard layout: {SelectedKeyboardLayoutIndex}");
        IsPs3ControlsEnabled = _eaglePatchSettings.GetBool("EaglePatchAC1", "PS3Controls");
        Logger.Info($"PS3Controls: {IsPs3ControlsEnabled}");
        IsSkipIntroVideosEnabled = _eaglePatchSettings.GetBool("EaglePatchAC1", "SkipIntroVideos");
        Logger.Info($"SkipIntroVideos: {IsSkipIntroVideosEnabled}");
    }

    // TODO: Saving settings
    partial void OnIsUModEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"UMod: {oldValue} -> {newValue}");
        App.Settings.Tweaks.UMod = newValue;
        App.AppSettings.SaveSettings();
    }

    partial void OnIsReShadeEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"ReShade: {oldValue} -> {newValue}");
    }

    partial void OnIsEaglePatchEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        if (IsEaglePatchEnabled && !File.Exists(Path.Combine(AppPaths.Scripts, "EaglePatchAC1.asi")) && !File.Exists(Path.Combine(AppPaths.Scripts, "EaglePatchAC1.asi.disabled")))
        {
            Logger.Error("EaglePatch couldn't be found");
            Dispatcher.UIThread.Post(() =>
            {
                _suppressUpdates = true;
                IsEaglePatchEnabled = false;
                _suppressUpdates = false;
            });

            MessageBox.Show("EaglePatch couldn't be found. This could mean corrupted EaglePatch Installation.", "Error", App.MainWindow);
            return;
        }
        Logger.Debug($"EaglePatch: {oldValue} -> {newValue}");
    }

    partial void OnSelectedKeyboardLayoutIndexChanged(int oldValue, int newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"Keyboard Layout: {oldValue} -> {newValue}");
        _eaglePatchSettings.Set("EaglePatchAC1", "KeyboardLayout", newValue);
        _eaglePatchSettings.Save();
    }

    partial void OnIsPs3ControlsEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"PS3 Controls: {oldValue} -> {newValue}");
        _eaglePatchSettings.Set("EaglePatchAC1", "PS3Controls", newValue);
        _eaglePatchSettings.Save();
    }

    partial void OnIsSkipIntroVideosEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"Skip Intro Videos: {oldValue} -> {newValue}");
        _eaglePatchSettings.Set("EaglePatchAC1", "SkipIntroVideos", newValue);
        _eaglePatchSettings.Save();
    }

    partial void OnIsStutterFixEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"Stutter Fix: {oldValue} -> {newValue}");
        App.Settings.Tweaks.StutterFix = newValue;
        App.AppSettings.SaveSettings();
    }

    partial void OnIsWindowedModePatchEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"Windowed Mode Patch: {oldValue} -> {newValue}");
        App.Settings.Tweaks.WindowedModePatch = newValue;
        App.AppSettings.SaveSettings();
    }

    partial void OnIsBorderlessFullscreenEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"Borderless Fullscreen: {oldValue} -> {newValue}");
        App.Settings.Tweaks.BorderlessFullScreen = newValue;
        App.AppSettings.SaveSettings();
    }
}