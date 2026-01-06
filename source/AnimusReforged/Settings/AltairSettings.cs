using System.Text.Json.Serialization;
using AnimusReforged.Settings.Core;

namespace AnimusReforged.Settings;

/// <summary>
/// Manages Altair-specific application settings with loading, saving, and access functionality.
/// </summary>
public class AltairSettings : AbstractSettings<AltairSettings.AltairSettingsStore>
{
    /// <summary>
    /// Represents the store for Altair-specific settings with common base properties.
    /// </summary>
    public class AltairSettingsStore : BaseSettingsStore
    {
        /// <summary>
        /// Gets or sets the tweak settings for the application.
        /// </summary>
        [JsonPropertyName("tweaks")]
        public TweakSettings Tweaks { get; set; } = new TweakSettings();
    }
}

/// <summary>
/// Defines tweak settings for the Altair application.
/// </summary>
public class TweakSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether UMod is enabled.
    /// </summary>
    [JsonPropertyName("umod")]
    public bool UMod { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether Reshade is enabled.
    /// </summary>
    [JsonPropertyName("reshade")]
    public bool Reshade { get; set; } = false;
}