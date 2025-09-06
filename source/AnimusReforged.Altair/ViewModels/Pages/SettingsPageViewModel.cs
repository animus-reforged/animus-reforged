using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnimusReforged.Altair.Views;
using AnimusReforged.Mods.Altair;
using AnimusReforged.Mods.Utilities;
using AnimusReforged.Paths;
using AnimusReforged.Utilities;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AnimusReforged.Altair.ViewModels.Pages;

public class ModItem
{
    public string FullPath { get; set; } = string.Empty;
    public string Name => Path.GetFileNameWithoutExtension(FullPath);
}

public static class ModItemExtensions
{
    public static List<string> ToList(this IEnumerable<ModItem> mods)
    {
        return mods.Select(mod => mod.FullPath).ToList();
    }
}

public partial class SettingsPageViewModel : ViewModelBase
{
    // Variables
    private bool _suppressUpdates;
    [ObservableProperty] private bool isUModEnabled = App.Settings.Tweaks.UMod;

    [ObservableProperty] private bool isReShadeEnabled;

    [ObservableProperty] private bool isEaglePatchEnabled;

    // uMod
    private uModTemplateParser _uModTemplateParser = null!;
    public ObservableCollection<ModItem> EnabledMods { get; } = [];
    public ObservableCollection<ModItem> DisabledMods { get; } = [];

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
    [ObservableProperty] private int selectedWidth;

    public ObservableCollection<int> SupportedHeights { get; } = [];
    [ObservableProperty] private int selectedHeight;

    // Constructor
    public SettingsPageViewModel()
    {
        _suppressUpdates = true;
        PopulateSupportedResolutions();

        // uMod
        LoaduModSettings();

        // ReShade
        IsReShadeEnabled = App.Settings.Tweaks.Reshade.Enabled;

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
    // Loading methods
    private void PopulateSupportedResolutions()
    {
        (List<int> widths, List<int> heights) = DisplayHelper.GetSupportedResolutions();
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

    private void LoaduModSettings()
    {
        _uModTemplateParser = new uModTemplateParser(AppPaths.AltairuModTemplateFile, AppPaths.Mods);
        List<string> enabledMods;
        List<string> disabledMods;
        try
        {
            (enabledMods, disabledMods) = _uModTemplateParser.LoadMods();
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to download Ultimate ASI Loader");
            Logger.LogExceptionDetails(ex);
            return;
        }
        EnabledMods.Clear();
        DisabledMods.Clear();

        foreach (string mod in enabledMods)
        {
            EnabledMods.Add(new ModItem { FullPath = mod });
        }
        
        foreach (string mod in disabledMods)
        {
            DisabledMods.Add(new ModItem { FullPath = mod });
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
        SelectedWidth = _altairFixSettings.GetInt("Display", "WindowWidth");
        Logger.Info($"Window Width: {SelectedWidth}");
        SelectedHeight = _altairFixSettings.GetInt("Display", "WindowHeight");
        Logger.Info($"Window Height: {SelectedHeight}");
    }
    
    // Saving/UI Interactions
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

    [RelayCommand]
    private async Task AddMod()
    {
        if (App.MainWindow?.StorageProvider is not { } storageProvider)
        {
            return;
        }
        FilePickerOpenOptions options = new FilePickerOpenOptions
        {
            Title = "Select Texture Mod",
            AllowMultiple = true,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("Texture Package Files") { Patterns = ["*.tpf"] }
            }
        };

        IReadOnlyList<IStorageFile> files = await storageProvider.OpenFilePickerAsync(options);

        foreach (IStorageFile file in files)
        {
            string oldFilePath = file.Path.LocalPath;
            string newFilePath = Path.Combine(AppPaths.Mods, Path.GetFileName(oldFilePath));
            File.Copy(oldFilePath, newFilePath, true);
            
            if (EnabledMods.Any(m => m.FullPath == newFilePath) || DisabledMods.Any(m => m.FullPath == newFilePath))
            {
                Logger.Warning($"Mod already exists: {newFilePath}");
                continue;
            }

            ModItem modItem = new ModItem { FullPath = newFilePath };
            EnabledMods.Add(modItem);
            Logger.Info($"Added mod: {newFilePath}");
        }

        _uModTemplateParser.SaveEnabledMods(EnabledMods.ToList());
    }

    [RelayCommand]
    private void MoveModUp(ModItem mod)
    {
        int index = EnabledMods.IndexOf(mod);
        if (index <= 0)
        {
            return;
        }
        EnabledMods.Move(index, index - 1);
        _uModTemplateParser.SaveEnabledMods(EnabledMods.ToList());
    }

    [RelayCommand]
    private void MoveModDown(ModItem mod)
    {
        int index = EnabledMods.IndexOf(mod);
        if (index >= EnabledMods.Count - 1)
        {
            return;
        }
        EnabledMods.Move(index, index + 1);
        _uModTemplateParser.SaveEnabledMods(EnabledMods.ToList());
    }
    
    [RelayCommand]
    private void EnableMod(ModItem mod)
    {
        if (DisabledMods.Remove(mod))
        {
            EnabledMods.Add(mod);
            _uModTemplateParser.SaveEnabledMods(EnabledMods.ToList());
        }
    }

    [RelayCommand]
    private void DisableMod(ModItem mod)
    {
        if (EnabledMods.Remove(mod))
        {
            DisabledMods.Add(mod);
            _uModTemplateParser.SaveEnabledMods(EnabledMods.ToList());
        }
    }

    [RelayCommand]
    private void RemoveMod(ModItem mod)
    {
        if (!EnabledMods.Remove(mod))
        {
            DisabledMods.Remove(mod);
        }
        File.Delete(mod.FullPath);
        _uModTemplateParser.SaveEnabledMods(EnabledMods.ToList());
    }

    partial void OnIsReShadeEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"ReShade: {oldValue} -> {newValue}");
        try
        {
            if (!UpdateAsiState("ReShade.asi", newValue))
            {
                // Couldn’t find the file when enabling, so revert in UI
                _suppressUpdates = true;
                IsReShadeEnabled = false;
                _suppressUpdates = false;

                MessageBox.Show("ReShade couldn't be found. This may mean a corrupted ReShade installation.", "Error", App.MainWindow);
                return;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to toggle ReShade state");

            // Revert UI state to last known good
            _suppressUpdates = true;
            IsReShadeEnabled = oldValue;
            _suppressUpdates = false;

            MessageBox.Show("An error occurred while toggling ReShade. Please check file permissions.", "Error", App.MainWindow);
            return;
        }
        App.Settings.Tweaks.Reshade.Enabled = newValue;
        App.AppSettings.SaveSettings();
    }

    partial void OnIsEaglePatchEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }

        try
        {
            if (!UpdateAsiState("EaglePatchAC1.asi", newValue))
            {
                // If we couldn’t find the file when enabling, revert the toggle in UI
                _suppressUpdates = true;
                IsEaglePatchEnabled = false;
                _suppressUpdates = false;

                MessageBox.Show("EaglePatch couldn't be found. This could mean a corrupted EaglePatch installation.", "Error", App.MainWindow);
                return;
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to toggle EaglePatch state");
            _suppressUpdates = true;
            IsEaglePatchEnabled = oldValue;
            _suppressUpdates = false;

            MessageBox.Show("An error occurred while toggling EaglePatch. Please check file permissions.", "Error", App.MainWindow);
            return;
        }

        Logger.Debug($"EaglePatch: {oldValue} -> {newValue}");
    }

    private bool UpdateAsiState(string asiFileName, bool enable)
    {
        string asiPath = Path.Combine(AppPaths.Scripts, asiFileName);
        string disabledPath = asiPath + ".disabled";

        if (enable)
        {
            if (File.Exists(disabledPath))
            {
                File.Move(disabledPath, asiPath);
                Logger.Info($"{asiFileName} enabled");
                return true;
            }
            else if (File.Exists(asiPath))
            {
                // Already enabled
                return true;
            }
            else
            {
                Logger.Error($"{asiFileName} file not found (enable requested)");
                return false;
            }
        }
        else
        {
            if (File.Exists(asiPath))
            {
                File.Move(asiPath, disabledPath);
                Logger.Info($"{asiFileName} disabled");
            }
            // Already disabled — that’s fine too
            return true;
        }
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

    partial void OnIsHighCoreCountFixEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        Logger.Debug($"High Core Count Fix: {oldValue} -> {newValue}");
        App.Settings.Tweaks.HighCoreCountFix = newValue;
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