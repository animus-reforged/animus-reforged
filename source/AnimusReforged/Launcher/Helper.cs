using System.Diagnostics;
using AnimusReforged.Logging;
using AnimusReforged.Utilities;

namespace AnimusReforged.Launcher;

/// <summary>
/// Provides helper methods for launching external processes such as uMod and the game executable.
/// </summary>
public class Helper
{
    /// <summary>
    /// Launches the uMod process with minimized window style.
    /// </summary>
    /// <returns>The Process object representing the launched uMod instance.</returns>
    public static Process LaunchuMod()
    {
        try
        {
            Process uMod = new Process();
            uMod.StartInfo.FileName = FilePaths.UModExecutable;
            uMod.StartInfo.WorkingDirectory = FilePaths.UModLocation;
            uMod.StartInfo.UseShellExecute = true;
            uMod.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;

            bool started = uMod.Start();
            if (!started)
            {
                Logger.Error<Helper>($"Failed to start uMod process: {FilePaths.UModExecutable}");
                throw new InvalidOperationException($"Could not start uMod process: {FilePaths.UModExecutable}");
            }

            return uMod;
        }
        catch (Exception ex)
        {
            Logger.Error<Helper>($"Error launching uMod: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Launches a game executable with the application's base directory as the working directory.
    /// </summary>
    /// <param name="executablePath">The path to the game executable to launch.</param>
    /// <returns>The Process object representing the launched game instance.</returns>
    public static Process LaunchGame(string executablePath)
    {
        try
        {
            if (string.IsNullOrEmpty(executablePath))
            {
                Logger.Error<Helper>("Game executable path is null or empty");
                throw new ArgumentException("Game executable path cannot be null or empty", nameof(executablePath));
            }

            if (!File.Exists(executablePath))
            {
                Logger.Error<Helper>($"Game executable does not exist: {executablePath}");
                throw new FileNotFoundException($"Game executable not found: {executablePath}");
            }

            Process game = new Process();
            game.StartInfo.FileName = executablePath;
            game.StartInfo.WorkingDirectory = AbsolutePath.BaseDirectory();
            game.StartInfo.UseShellExecute = true;

            bool started = game.Start();
            if (!started)
            {
                Logger.Error<Helper>($"Failed to start game process: {executablePath}");
                throw new InvalidOperationException($"Could not start game process: {executablePath}");
            }

            return game;
        }
        catch (Exception ex)
        {
            Logger.Error<Helper>($"Error launching game: {ex.Message}");
            throw;
        }
    }
}