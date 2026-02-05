using NLog;

namespace AnimusReforged.Logging;

/// <summary>
/// Helper class for converting between integer values and NLog LogLevel objects
/// </summary>
public class LogLevelHelper
{
    /// <summary>
    /// Converts an integer value to the corresponding NLog LogLevel
    /// </summary>
    /// <param name="logLevel">Integer value representing log level (0-6)</param>
    /// <returns>NLog LogLevel object</returns>
    public static LogLevel FromInt(int? logLevel)
    {
        return logLevel switch
        {
            6 => LogLevel.Trace,
            5 => LogLevel.Debug,
            4 => LogLevel.Info,
            3 => LogLevel.Warn,
            2 => LogLevel.Error,
            1 => LogLevel.Fatal,
            0 => LogLevel.Off,
            _ => LogLevel.Info
        };
    }

    /// <summary>
    /// Converts an NLog LogLevel to its string representation
    /// </summary>
    /// <param name="level">NLog LogLevel object</param>
    /// <returns>String representation of the log level</returns>
    public static string ToString(LogLevel level)
    {
        return level.Name;
    }
}