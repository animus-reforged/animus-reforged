using System.Reflection;
using System.Text.Json.Serialization;

namespace AnimusReforged.Settings.Core;

/// <summary>
/// Provides a base implementation for settings stores with common properties and functionality.
/// </summary>
public abstract class BaseSettingsStore
{
    /// <summary>
    /// Gets or sets a value indicating whether the initial setup has been completed.
    /// </summary>
    [JsonPropertyName("setup_completed")]
    public bool SetupCompleted { get; set; } = false;

    /// <summary>
    /// Gets the version of the application from the assembly information.
    /// </summary>
    /// <returns>The application version in major.minor.build format, or "0.0.0" if version information is unavailable.</returns>
    public string GetVersion()
    {
        try
        {
            Assembly? assembly = Assembly.GetEntryAssembly();
            Version? version = assembly?.GetName().Version;
            return version == null ? "0.0.0" : $"{version.Major}.{version.Minor}.{version.Build}";
        }
        catch
        {
            return "0.0.0";
        }
    }
}