using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using AnimusReforged.Altair.Views;
using AnimusReforged.Mods.Altair;
using AnimusReforged.Mods.Utilities;
using AnimusReforged.Paths;
using AnimusReforged.Utilities;
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

    [ObservableProperty] private bool isStutterFixEnabled;

    [ObservableProperty] private bool isHighCoreCountFixEnabled;

    // Windowed Mode
    private IniParser _altairFixSettings = null!;
    private ObservableCollection<string> WindowModes { get; } = ["Fullscreen", "Borderless", "Windowed"];

    [ObservableProperty] private int selectedWindowModeIndex;

    [ObservableProperty] private bool isCustomResolutionMode;
    
    public ObservableCollection<int> SupportedWidths { get; } = [];
    [ObservableProperty]
    private int selectedWidth;
    
    public ObservableCollection<int> SupportedHeights { get; } = [];
    [ObservableProperty]
    private int selectedHeight;

    // Constructor
    public SettingsPageViewModel()
    {
        _suppressUpdates = true;
        PopulateSupportedResolutions();
        // TODO: Loading settings from files
        // Load EaglePatch settings
        LoadEaglePatchSettings();
        
        // Load AltairFix settings
        LoadAltairFixSettings();
        
        // Stutter Patch
        IsStutterFixEnabled = App.Settings.Tweaks.StutterFix;

        // High Core Count Fix
        IsHighCoreCountFixEnabled = App.Settings.Tweaks.HighCoreCountFix;

        _suppressUpdates = false;
    }

    // Methods

    private void PopulateSupportedResolutions()
    {
        (List<int> widths, List<int> heights)= DisplayHelper.GetSupportedResolutions();
        SupportedWidths.Clear();
        SupportedHeights.Clear();
        foreach (int width in widths)
        {
            SupportedWidths.Add(width);
        }
        foreach (int height in heights)
        {
            SupportedHeights.Add(height);
        }
    }
    
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

    private void LoadAltairFixSettings()
    {
        _altairFixSettings = new IniParser(AppPaths.AltairFixIni);
        SelectedWindowModeIndex = _altairFixSettings.GetInt("Display", "WindowMode");
        Logger.Info($"Window Mode: {WindowModes[SelectedWindowModeIndex]}");
        IsCustomResolutionMode = SelectedWindowModeIndex == 2;
        SelectedWidth = _altairFixSettings.GetInt("Display","WindowWidth");
        Logger.Info($"Window Width: {SelectedWidth}");
        SelectedHeight = _altairFixSettings.GetInt("Display","WindowHeight");
        Logger.Info($"Window Height: {SelectedHeight}");
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
        try
        {
            // Apply Patch
            if (newValue)
            {
                Patcher.StutterPatch(AppPaths.AltairGameExecutable);
            }
            else
            {
                // Revert Patch
                Patcher.StutterPatchRevert(AppPaths.AltairGameExecutable);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            Logger.LogExceptionDetails(ex);
            MessageBox.Show($"Failed to apply Stutter Fix. Full Error:\n{ex}", "Error", App.MainWindow);
        }
        App.Settings.Tweaks.StutterFix = newValue;
        App.AppSettings.SaveSettings();
    }

    partial void OnSelectedWindowModeIndexChanged(int oldValue, int newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"Window Mode: {WindowModes[oldValue]} -> {WindowModes[newValue]}");
        IsCustomResolutionMode = newValue == 2;
        _altairFixSettings.Set("Display", "WindowMode", newValue);
        _altairFixSettings.Save();
    }
    
    partial void OnSelectedWidthChanged(int oldValue, int newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"Window Width: {oldValue} -> {newValue}");
        _altairFixSettings.Set("Display", "WindowWidth", newValue);
        _altairFixSettings.Save();
    }

    partial void OnSelectedHeightChanged(int oldValue, int newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"Window Height: {oldValue} -> {newValue}");
        _altairFixSettings.Set("Display", "WindowHeight", newValue);
        _altairFixSettings.Save();
    }
}