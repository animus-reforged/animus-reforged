namespace AnimusReforged.Mods.Utilities;

internal class IniLine
{
    public IniLineType Type { get; set; }
    public string Content { get; set; }
    public string Section { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }

    public IniLine(IniLineType type, string content, string section = "", string key = "", string value = "")
    {
        Type = type;
        Content = content;
        Section = section;
        Key = key;
        Value = value;
    }
}

internal enum IniLineType
{
    Comment,
    Section,
    KeyValue,
    Empty
}

public class IniParser
{
    // Variables
    private readonly string _iniPath;
    private readonly Dictionary<string, Dictionary<string, string>> _iniData = new Dictionary<string, Dictionary<string, string>>();
    private readonly List<IniLine> _iniLines = new List<IniLine>();

    // Constructor
    public IniParser(string iniPath)
    {
        _iniPath = iniPath;

        if (!File.Exists(iniPath))
        {
            Logger.Error(".ini file doesn't exist");
            throw new IOException("Couldn't find the selected .ini file.");
        }

        Parse(iniPath);
    }

    // Methods
    private void Parse(string iniPath)
    {
        string[] lines = File.ReadAllLines(iniPath);
        string currentSection = string.Empty;

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            Logger.Debug($"Current line: {trimmedLine}");

            // Handle empty lines
            if (string.IsNullOrEmpty(trimmedLine))
            {
                _iniLines.Add(new IniLine(IniLineType.Empty, line));
                Logger.Debug("Found empty line");
                continue;
            }

            // Handle comments
            if (trimmedLine.StartsWith(";"))
            {
                _iniLines.Add(new IniLine(IniLineType.Comment, line));
                Logger.Debug("Found comment line");
                continue;
            }

            // Handle sections
            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                currentSection = trimmedLine.Trim('[', ']');
                _iniLines.Add(new IniLine(IniLineType.Section, line, currentSection));
                Logger.Debug($"Found section: {currentSection}");

                if (!_iniData.ContainsKey(currentSection))
                {
                    _iniData.Add(currentSection, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                }
            }
            // Handle key-value pairs
            else if (trimmedLine.Contains("="))
            {
                string[] valueLine = trimmedLine.Split('=', 2); // Split only on first '=' to handle values with '='
                string key = valueLine[0].Trim();
                string value = valueLine.Length > 1 ? valueLine[1].Trim() : "";

                _iniLines.Add(new IniLine(IniLineType.KeyValue, line, currentSection, key, value));
                Logger.Debug($"Found value: {key}={value}");

                if (!_iniData.ContainsKey(currentSection))
                {
                    _iniData.Add(currentSection, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                }
                _iniData[currentSection][key] = value;
            }
            else
            {
                // Handle malformed lines as comments to preserve them
                _iniLines.Add(new IniLine(IniLineType.Comment, line));
                Logger.Debug("Found malformed line, treating as comment");
            }
        }
    }

    public void Save()
    {
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
                    break;

                case IniLineType.KeyValue:
                    // Check if the key still exists in our data structure
                    if (_iniData.TryGetValue(iniLine.Section, out var section) &&
                        section.TryGetValue(iniLine.Key, out var currentValue))
                    {
                        lines.Add($"{iniLine.Key}={currentValue}");
                    }
                    // If the key was removed, don't add it back
                    break;
                default:
                    throw new NotImplementedException("Unknown line type");
            }
        }

        // Add any new sections/keys that weren't in the original file
        foreach (var sectionKvp in _iniData)
        {
            bool sectionExistsInOriginal = _iniLines.Any(line => line.Type == IniLineType.Section && line.Section == sectionKvp.Key);

            if (!sectionExistsInOriginal)
            {
                lines.Add($"[{sectionKvp.Key}]");
                foreach (var keyKvp in sectionKvp.Value)
                {
                    lines.Add($"{keyKvp.Key}={keyKvp.Value}");
                }
                lines.Add(""); // blank line between sections
            }
            else
            {
                // Add any new keys in existing sections
                foreach (var keyKvp in sectionKvp.Value)
                {
                    bool keyExistsInOriginal = _iniLines.Any(line => line.Type == IniLineType.KeyValue && line.Section == sectionKvp.Key && line.Key == keyKvp.Key);

                    if (!keyExistsInOriginal)
                    {
                        // Find the last key-value pair in this section and insert after it
                        int lastKeyIndex = -1;
                        for (int i = _iniLines.Count - 1; i >= 0; i--)
                        {
                            if (_iniLines[i].Type == IniLineType.KeyValue && _iniLines[i].Section == sectionKvp.Key)
                            {
                                lastKeyIndex = i;
                                break;
                            }
                        }

                        if (lastKeyIndex >= 0)
                        {
                            _iniLines.Insert(lastKeyIndex + 1, new IniLine(IniLineType.KeyValue, "", sectionKvp.Key, keyKvp.Key, keyKvp.Value));
                        }
                    }
                }
            }
        }

        File.WriteAllLines(_iniPath, lines);
    }

    public string Get(string section, string key, string defaultValue = "") => _iniData.TryGetValue(section, out var sec) && sec.TryGetValue(key, out var value)
        ? value
        : defaultValue;
    public int GetInt(string section, string key, int defaultValue = 0)
        => int.TryParse(Get(section, key), out var result) ? result : defaultValue;
    public bool GetBool(string section, string key, bool defaultValue = false)
        => int.TryParse(Get(section, key), out var result) ? result == 1 : defaultValue;

    public void Set(string section, string key, string value)
    {
        if (!_iniData.ContainsKey(section))
        {
            _iniData[section] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        _iniData[section][key] = value;
    }
    public void Set(string section, string key, int value) => Set(section, key, value.ToString());
    public void Set(string section, string key, bool value) => Set(section, key, value ? "1" : "0");
}