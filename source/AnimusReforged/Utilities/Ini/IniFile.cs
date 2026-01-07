using System.Globalization;
using AnimusReforged.Logging;

namespace AnimusReforged.Utilities.Ini;

/// <summary>
/// Provides methods for reading, writing, and manipulating INI configuration files.
/// Preserves the original file structure including comments, empty lines, and formatting.
/// </summary>
public class IniFile
{
    private readonly string _iniPath;
    private readonly Dictionary<string, Dictionary<string, string>> _iniData = new Dictionary<string, Dictionary<string, string>>();
    private readonly List<IniLine> _iniLines = new List<IniLine>();

    /// <summary>
    /// Initializes a new instance of the <see cref="IniFile"/> class by loading the specified INI file.
    /// </summary>
    /// <param name="iniPath">The path to the INI file to load.</param>
    /// <exception cref="IOException">Thrown when the specified INI file does not exist.</exception>
    public IniFile(string iniPath)
    {
        Logger.Trace<IniFile>($"Initializing IniFile with path: {iniPath}");

        if (string.IsNullOrWhiteSpace(iniPath))
        {
            Logger.Trace<IniFile>("INI file path is null or empty, throwing ArgumentException");
            throw new ArgumentException("INI file path cannot be null or empty.", nameof(iniPath));
        }

        _iniPath = iniPath;

        if (!File.Exists(iniPath))
        {
            Logger.Error<IniFile>($"INI file does not exist: {iniPath}");
            throw new IOException($"Couldn't find the selected INI file: {iniPath}");
        }

        Logger.Trace<IniFile>($"INI file exists at path: {iniPath}, starting parsing");
        Parse(iniPath);
        Logger.Trace<IniFile>($"IniFile initialization completed successfully for path: {iniPath}");
    }

    /// <summary>
    /// Parses the INI file at the specified path and populates the internal data structures.
    /// </summary>
    /// <param name="iniPath">The path to the INI file to parse.</param>
    private void Parse(string iniPath)
    {
        Logger.Trace<IniFile>($"Starting to parse INI file: {iniPath}");
        string[] lines = File.ReadAllLines(iniPath);
        Logger.Trace<IniFile>($"Read {lines.Length} lines from INI file: {iniPath}");

        string currentSection = string.Empty;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            Logger.Debug<IniFile>($"Current line: {trimmedLine}");

            // Handle empty lines
            if (string.IsNullOrEmpty(trimmedLine))
            {
                _iniLines.Add(new IniLine(IniLineType.Empty, line));
                Logger.Debug<IniFile>("Found empty line");
                continue;
            }

            // Handle comments
            if (trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
            {
                _iniLines.Add(new IniLine(IniLineType.Comment, line));
                Logger.Debug<IniFile>("Found comment line");
                continue;
            }

            // Handle sections
            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                currentSection = trimmedLine.Trim('[', ']');
                _iniLines.Add(new IniLine(IniLineType.Section, line, currentSection));
                Logger.Debug<IniFile>($"Found section: {currentSection}");

                if (!_iniData.ContainsKey(currentSection))
                {
                    _iniData[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    Logger.Trace<IniFile>($"Created new dictionary for section: {currentSection}");
                }
                else
                {
                    Logger.Trace<IniFile>($"Section already exists: {currentSection}");
                }
            }
            // Handle key-value pairs
            else if (trimmedLine.Contains("="))
            {
                int separatorIndex = trimmedLine.IndexOf('=');
                string key = trimmedLine.Substring(0, separatorIndex).Trim();
                string value = trimmedLine.Substring(separatorIndex + 1).Trim();

                _iniLines.Add(new IniLine(IniLineType.KeyValue, line, currentSection, key, value));
                Logger.Debug<IniFile>($"Found key-value pair: {key}={value} in section: {currentSection}");

                if (!_iniData.ContainsKey(currentSection))
                {
                    _iniData[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    Logger.Trace<IniFile>($"Created new dictionary for section: {currentSection}");
                }
                _iniData[currentSection][key] = value;
                Logger.Trace<IniFile>($"Added key-value pair to section {currentSection}: {key}={value}");
            }
            else
            {
                // Handle malformed lines as comments to preserve them
                _iniLines.Add(new IniLine(IniLineType.Comment, line));
                Logger.Trace<IniFile>($"Found malformed line, treating as comment: {trimmedLine}");
            }
        }
        Logger.Trace<IniFile>($"Completed parsing INI file: {iniPath}, total lines processed: {lines.Length}");
    }

    /// <summary>
    /// Saves the current INI data back to the file, preserving the original structure and comments.
    /// </summary>
    /// <exception cref="IOException">Thrown when the file cannot be written to.</exception>
    public void Save()
    {
        Logger.Trace<IniFile>($"Starting to save INI file: {_iniPath}");

        try
        {
            // Create a dictionary to track which keys have been written to avoid duplication
            Dictionary<string, HashSet<string>> writtenKeys = new Dictionary<string, HashSet<string>>();

            List<string> lines = new List<string>();

            foreach (IniLine iniLine in _iniLines)
            {
                switch (iniLine.Type)
                {
                    case IniLineType.Comment:
                    case IniLineType.Empty:
                        lines.Add(iniLine.Content);
                        break;

                    case IniLineType.Section:
                        lines.Add($"[{iniLine.Section}]");
                        // Initialize the set for tracking keys in this section
                        writtenKeys[iniLine.Section] = new HashSet<string>();
                        Logger.Trace<IniFile>($"Added section line: [{iniLine.Section}]");
                        break;

                    case IniLineType.KeyValue:
                        // Check if the key still exists in our data structure
                        if (_iniData.TryGetValue(iniLine.Section, out Dictionary<string, string>? section) && section.TryGetValue(iniLine.Key, out string? currentValue))
                        {
                            lines.Add($"{iniLine.Key}={currentValue}");
                            Logger.Trace<IniFile>($"Added key-value line: {iniLine.Key}={currentValue}");
                            // Mark this key as written
                            if (writtenKeys.ContainsKey(iniLine.Section))
                            {
                                writtenKeys[iniLine.Section].Add(iniLine.Key);
                            }
                        }
                        else
                        {
                            Logger.Trace<IniFile>($"Skipping key-value line (key removed): {iniLine.Key}={iniLine.Value}");
                        }
                        // If the key was removed, don't add it back
                        break;
                    default:
                        throw new NotImplementedException("Unknown line type");
                }
            }

            // Add any new sections/keys that weren't in the original file
            foreach (KeyValuePair<string, Dictionary<string, string>> sectionKvp in _iniData)
            {
                bool sectionExistsInOriginal = writtenKeys.ContainsKey(sectionKvp.Key);

                if (!sectionExistsInOriginal)
                {
                    lines.Add($"[{sectionKvp.Key}]");
                    Logger.Trace<IniFile>($"Added new section: [{sectionKvp.Key}]");

                    foreach (KeyValuePair<string, string> keyKvp in sectionKvp.Value)
                    {
                        lines.Add($"{keyKvp.Key}={keyKvp.Value}");
                        Logger.Trace<IniFile>($"Added new key-value pair: {keyKvp.Key}={keyKvp.Value}");
                    }
                    lines.Add(""); // blank line between sections
                }
                else
                {
                    // Add any new keys in existing sections that weren't written yet
                    foreach (KeyValuePair<string, string> keyKvp in sectionKvp.Value)
                    {
                        if (!writtenKeys[sectionKvp.Key].Contains(keyKvp.Key))
                        {
                            lines.Add($"{keyKvp.Key}={keyKvp.Value}");
                            Logger.Trace<IniFile>($"Added new key-value pair to existing section: {keyKvp.Key}={keyKvp.Value}");
                        }
                    }
                }
            }

            File.WriteAllLines(_iniPath, lines);
            Logger.Info<IniFile>($"Successfully saved INI file: {_iniPath} with {lines.Count} lines");
            Logger.Trace<IniFile>($"Save operation completed for INI file: {_iniPath}");
        }
        catch (UnauthorizedAccessException)
        {
            Logger.Error<IniFile>($"Access denied when writing to INI file: {_iniPath}");
            throw;
        }
        catch (DirectoryNotFoundException)
        {
            Logger.Error<IniFile>($"Directory not found when writing to INI file: {_iniPath}");
            throw;
        }
        catch (IOException ioe)
        {
            Logger.Error<IniFile>($"IO error when writing to INI file: {_iniPath}, Error: {ioe.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error<IniFile>($"Unexpected error when writing to INI file: {_iniPath}, Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Gets a string value from the specified section and key.
    /// </summary>
    /// <param name="section">The section name.</param>
    /// <param name="key">The key name.</param>
    /// <param name="defaultValue">The default value to return if the key is not found.</param>
    /// <returns>The value associated with the specified section and key, or the default value if not found.</returns>
    public string Get(string section, string key, string defaultValue = "")
    {
        Logger.Trace<IniFile>($"Getting value for key '{key}' in section '{section}' with default value '{defaultValue}'");

        if (string.IsNullOrWhiteSpace(section))
        {
            Logger.Trace<IniFile>("Section name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Section name cannot be null or empty.", nameof(section));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            Logger.Trace<IniFile>("Key name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Key name cannot be null or empty.", nameof(key));
        }

        if (_iniData.TryGetValue(section, out Dictionary<string, string>? sec) && sec.TryGetValue(key, out string? value))
        {
            Logger.Trace<IniFile>($"Found value for key '{key}' in section '{section}': {value}");
            return value;
        }
        else
        {
            Logger.Trace<IniFile>($"Key '{key}' not found in section '{section}', returning default value: {defaultValue}");
            return defaultValue;
        }
    }

    /// <summary>
    /// Gets an integer value from the specified section and key.
    /// </summary>
    /// <param name="section">The section name.</param>
    /// <param name="key">The key name.</param>
    /// <param name="defaultValue">The default value to return if the key is not found or cannot be parsed as an integer.</param>
    /// <returns>The integer value associated with the specified section and key, or the default value if not found or invalid.</returns>
    public int GetInt(string section, string key, int defaultValue = 0)
    {
        Logger.Trace<IniFile>($"Getting integer value for key '{key}' in section '{section}' with default value '{defaultValue}'");

        if (string.IsNullOrWhiteSpace(section))
        {
            Logger.Trace<IniFile>("Section name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Section name cannot be null or empty.", nameof(section));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            Logger.Trace<IniFile>("Key name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Key name cannot be null or empty.", nameof(key));
        }

        string stringValue = Get(section, key);
        if (int.TryParse(stringValue, out int result))
        {
            Logger.Trace<IniFile>($"Successfully parsed integer value for key '{key}' in section '{section}': {result}");
            return result;
        }
        else
        {
            Logger.Trace<IniFile>($"Failed to parse integer value for key '{key}' in section '{section}' (value: '{stringValue}'), returning default: {defaultValue}");
            return defaultValue;
        }
    }

    /// <summary>
    /// Gets a boolean value from the specified section and key.
    /// </summary>
    /// <param name="section">The section name.</param>
    /// <param name="key">The key name.</param>
    /// <param name="defaultValue">The default value to return if the key is not found or cannot be parsed as a boolean.</param>
    /// <returns>True if the value is "1", false otherwise (including if not found or invalid).</returns>
    public bool GetBool(string section, string key, bool defaultValue = false)
    {
        Logger.Trace<IniFile>($"Getting boolean value for key '{key}' in section '{section}' with default value '{defaultValue}'");

        if (string.IsNullOrWhiteSpace(section))
        {
            Logger.Trace<IniFile>("Section name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Section name cannot be null or empty.", nameof(section));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            Logger.Trace<IniFile>("Key name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Key name cannot be null or empty.", nameof(key));
        }

        string stringValue = Get(section, key);
        if (int.TryParse(stringValue, out int result))
        {
            bool boolResult = result == 1;
            Logger.Trace<IniFile>($"Successfully parsed boolean value for key '{key}' in section '{section}': {boolResult} (from integer {result})");
            return boolResult;
        }
        else
        {
            Logger.Trace<IniFile>($"Failed to parse boolean value for key '{key}' in section '{section}' (value: '{stringValue}'), returning default: {defaultValue}");
            return defaultValue;
        }
    }

    /// <summary>
    /// Gets a double value from the specified section and key.
    /// </summary>
    /// <param name="section">The section name.</param>
    /// <param name="key">The key name.</param>
    /// <param name="defaultValue">The default value to return if the key is not found or cannot be parsed as a double.</param>
    /// <returns>The double value associated with the specified section and key, or the default value if not found or invalid.</returns>
    public double GetDouble(string section, string key, double defaultValue = 0.0)
    {
        Logger.Trace<IniFile>($"Getting double value for key '{key}' in section '{section}' with default value '{defaultValue}'");

        if (string.IsNullOrWhiteSpace(section))
        {
            Logger.Trace<IniFile>("Section name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Section name cannot be null or empty.", nameof(section));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            Logger.Trace<IniFile>("Key name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Key name cannot be null or empty.", nameof(key));
        }

        string stringValue = Get(section, key);
        if (double.TryParse(stringValue, out double result))
        {
            Logger.Trace<IniFile>($"Successfully parsed double value for key '{key}' in section '{section}': {result}");
            return result;
        }
        else
        {
            Logger.Trace<IniFile>($"Failed to parse double value for key '{key}' in section '{section}' (value: '{stringValue}'), returning default: {defaultValue}");
            return defaultValue;
        }
    }

    /// <summary>
    /// Sets a string value for the specified section and key.
    /// </summary>
    /// <param name="section">The section name.</param>
    /// <param name="key">The key name.</param>
    /// <param name="value">The value to set.</param>
    public void Set(string section, string key, string? value)
    {
        Logger.Trace<IniFile>($"Setting value for key '{key}' in section '{section}' to: {value ?? "null"}");

        if (string.IsNullOrWhiteSpace(section))
        {
            Logger.Trace<IniFile>("Section name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Section name cannot be null or empty.", nameof(section));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            Logger.Trace<IniFile>("Key name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Key name cannot be null or empty.", nameof(key));
        }

        if (!_iniData.ContainsKey(section))
        {
            _iniData[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Logger.Trace<IniFile>($"Created new dictionary for section: {section}");
        }
        _iniData[section][key] = value ?? string.Empty;
        Logger.Trace<IniFile>($"Successfully set value for key '{key}' in section '{section}' to: {value ?? "null"}");
    }

    /// <summary>
    /// Sets an integer value for the specified section and key.
    /// </summary>
    /// <param name="section">The section name.</param>
    /// <param name="key">The key name.</param>
    /// <param name="value">The value to set.</param>
    public void Set(string section, string key, int value)
    {
        Logger.Trace<IniFile>($"Setting integer value for key '{key}' in section '{section}' to: {value}");
        Set(section, key, value.ToString());
    }

    /// <summary>
    /// Sets a boolean value for the specified section and key.
    /// </summary>
    /// <param name="section">The section name.</param>
    /// <param name="key">The key name.</param>
    /// <param name="value">The value to set (true becomes "1", false becomes "0").</param>
    public void Set(string section, string key, bool value)
    {
        Logger.Trace<IniFile>($"Setting boolean value for key '{key}' in section '{section}' to: {value}");
        Set(section, key, value ? "1" : "0");
    }

    /// <summary>
    /// Sets a double value for the specified section and key.
    /// </summary>
    /// <param name="section">The section name.</param>
    /// <param name="key">The key name.</param>
    /// <param name="value">The value to set.</param>
    public void Set(string section, string key, double value)
    {
        Logger.Trace<IniFile>($"Setting double value for key '{key}' in section '{section}' to: {value}");
        Set(section, key, value.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Checks if a section exists in the INI file.
    /// </summary>
    /// <param name="section">The section name to check.</param>
    /// <returns>True if the section exists, false otherwise.</returns>
    public bool HasSection(string section)
    {
        Logger.Trace<IniFile>($"Checking if section '{section}' exists");

        if (string.IsNullOrWhiteSpace(section))
        {
            Logger.Trace<IniFile>("Section name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Section name cannot be null or empty.", nameof(section));
        }

        bool exists = _iniData.ContainsKey(section);
        Logger.Trace<IniFile>($"Section '{section}' exists: {exists}");
        return exists;
    }

    /// <summary>
    /// Checks if a key exists in the specified section.
    /// </summary>
    /// <param name="section">The section name.</param>
    /// <param name="key">The key name to check.</param>
    /// <returns>True if the key exists in the section, false otherwise.</returns>
    public bool HasKey(string section, string key)
    {
        Logger.Trace<IniFile>($"Checking if key '{key}' exists in section '{section}'");

        if (string.IsNullOrWhiteSpace(section))
        {
            Logger.Trace<IniFile>("Section name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Section name cannot be null or empty.", nameof(section));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            Logger.Trace<IniFile>("Key name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Key name cannot be null or empty.", nameof(key));
        }

        bool exists = _iniData.TryGetValue(section, out var sectionDict) && sectionDict.ContainsKey(key);
        Logger.Trace<IniFile>($"Key '{key}' exists in section '{section}': {exists}");
        return exists;
    }

    /// <summary>
    /// Removes a key from the specified section.
    /// </summary>
    /// <param name="section">The section name.</param>
    /// <param name="key">The key name to remove.</param>
    /// <returns>True if the key was removed, false if it didn't exist.</returns>
    public bool RemoveKey(string section, string key)
    {
        Logger.Trace<IniFile>($"Removing key '{key}' from section '{section}'");

        if (string.IsNullOrWhiteSpace(section))
        {
            Logger.Trace<IniFile>("Section name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Section name cannot be null or empty.", nameof(section));
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            Logger.Trace<IniFile>("Key name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Key name cannot be null or empty.", nameof(key));
        }

        if (_iniData.TryGetValue(section, out Dictionary<string, string>? sectionDict))
        {
            bool result = sectionDict.Remove(key);
            Logger.Trace<IniFile>($"Key removal result for '{key}' in section '{section}': {result}");

            // Also remove from the line structure if it exists there
            int removedLines = 0;
            for (int i = _iniLines.Count - 1; i >= 0; i--)
            {
                if (_iniLines[i].Type == IniLineType.KeyValue && _iniLines[i].Section == section && _iniLines[i].Key == key)
                {
                    _iniLines.RemoveAt(i);
                    removedLines++;
                }
            }
            Logger.Trace<IniFile>($"Removed {removedLines} line(s) from the line structure for key '{key}' in section '{section}'");

            return result;
        }

        Logger.Trace<IniFile>($"Section '{section}' does not exist, key '{key}' removal failed");
        return false;
    }

    /// <summary>
    /// Removes an entire section from the INI file.
    /// </summary>
    /// <param name="section">The section name to remove.</param>
    /// <returns>True if the section was removed, false if it didn't exist.</returns>
    public bool RemoveSection(string section)
    {
        Logger.Trace<IniFile>($"Removing section '{section}'");

        if (string.IsNullOrWhiteSpace(section))
        {
            Logger.Trace<IniFile>("Section name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Section name cannot be null or empty.", nameof(section));
        }

        bool result = _iniData.Remove(section);
        Logger.Trace<IniFile>($"Section removal result for '{section}': {result}");

        // Also remove from the line structure if it exists there
        int removedLines = 0;
        for (int i = _iniLines.Count - 1; i >= 0; i--)
        {
            if (_iniLines[i].Type == IniLineType.Section && _iniLines[i].Section == section)
            {
                // Remove the section line and any subsequent key-value lines until the next section
                _iniLines.RemoveAt(i);
                removedLines++;

                // Remove any subsequent key-value lines that belong to this section
                int j = i;
                while (j < _iniLines.Count)
                {
                    if (_iniLines[j].Type == IniLineType.KeyValue && _iniLines[j].Section == section)
                    {
                        _iniLines.RemoveAt(j);
                        removedLines++;
                    }
                    else if (_iniLines[j].Type == IniLineType.Section)
                    {
                        // We've reached the next section, stop removing
                        break;
                    }
                    else
                    {
                        j++;
                    }
                }
            }
        }
        Logger.Trace<IniFile>($"Removed {removedLines} line(s) from the line structure for section '{section}'");

        return result;
    }

    /// <summary>
    /// Gets all section names in the INI file.
    /// </summary>
    /// <returns>An array of section names.</returns>
    public string[] GetSections()
    {
        Logger.Trace<IniFile>($"Getting all sections, total count: {_iniData.Keys.Count}");
        string[] sections = _iniData.Keys.ToArray();
        Logger.Trace<IniFile>($"Retrieved sections: [{string.Join(", ", sections)}]");
        return sections;
    }

    /// <summary>
    /// Gets all key names in the specified section.
    /// </summary>
    /// <param name="section">The section name.</param>
    /// <returns>An array of key names in the section, or an empty array if the section doesn't exist.</returns>
    public string[] GetKeys(string section)
    {
        Logger.Trace<IniFile>($"Getting all keys in section '{section}'");

        if (string.IsNullOrWhiteSpace(section))
        {
            Logger.Trace<IniFile>("Section name is null or empty, throwing ArgumentException");
            throw new ArgumentException("Section name cannot be null or empty.", nameof(section));
        }

        if (_iniData.TryGetValue(section, out Dictionary<string, string>? sectionDict))
        {
            string[] keys = sectionDict.Keys.ToArray();
            Logger.Trace<IniFile>($"Retrieved {keys.Length} keys from section '{section}': [{string.Join(", ", keys)}]");
            return keys;
        }

        Logger.Trace<IniFile>($"Section '{section}' does not exist, returning empty array");
        return [];
    }
}