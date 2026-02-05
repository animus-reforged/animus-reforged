using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AnimusReforged.Altair.Services.UI;
using AnimusReforged.Logging;
using AnimusReforged.Models.Mods;
using AnimusReforged.Mods.Core;
using AnimusReforged.Settings;
using AnimusReforged.Utilities;
using AnimusReforged.Utilities.Ini;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace AnimusReforged.Altair.ViewModels.Pages;

public partial class SettingsPageViewModel : ViewModelBase
{
    // Variables
    private readonly IMessageBoxService _messageBoxService;
    private AltairSettings _settings { get; set; }
    private bool _suppressUpdates;

    // uMod
    private UModTemplateFile _templateFile = null!;
    [ObservableProperty] private bool isuModEnabled;
    public ObservableCollection<UModItem> EnabledMods { get; } = [];
    public ObservableCollection<UModItem> DisabledMods { get; } = [];

    // ReShade
    [ObservableProperty] private bool isReShadeEnabled;

    // AltairFix
    private IniFile _altairFixSettings = null!;
    [ObservableProperty] private bool isAltairFixEnabled;
    [ObservableProperty] private bool isStutterFixEnabled;
    [ObservableProperty] private bool isHighCoreCountFixEnabled;

    public ObservableCollection<string> WindowModes { get; } = ["Fullscreen", "Borderless", "Windowed"];
    [ObservableProperty] private int selectedWindowModeIndex;
    [ObservableProperty] private bool isCustomResolutionMode;
    public ObservableCollection<int> SupportedWidths { get; } = [];
    [ObservableProperty] private int selectedWidth;

    public ObservableCollection<int> SupportedHeights { get; } = [];
    [ObservableProperty] private int selectedHeight;

    // EaglePatch
    private IniFile _eaglePatchSettings = null!;
    [ObservableProperty] private bool isEaglePatchEnabled;
    public ObservableCollection<string> KeyboardLayouts { get; } = ["KeyboardMouse2", "KeyboardMouse5", "Keyboard", "KeyboardAlt"];
    [ObservableProperty] private int selectedKeyboardLayoutIndex = 2; // Default Value
    [ObservableProperty] private bool isPs3ControlsEnabled;
    [ObservableProperty] private bool isSkipIntroVideoEnabled;

    // Core Settings
    [ObservableProperty] private int loggingLevel;
    public Dictionary<string, int> LogLevelOptions { get; } = new Dictionary<string, int>
    {
        {"Off", 0},
        {"Fatal", 1},
        {"Error", 2},
        {"Warn", 3},
        {"Info", 4},
        {"Debug", 5},
        {"Trace", 6}
    };

    public string SelectedLogLevelKey
    {
        get => LogLevelOptions.FirstOrDefault(x => x.Value == LoggingLevel).Key ?? "Info";
        set
        {
            if (!string.IsNullOrEmpty(value) && LogLevelOptions.ContainsKey(value))
            {
                LoggingLevel = LogLevelOptions[value];
                OnPropertyChanged();
            }
        }
    }

    // Constructor
    public SettingsPageViewModel()
    {
        _messageBoxService = App.Services.GetRequiredService<IMessageBoxService>();
        _settings = App.Services.GetRequiredService<AltairSettings>();
        _suppressUpdates = true;
        PopulateSupportedResolutions();
        LoaduModSettings();
        LoadReShadeSettings();
        LoadAltairFixSettings();
        LoadEaglePatchSettings();
        LoadCoreSettings();
        _suppressUpdates = false;
    }

    // Functions
    // Loading/UI functions
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
        IsuModEnabled = _settings.Settings.Tweaks.UMod;
        string templateFileLocation = Path.Combine(FilePaths.UModTemplates, "ac1.txt");
        _templateFile = new UModTemplateFile(templateFileLocation, FilePaths.ModsDirectory);
        IReadOnlyList<string> enabledMods;
        IReadOnlyList<string> disabledMods;
        try
        {
            (enabledMods, disabledMods) = _templateFile.LoadMods();
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to download Ultimate ASI Loader");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            return;
        }
        EnabledMods.Clear();
        DisabledMods.Clear();

        foreach (string mod in enabledMods)
        {
            EnabledMods.Add(new UModItem { FullPath = mod });
        }

        foreach (string mod in disabledMods)
        {
            DisabledMods.Add(new UModItem { FullPath = mod });
        }
    }

    private void LoadReShadeSettings()
    {
        if (File.Exists(Path.Combine(FilePaths.ScriptsDirectory, "ReShade.asi")))
        {
            Logger.Info<SettingsPageViewModel>("ReShade is enabled");
            IsReShadeEnabled = true;
        }
        else if (File.Exists(Path.Combine(FilePaths.ScriptsDirectory, "ReShade.asi.disabled")))
        {
            Logger.Info<SettingsPageViewModel>("ReShade is disabled");
            IsReShadeEnabled = false;
        }
        else
        {
            Logger.Warning<SettingsPageViewModel>("ReShade is not installed");
            IsReShadeEnabled = false;
        }
    }

    private void LoadAltairFixSettings()
    {
        if (File.Exists(Path.Combine(FilePaths.ScriptsDirectory, "AltairFix.asi")))
        {
            Logger.Info<SettingsPageViewModel>("AltairFix is enabled");
            IsAltairFixEnabled = true;
        }
        else if (File.Exists(Path.Combine(FilePaths.ScriptsDirectory, "AltairFix.asi.disabled")))
        {
            Logger.Info<SettingsPageViewModel>("AltairFix is disabled");
            IsAltairFixEnabled = false;
        }
        else
        {
            Logger.Warning<SettingsPageViewModel>("AltairFix is not installed");
            IsAltairFixEnabled = false;
        }

        // Loading .ini config
        _altairFixSettings = new IniFile(FilePaths.AltairFixSettingsFile);
        SelectedWindowModeIndex = _altairFixSettings.GetInt("Display", "WindowMode");
        Logger.Info<SettingsPageViewModel>($"Window Mode: {WindowModes[SelectedWindowModeIndex]}");
        IsCustomResolutionMode = SelectedWindowModeIndex == 2;
        SelectedWidth = _altairFixSettings.GetInt("Display", "WindowWidth");
        Logger.Info<SettingsPageViewModel>($"Window Width: {SelectedWidth}");
        SelectedHeight = _altairFixSettings.GetInt("Display", "WindowHeight");
        Logger.Info<SettingsPageViewModel>($"Window Height: {SelectedHeight}");
        IsStutterFixEnabled = _altairFixSettings.GetBool("Tweaks", "ServerBlocker");
        Logger.Info<SettingsPageViewModel>($"Stutter Fix: {IsStutterFixEnabled}");
        IsHighCoreCountFixEnabled = _altairFixSettings.GetBool("Tweaks", "HighCoreCountFix");
        Logger.Info<SettingsPageViewModel>($"High Core Count Fix: {IsHighCoreCountFixEnabled}");
    }

    private void LoadEaglePatchSettings()
    {
        if (File.Exists(Path.Combine(FilePaths.ScriptsDirectory, "EaglePatchAC1.asi")))
        {
            Logger.Info<SettingsPageViewModel>("EaglePatch is enabled");
            IsEaglePatchEnabled = true;
        }
        else if (File.Exists(Path.Combine(FilePaths.ScriptsDirectory, "EaglePatchAC1.asi.disabled")))
        {
            Logger.Info<SettingsPageViewModel>("EaglePatch is disabled");
            IsEaglePatchEnabled = false;
        }
        else
        {
            Logger.Warning<SettingsPageViewModel>("EaglePatch is not installed");
            IsEaglePatchEnabled = false;
        }

        // Loading .ini config
        _eaglePatchSettings = new IniFile(FilePaths.EaglePatchSettingsFile);
        SelectedKeyboardLayoutIndex = _eaglePatchSettings.GetInt("EaglePatchAC1", "KeyboardLayout");
        Logger.Info<SettingsPageViewModel>($"Selected keyboard layout: {KeyboardLayouts[SelectedKeyboardLayoutIndex]}");
        IsPs3ControlsEnabled = _eaglePatchSettings.GetBool("EaglePatchAC1", "PS3Controls");
        Logger.Info<SettingsPageViewModel>($"PS3 Controls: {IsPs3ControlsEnabled}");
        IsSkipIntroVideoEnabled = _eaglePatchSettings.GetBool("EaglePatchAC1", "SkipIntroVideos");
        Logger.Info<SettingsPageViewModel>($"PS3 Controls: {IsSkipIntroVideoEnabled}");
    }

    private void LoadCoreSettings()
    {
        LoggingLevel = _settings.Settings.Core.LoggingLevel;
        Logger.Info<SettingsPageViewModel>($"Logging Level: {LoggingLevel}");
    }

    // Saving UI Functions
    partial void OnIsuModEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        try
        {
            _settings.Settings.Tweaks.UMod = IsuModEnabled;
            _settings.SaveSettings();
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to change IsuModEnabled tweak");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            IsuModEnabled = oldValue;
            _messageBoxService.ShowErrorAsync("Error changing IsuModEnabled", "An error occurred while changing IsuModEnabled.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"uMod: {oldValue} -> {newValue}");
    }

    private bool UpdateAsiState(string asiFileName, bool enable)
    {
        string asiPath = Path.Combine(FilePaths.ScriptsDirectory, asiFileName);
        string disabledPath = asiPath + ".disabled";

        if (enable)
        {
            if (File.Exists(disabledPath))
            {
                File.Move(disabledPath, asiPath);
                Logger.Info<SettingsPageViewModel>($"{asiFileName} enabled");
                return true;
            }
            if (File.Exists(asiPath))
            {
                // Already enabled
                return true;
            }
            Logger.Error<SettingsPageViewModel>($"{asiFileName} file not found (enable requested)");
            return false;
        }
        if (!File.Exists(asiPath))
        {
            return true;
        }
        File.Move(asiPath, disabledPath);
        Logger.Info<SettingsPageViewModel>($"{asiFileName} disabled");
        // Already disabled — that’s fine too
        return true;
    }

    partial void OnIsReShadeEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }

        try
        {
            if (!UpdateAsiState("ReShade.asi", newValue))
            {
                // If we couldn't find the file when enabling, revert the toggle in UI
                _suppressUpdates = true;
                IsReShadeEnabled = false;
                _suppressUpdates = false;
                _messageBoxService.ShowErrorAsync("ReShade Not Found", "ReShade.asi couldn't be found. This could mean a corrupted ReShade installation.");
                return;
            }
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to toggle ReShade state");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            IsReShadeEnabled = oldValue;
            _messageBoxService.ShowErrorAsync("Error toggling ReShade", "An error occurred while toggling ReShade. Please check file permissions.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"ReShade: {oldValue} -> {newValue}");
    }

    partial void OnIsAltairFixEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }

        try
        {
            if (!UpdateAsiState("AltairFix.asi", newValue))
            {
                // If we couldn't find the file when enabling, revert the toggle in UI
                _suppressUpdates = true;
                IsAltairFixEnabled = false;
                _suppressUpdates = false;
                _messageBoxService.ShowErrorAsync("AltairFix Not Found", "AltairFix.asi couldn't be found. This could mean a corrupted AltairFix installation.");
                return;
            }
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to toggle AltairFix state");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            IsAltairFixEnabled = oldValue;
            _messageBoxService.ShowErrorAsync("Error toggling AltairFix", "An error occurred while toggling AltairFix. Please check file permissions.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"AltairFix: {oldValue} -> {newValue}");
    }

    partial void OnIsStutterFixEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        try
        {
            _altairFixSettings.Set("Tweaks", "ServerBlocker", newValue);
            _altairFixSettings.Save();
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to change ServerBlocker tweak");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            IsStutterFixEnabled = oldValue;
            _messageBoxService.ShowErrorAsync("Error changing ServerBlocker (AltairFix)", "An error occurred while changing ServerBlocker.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"Stutter Fix: {oldValue} -> {newValue}");
    }

    partial void OnIsHighCoreCountFixEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        try
        {
            _altairFixSettings.Set("Tweaks", "HighCoreCountFix", newValue);
            _altairFixSettings.Save();
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to change HighCoreCountFix tweak");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            IsHighCoreCountFixEnabled = oldValue;
            _messageBoxService.ShowErrorAsync("Error changing HighCoreCountFix (AltairFix)", "An error occurred while changing HighCoreCountFix.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"High Core Count Fix: {oldValue} -> {newValue}");
    }

    partial void OnSelectedWindowModeIndexChanged(int oldValue, int newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        try
        {
            IsCustomResolutionMode = newValue == 2;
            _altairFixSettings.Set("Display", "WindowMode", newValue);
            _altairFixSettings.Save();
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to change Window Mode tweak");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            SelectedWindowModeIndex = oldValue;
            IsCustomResolutionMode = newValue == 2;
            _messageBoxService.ShowErrorAsync("Error changing Window Mode (AltairFix)", "An error occurred while changing Window Mode.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"Window Mode: {WindowModes[oldValue]} -> {WindowModes[newValue]}");
    }

    partial void OnSelectedWidthChanged(int oldValue, int newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        try
        {
            _altairFixSettings.Set("Display", "WindowWidth", newValue);
            _altairFixSettings.Save();
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to change Window Width tweak");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            SelectedWidth = oldValue;
            _messageBoxService.ShowErrorAsync("Error changing Window Width (AltairFix)", "An error occurred while changing Window Width.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"Window Width: {oldValue} -> {newValue}");
    }

    partial void OnSelectedHeightChanged(int oldValue, int newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        try
        {
            _altairFixSettings.Set("Display", "WindowHeight", newValue);
            _altairFixSettings.Save();
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to change Window Height tweak");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            SelectedHeight = oldValue;
            _messageBoxService.ShowErrorAsync("Error changing Window Height (AltairFix)", "An error occurred while changing Window Height.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"Window Height: {oldValue} -> {newValue}");
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
                // If we couldn't find the file when enabling, revert the toggle in UI
                _suppressUpdates = true;
                IsEaglePatchEnabled = false;
                _suppressUpdates = false;
                _messageBoxService.ShowErrorAsync("EaglePatch Not Found", "EaglePatchAC1.asi couldn't be found. This could mean a corrupted EaglePatch installation.");
                return;
            }
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to toggle EaglePatch state");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            IsEaglePatchEnabled = oldValue;
            _messageBoxService.ShowErrorAsync("Error toggling EaglePatch", "An error occurred while toggling EaglePatch. Please check file permissions.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"EaglePatch: {oldValue} -> {newValue}");
    }

    partial void OnSelectedKeyboardLayoutIndexChanged(int oldValue, int newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        try
        {
            _eaglePatchSettings.Set("EaglePatchAC1", "KeyboardLayout", newValue);
            _eaglePatchSettings.Save();
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to change Keyboard Layout tweak");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            SelectedWindowModeIndex = oldValue;
            _messageBoxService.ShowErrorAsync("Error changing Keyboard Layout (EaglePatch)", "An error occurred while changing Keyboard Layout.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"Keyboard Layout: {oldValue} -> {newValue}");
    }

    partial void OnIsPs3ControlsEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        try
        {
            _eaglePatchSettings.Set("EaglePatchAC1", "PS3Controls", newValue);
            _eaglePatchSettings.Save();
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to change IsPs3ControlsEnabled tweak");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            IsPs3ControlsEnabled = oldValue;
            _messageBoxService.ShowErrorAsync("Error changing IsPs3ControlsEnabled (EaglePatch)", "An error occurred while changing IsPs3ControlsEnabled.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"PS3 Controls: {oldValue} -> {newValue}");
    }

    partial void OnIsSkipIntroVideoEnabledChanged(bool oldValue, bool newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        try
        {
            _eaglePatchSettings.Set("EaglePatchAC1", "SkipIntroVideos", newValue);
            _eaglePatchSettings.Save();
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to change IsSkipIntroVideoEnabled tweak");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            IsSkipIntroVideoEnabled = oldValue;
            _messageBoxService.ShowErrorAsync("Error changing IsSkipIntroVideoEnabled (EaglePatch)", "An error occurred while changing IsSkipIntroVideoEnabled.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"Skip Intro Videos: {oldValue} -> {newValue}");
    }

    partial void OnLoggingLevelChanged(int oldValue, int newValue)
    {
        if (_suppressUpdates)
        {
            return;
        }
        try
        {
            _settings.Settings.Core.LoggingLevel = newValue;
            _settings.SaveSettings();
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to change Logging Level setting");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
            LoggingLevel = oldValue;
            _messageBoxService.ShowErrorAsync("Error changing Logging Level", "An error occurred while changing Logging Level.");
            return;
        }
        Logger.Debug<SettingsPageViewModel>($"Logging Level: {oldValue} -> {newValue}");
    }

    // Commands
    [RelayCommand]
    private async Task AddMod()
    {
        try
        {
            // Check if we have StorageProvider
            if (App.MainWindow?.StorageProvider is not { } storageProvider)
            {
                Logger.Warning<SettingsPageViewModel>("Storage provider is not available");
                return;
            }

            // Create file picker
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
                // Copy the file to the Mods folder
                string oldFilePath = file.Path.LocalPath;
                string newFilePath = Path.Combine(FilePaths.ModsDirectory, Path.GetFileName(oldFilePath));
                File.Copy(oldFilePath, newFilePath, true);

                // Check if the mod is already loaded into the UI lists
                if (EnabledMods.Any(mod => mod.FullPath == newFilePath) || DisabledMods.Any(mod => mod.FullPath == newFilePath))
                {
                    Logger.Warning<SettingsPageViewModel>($"Mod is already in the UI ({newFilePath})");
                    continue;
                }

                // If it's not, load it yourself
                UModItem mod = new UModItem { FullPath = newFilePath };
                EnabledMods.Add(mod);
                Logger.Info<SettingsPageViewModel>($"Added mod: {mod.FullPath}");
            }

            _templateFile.SaveEnabledMods(EnabledMods.ToList());
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to add mod(s)");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
        }
    }

    [RelayCommand]
    private void MoveModUp(UModItem mod)
    {
        try
        {
            int index = EnabledMods.IndexOf(mod);
            if (index <= 0)
            {
                return;
            }
            EnabledMods.Move(index, index - 1);
            _templateFile.SaveEnabledMods(EnabledMods.ToList());
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to move mod up");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
        }
    }

    [RelayCommand]
    private void MoveModDown(UModItem mod)
    {
        try
        {
            int index = EnabledMods.IndexOf(mod);
            if (index >= EnabledMods.Count - 1)
            {
                return;
            }
            EnabledMods.Move(index, index + 1);
            _templateFile.SaveEnabledMods(EnabledMods.ToList());
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to move mod down");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
        }
    }

    [RelayCommand]
    private void EnableMod(UModItem mod)
    {
        try
        {
            if (DisabledMods.Remove(mod))
            {
                EnabledMods.Add(mod);
                _templateFile.SaveEnabledMods(EnabledMods.ToList());
            }
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to enable mod");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
        }
    }

    [RelayCommand]
    private void DisableMod(UModItem mod)
    {
        try
        {
            if (EnabledMods.Remove(mod))
            {
                Logger.Info<SettingsPageViewModel>($"Disabled mod: {mod.FullPath}");
                DisabledMods.Add(mod);
                _templateFile.SaveEnabledMods(EnabledMods.ToList());
            }
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to disable mod");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
        }
    }

    [RelayCommand]
    private void RemoveMod(UModItem mod)
    {
        try
        {
            if (!EnabledMods.Remove(mod))
            {
                DisabledMods.Remove(mod);
            }
            File.Delete(mod.FullPath);
            Logger.Info<SettingsPageViewModel>($"Removed mod: {mod.FullPath}");
            _templateFile.SaveEnabledMods(EnabledMods.ToList());
        }
        catch (Exception ex)
        {
            Logger.Error<SettingsPageViewModel>("Failed to remove mod");
            Logger.LogExceptionDetails<SettingsPageViewModel>(ex);
        }
    }
}