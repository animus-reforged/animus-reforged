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
}