using System.Reflection;
using System.Text.Json.Serialization;

namespace AnimusReforged.Settings.Core;

/// <summary>
/// Provides a base implementation for settings stores with common properties and functionality.
/// </summary>
public abstract class BaseSettingsStore
{
    /// <summary>
    /// Stores indicator and executable location required for the program to work
    /// </summary>
    [JsonPropertyName("setup")]
    public SetupSettings Setup { get; set; } = new SetupSettings();

    /// <summary>
    /// Gets or sets the date when the last update check was performed.
    /// </summary>
    [JsonPropertyName("last_update_check_date")]
    public DateTime? LastUpdateCheckDate { get; set; } = null;

    /// <summary>
    /// Gets or sets the core settings for the application.
    /// </summary>
    [JsonPropertyName("settings")]
    public CoreSettings Core { get; set; } = new CoreSettings();

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

/// <summary>
/// Contains setup-related settings for the application.
/// This includes the setup completion status and the location of the game executable.
/// </summary>
public class SetupSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the initial setup process has been completed.
    /// This is used to determine if the application should show the setup wizard on startup.
    /// </summary>
    [JsonPropertyName("completed")]
    public bool Completed { get; set; } = false;
}