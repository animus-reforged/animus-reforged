namespace AnimusReforged.Utilities.Ini;

/// <summary>
/// Represents a single line in an INI file with its type and associated data.
/// </summary>
internal class IniLine
{
    /// <summary>
    /// Gets or sets the type of the INI line.
    /// </summary>
    public IniLineType Type { get; set; }

    /// <summary>
    /// Gets or sets the original content of the line.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Gets or sets the section name for section and key-value lines.
    /// </summary>
    public string Section { get; set; }

    /// <summary>
    /// Gets or sets the key name for key-value lines.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the value for key-value lines.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IniLine"/> class.
    /// </summary>
    /// <param name="type">The type of the INI line.</param>
    /// <param name="content">The original content of the line.</param>
    /// <param name="section">The section name for section and key-value lines.</param>
    /// <param name="key">The key name for key-value lines.</param>
    /// <param name="value">The value for key-value lines.</param>
    public IniLine(IniLineType type, string content, string section = "", string key = "", string value = "")
    {
        Type = type;
        Content = content;
        Section = section;
        Key = key;
        Value = value;
    }
}