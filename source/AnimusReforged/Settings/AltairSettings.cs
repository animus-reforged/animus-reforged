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

    [JsonPropertyName("reshade")] 
    public ReShadeSettings Reshade { get; set; } = new ReShadeSettings();
}