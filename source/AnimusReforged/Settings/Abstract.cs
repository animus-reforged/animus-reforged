using System.Text.Json;
using System.Text.Json.Serialization;
using AnimusReforged.Paths;

namespace AnimusReforged.Settings;

public abstract class AbstractSettings<T> where T : class, new()
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly string _settingsPath;
    private T? _settings;

    protected virtual T DefaultSettings => new();
    public T Settings => _settings ??= LoadSettings();

    protected AbstractSettings()
    {
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = {new JsonStringEnumConverter()},
            WriteIndented = true
        };

        _settingsPath = AppPaths.ConfigFile;
    }

    /// <summary>
    /// Load the settings from the file. If the file does not exist or fails to load, the default settings are returned.
    /// </summary>
    public virtual T LoadSettings()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                // Save the default settings if the file is missing
                SaveSettings(DefaultSettings);
                return DefaultSettings;
            }

            string settingsSerialized = File.ReadAllText(_settingsPath);
            var settings = JsonSerializer.Deserialize<T>(settingsSerialized, _jsonSerializerOptions);
            return settings ?? DefaultSettings;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.Message);
            return DefaultSettings;
        }
    }

    /// <summary>
    /// Save the current settings into the file.
    /// </summary>
    public void SaveSettings() => SaveSettings(_settings ?? DefaultSettings);

    /// <summary>
    /// Save the provided settings instance into the file.
    /// </summary>
    public void SaveSettings(T settings)
    {
        try
        {
            string settingsSerialized = JsonSerializer.Serialize(settings, _jsonSerializerOptions);
            File.WriteAllText(_settingsPath, settingsSerialized);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.Message);
        }
    }
}