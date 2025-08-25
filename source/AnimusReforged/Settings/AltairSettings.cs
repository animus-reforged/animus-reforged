using System.Text.Json.Serialization;

namespace AnimusReforged.Settings;

/// <summary>
/// Altair settings
/// </summary>
public class AltairSettings() : AbstractSettings<AltairSettings.AltairSettingsStore>()
{
    public class AltairSettingsStore : BaseSettingsStore
    {
        // Settings
        [JsonPropertyName("tweaks")]
        public TweakSettings Tweaks { get; set; } = new TweakSettings();
    }
}

public class TweakSettings
{
    [JsonPropertyName("umod")]
    public bool UMod { get; set; } = false;
    
    [JsonPropertyName("stutter_fix")]
    public bool StutterFix { get; set; } = false;
    
    [JsonPropertyName("high_core_count_fix")]
    public bool HighCoreCountFix { get; set; } = false;
    
    [JsonPropertyName("windowed_mode_patch")]
    public bool WindowedModePatch { get; set; } = false;
    
    [JsonPropertyName("borderless_fullscreen")]
    public bool BorderlessFullScreen { get; set; } = false;
}