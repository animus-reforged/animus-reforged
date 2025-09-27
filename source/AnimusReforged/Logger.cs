using AnimusReforged.Paths;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace AnimusReforged;

/// <summary>
/// Customized Serilog Logger
/// </summary>
public static class Logger
{
    // Variables
    private static ILogger? _logger { get; set; }

    private static readonly LoggingLevelSwitch _levelSwitch = new LoggingLevelSwitch(LogEventLevel.Verbose);
    // Functions
    /// <summary>
    /// Initializes logger
    /// </summary>
    public static void Initialize(bool showConsole = false)
    {
        if (showConsole)
        {
            Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            Console.SetError(new StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
            Console.SetIn(new StreamReader(Console.OpenStandardInput()));
        }
        
        // Initialize Serilog with the configuration
        _logger = new LoggerConfiguration()
            // Set a minimum log level
            .MinimumLevel.ControlledBy(_levelSwitch)
            .WriteTo.Console()
            .WriteTo
            .File(path: AppPaths.LogFile, rollingInterval: RollingInterval.Infinite, fileSizeLimitBytes: 10 * 1024 * 1024, rollOnFileSizeLimit: true, shared: true, buffered: false)
            .CreateLogger();

        Log.Logger = _logger;
    }

    /// <summary>
    /// Writes an informational log message
    /// </summary>
    public static void Info(string message, params object[] propertyValues)
    {
        _logger?.Information(message, propertyValues);
    }

    /// <summary>
    /// Writes a debug log message
    /// </summary>
    public static void Debug(string message, params object[] propertyValues)
    {
#if DEBUG
        _logger?.Debug(message, propertyValues);
#endif
    }

    /// <summary>
    /// Writes a warning log message
    /// </summary>
    public static void Warning(string message, params object[] propertyValues)
    {
        _logger?.Warning(message, propertyValues);
    }

    /// <summary>
    /// Writes an error log message
    /// </summary>
    public static void Error(string message, params object[] propertyValues)
    {
        _logger?.Error(message, propertyValues);
    }

    /// <summary>
    /// Writes an error log message with an exception
    /// </summary>
    public static void Error(Exception ex, string? message = null)
    {
        _logger?.Error(ex, message ?? ex.Message);
    }

    /// <summary>
    /// Changes the level of debugging
    /// </summary>
    /// <param name="level"></param>
    public static void SetMinimumLevel(LogEventLevel level)
    {
        _levelSwitch.MinimumLevel = level;
    }

    public static void LogExceptionDetails(Exception ex)
    {
        Error("=== Exception Details ===");
        Error($"Exception Type: {ex.GetType().FullName}");
        Error($"Exception Message: {ex.Message}");
        Error($"Exception Source: {ex.Source}");
        Error($"Exception StackTrace:\n{ex.StackTrace}");
        Error($"Exception TargetSite:\n{ex.TargetSite}");

        // Logging inner exceptions
        Exception? innerEx = ex.InnerException;
        int depth = 1;
        while (innerEx != null)
        {
            Error("Inner Exception " + depth + ": ", innerEx.Message);
            depth++;
            innerEx = innerEx.InnerException;
        }
        Error("=== End Exception Details ===");
    }

    /// <summary>
    /// Closes and flushes the logger
    /// </summary>
    public static void Shutdown()
    {
        Log.CloseAndFlush();
    }
}