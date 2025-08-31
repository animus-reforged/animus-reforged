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
            var version = assembly?.GetName()?.Version;
            if (version == null)
            {
                return "0.0.0";
            }
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
        catch
        {
            return "0.0.0";
        }
    }
}

public class ReShadeSettings
{
    [JsonPropertyName("install_date")]
    public DateTime? InstallDate { get; set; } = null;
        
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = false;
}