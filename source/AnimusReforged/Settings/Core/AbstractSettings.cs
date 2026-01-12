using System.Text.Json;
using System.Text.Json.Serialization;
using AnimusReforged.Logging;
using AnimusReforged.Utilities;

namespace AnimusReforged.Settings.Core;

/// <summary>
/// Provides a base implementation for settings management with JSON serialization and file-based persistence.
/// </summary>
/// <typeparam name="T">The type of settings object to manage, must be a class with a parameterless constructor.</typeparam>
public abstract class AbstractSettings<T> : ISettingsService<T> where T : class, new()
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly string _settingsPath = FilePaths.SettingsFile;
    private T? _settings;
    private readonly Lock _lock = new Lock();
    private bool _settingsLoaded = false;
    private T? _lastSavedSettings;

    /// <summary>
    /// Occurs when settings have been changed.
    /// </summary>
    public event EventHandler? SettingsChanged;

    /// <summary>
    /// Gets the default settings instance.
    /// </summary>
    protected virtual T DefaultSettings => new T();

    /// <summary>
    /// Raises the SettingsChanged event.
    /// </summary>
    protected virtual void OnSettingsChanged()
    {
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Gets the currently loaded settings instance, loading them if necessary.
    /// </summary>
    public T Settings
    {
        get
        {
            if (!_settingsLoaded)
            {
                LoadSettings();
            }
            return _settings!;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractSettings{T}"/> class with default JSON serialization options.
    /// </summary>
    protected AbstractSettings()
    {
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters = {new JsonStringEnumConverter()},
            WriteIndented = true
        };
        
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath) ?? string.Empty);
    }

    /// <summary>
    /// Loads settings from the persistent storage file. If the file does not exist or fails to load, the default settings are returned.
    /// </summary>
    /// <returns>The loaded settings instance, or default settings if loading fails.</returns>
    public virtual T LoadSettings()
    {
        lock (_lock)
        {
            try
            {
                if (!File.Exists(_settingsPath))
                {
                    // Save the default settings if the file is missing
                    T defaultSettings = DefaultSettings;
                    SaveSettings(defaultSettings);
                    _settings = defaultSettings;
                    _settingsLoaded = true;
                    _lastSavedSettings = CloneSettings(defaultSettings);
                    return _settings;
                }

                string settingsSerialized = File.ReadAllText(_settingsPath);
                T? settings = JsonSerializer.Deserialize<T>(settingsSerialized, _jsonSerializerOptions);

                if (settings != null)
                {
                    _settings = settings;
                    _settingsLoaded = true;
                    _lastSavedSettings = CloneSettings(settings);
                    return _settings;
                }
                else
                {
                    T defaultSettings = DefaultSettings;
                    _settings = defaultSettings;
                    _settingsLoaded = true;
                    _lastSavedSettings = CloneSettings(defaultSettings);
                    return _settings;
                }
            }
            catch (JsonException ex)
            {
                Logger.Error<AbstractSettings<T>>($"JSON deserialization error while loading settings from {_settingsPath}");
                Logger.LogExceptionDetails<AbstractSettings<T>>(ex);
                T defaultSettings = DefaultSettings;
                _settings = defaultSettings;
                _settingsLoaded = true;
                _lastSavedSettings = CloneSettings(defaultSettings);
                return _settings;
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error<AbstractSettings<T>>($"Access denied while loading settings from {_settingsPath}");
                Logger.LogExceptionDetails<AbstractSettings<T>>(ex);
                T defaultSettings = DefaultSettings;
                _settings = defaultSettings;
                _settingsLoaded = true;
                _lastSavedSettings = CloneSettings(defaultSettings);
                return _settings;
            }
            catch (Exception ex)
            {
                Logger.Error<AbstractSettings<T>>($"There was an unexpected error loading settings from {_settingsPath}");
                Logger.LogExceptionDetails<AbstractSettings<T>>(ex);
                T defaultSettings = DefaultSettings;
                _settings = defaultSettings;
                _settingsLoaded = true;
                _lastSavedSettings = CloneSettings(defaultSettings);
                return _settings;
            }
        }
    }

    /// <summary>
    /// Asynchronously loads settings from the persistent storage.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded settings instance, or default settings if loading fails.</returns>
    public async virtual Task<T> LoadSettingsAsync()
    {
        return await Task.Run(LoadSettings);
    }

    /// <summary>
    /// Saves the current settings instance to persistent storage.
    /// </summary>
    public void SaveSettings()
    {
        T settingsToSave = _settings ?? DefaultSettings;
        SaveSettings(settingsToSave);
    }

    /// <summary>
    /// Asynchronously saves the current settings instance to persistent storage.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SaveSettingsAsync()
    {
        T settingsToSave = _settings ?? DefaultSettings;
        await SaveSettingsAsync(settingsToSave);
    }

    /// <summary>
    /// Saves the provided settings instance to persistent storage.
    /// </summary>
    /// <param name="settings">The settings instance to save.</param>
    public void SaveSettings(T settings)
    {
        lock (_lock)
        {
            try
            {
                // Check if settings have actually changed to avoid unnecessary writes
                if (AreSettingsEqual(settings, _lastSavedSettings))
                {
                    return; // No changes to save
                }

                string settingsSerialized = JsonSerializer.Serialize(settings, _jsonSerializerOptions);

                // Use WriteAllTextAsync to ensure atomic write operation
                File.WriteAllText(_settingsPath, settingsSerialized);

                _settings = settings;
                _settingsLoaded = true;
                _lastSavedSettings = CloneSettings(settings);

                // Trigger the settings changed event
                OnSettingsChanged();
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error<AbstractSettings<T>>($"Access denied while saving settings to {_settingsPath}");
                Logger.LogExceptionDetails<AbstractSettings<T>>(ex);
            }
            catch (Exception ex)
            {
                Logger.Error<AbstractSettings<T>>($"There was an unexpected error saving settings to {_settingsPath}");
                Logger.LogExceptionDetails<AbstractSettings<T>>(ex);
            }
        }
    }

    /// <summary>
    /// Asynchronously saves the provided settings instance to persistent storage.
    /// </summary>
    /// <param name="settings">The settings instance to save.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task SaveSettingsAsync(T settings)
    {
        await Task.Run(() => SaveSettings(settings));
    }

    /// <summary>
    /// Compares two settings instances for equality to determine if a save operation is necessary.
    /// </summary>
    /// <param name="settings1">The first settings instance to compare.</param>
    /// <param name="settings2">The second settings instance to compare.</param>
    /// <returns>True if the settings are equal, false otherwise.</returns>
    private bool AreSettingsEqual(T? settings1, T? settings2)
    {
        if (settings1 == null && settings2 == null)
        {
            return true;
        }
        if (settings1 == null || settings2 == null)
        {
            return false;
        }

        string json1 = JsonSerializer.Serialize(settings1, _jsonSerializerOptions);
        string json2 = JsonSerializer.Serialize(settings2, _jsonSerializerOptions);

        return json1.Equals(json2);
    }

    /// <summary>
    /// Creates a deep clone of the settings object by serializing and deserializing it.
    /// </summary>
    /// <param name="settings">The settings object to clone.</param>
    /// <returns>A cloned copy of the settings object.</returns>
    private T CloneSettings(T settings)
    {
        string settingsSerialized = JsonSerializer.Serialize(settings, _jsonSerializerOptions);
        return JsonSerializer.Deserialize<T>(settingsSerialized, _jsonSerializerOptions) ?? DefaultSettings;
    }
}