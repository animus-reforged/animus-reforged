namespace AnimusReforged.Utilities.Ini;

/// <summary>
/// Defines the types of lines that can be found in an INI file.
/// </summary>
internal enum IniLineType
{
    /// <summary>
    /// Represents a comment line that starts with ; or #.
    /// </summary>
    Comment,

    /// <summary>
    /// Represents a section header line enclosed in square brackets [ ].
    /// </summary>
    Section,

    /// <summary>
    /// Represents a key-value pair line with an equals sign (=).
    /// </summary>
    KeyValue,

    /// <summary>
    /// Represents an empty or whitespace-only line.
    /// </summary>
    Empty
}