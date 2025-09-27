using System.Reflection;
using System.Text.Json.Serialization;

namespace AnimusReforged.Settings;

public abstract class BaseSettingsStore
{
    // Variables
    [JsonPropertyName("setup_completed")]
    public bool SetupCompleted { get; set; } = false;
    
    // Methods
    public string GetVersion()
    {
        try
        {
            Assembly? assembly = Assembly.GetEntryAssembly();
            Version? version = assembly?.GetName()?.Version;
            return version == null ? "0.0.0" : $"{version.Major}.{version.Minor}.{version.Build}";
        }
        catch
        {
            return "0.0.0";
        }
    }
}