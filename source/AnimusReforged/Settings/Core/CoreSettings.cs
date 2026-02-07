using System.Text.Json.Serialization;

namespace AnimusReforged.Settings.Core;

/// <summary>
/// Universal core settings for the launcher
/// </summary>
public class CoreSettings
{
    /// <summary>
    /// Logging level of the launcher
    /// </summary>
    [JsonPropertyName("log_level")]
    public int LoggingLevel { get; set; } = 5;

    /// <summary>
    /// The language code for the application UI
    /// </summary>
    [JsonPropertyName("language")]
    public string Language { get; set; } = "en";
}