using System.Text.Json.Serialization;
using AnimusReforged.Settings.Core;

namespace AnimusReforged.Settings;

/// <summary>
/// Manages Altair-specific application settings with loading, saving, and access functionality.
/// Includes methods for updating installed mod versions.
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

        /// <summary>
        /// Gets or sets the installed mod versions, mapping mod IDs to their installed versions.
        /// </summary>
        [JsonPropertyName("installed_mod_versions")]
        public Dictionary<string, string> InstalledModVersions { get; set; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// Updates the installed version for a specific mod.
    /// </summary>
    /// <param name="modId">The unique identifier of the mod</param>
    /// <param name="version">The version of the mod that was installed</param>
    public void UpdateInstalledModVersion(string modId, string version)
    {
        Settings.InstalledModVersions[modId] = version;
    }

    /// <summary>
    /// Gets the installed version of a specific mod, if available.
    /// </summary>
    /// <param name="modId">The unique identifier of the mod</param>
    /// <returns>The installed version of the mod, or null if not installed</returns>
    public string? GetInstalledModVersion(string modId)
    {
        if (Settings.InstalledModVersions.TryGetValue(modId, out string? version))
        {
            return version;
        }
        return null;
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
}