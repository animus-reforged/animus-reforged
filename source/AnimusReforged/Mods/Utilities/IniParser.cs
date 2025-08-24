namespace AnimusReforged.Mods.Utilities;

public class IniParser
{
    // Variables
    private readonly string _iniPath;
    private readonly Dictionary<string, Dictionary<string, string>> _iniData = new Dictionary<string, Dictionary<string, string>>();

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
            if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";"))
            {
                Logger.Debug("Skipping line");
                continue;
            }

            if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
            {
                currentSection = trimmedLine.Trim('[', ']');
                Logger.Debug($"Found section: {currentSection}");
                if (!_iniData.ContainsKey(currentSection))
                {
                    _iniData.Add(currentSection, new Dictionary<string, string>());
                }
            }
            else if (trimmedLine.Contains("="))
            {
                string[] valueLine = trimmedLine.Split('=');
                Logger.Debug($"Found value: {valueLine[0]}={valueLine[1]}");
                if (!_iniData.ContainsKey(currentSection))
                {
                    _iniData.Add(currentSection, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                }
                _iniData[currentSection][valueLine[0]] = valueLine[1];
            }
        }
    }

    public void Save()
    {
        List<string> lines = new List<string>();

        foreach (var section in _iniData)
        {
            lines.Add($"[{section.Key}]");
            foreach (var kvp in section.Value)
            {
                lines.Add($"{kvp.Key}={kvp.Value}");
            }
            lines.Add(""); // blank line between sections
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