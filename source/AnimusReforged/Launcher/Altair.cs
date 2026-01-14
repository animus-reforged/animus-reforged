using System.Diagnostics;
using AnimusReforged.Logging;
using AnimusReforged.Utilities;

namespace AnimusReforged.Launcher;

/// <summary>
/// Provides functionality for launching the Assassin's Creed game with optional uMod support.
/// Handles both synchronous and asynchronous launch operations with proper process management.
/// </summary>
public class Altair
{
    /// <summary>
    /// Launches the Assassin's Creed game asynchronously with optional uMod support.
    /// Waits for the game process to exit before closing uMod if enabled.
    /// </summary>
    /// <param name="uModEnabled">Whether to launch uMod alongside the game (defaults to false).</param>
    public static async Task LaunchAsync(bool uModEnabled = false)
    {
        Process? uMod = null;
        Process? game = null;

        try
        {
            if (uModEnabled)
            {
                Logger.Info<Altair>("Launching uMod");
                uMod = Helper.LaunchuMod();
            }

            Logger.Info<Altair>("Launching the game");
            game = Helper.LaunchGame(FilePaths.AltairExecutable);

            Logger.Info<Altair>("Waiting for the game to exit");
            await game.WaitForExitAsync();
            Logger.Info<Altair>("Game exited");
        }
        finally
        {
            // Close uMod after game exits
            if (uModEnabled && uMod != null && !uMod.HasExited)
            {
                Logger.Info<Altair>("Closing uMod");
                uMod.CloseMainWindow();
                uMod.Dispose();
            }

            // Dispose of game process
            game?.Dispose();
        }
    }

    /// <summary>
    /// Launches the Assassin's Creed game synchronously with optional uMod support.
    /// Blocks the calling thread until the game process exits before closing uMod if enabled.
    /// </summary>
    /// <param name="uModEnabled">Whether to launch uMod alongside the game (defaults to false).</param>
    public static void Launch(bool uModEnabled = false)
    {
        Process? uMod = null;
        Process? game = null;

        try
        {
            if (uModEnabled)
            {
                Logger.Info<Altair>("Launching uMod");
                uMod = Helper.LaunchuMod();
            }

            Logger.Info<Altair>("Launching the game");
            game = Helper.LaunchGame(FilePaths.AltairExecutable);

            Logger.Info<Altair>("Waiting for the game to exit");
            game.WaitForExit();
            Logger.Info<Altair>("Game exited");
        }
        finally
        {
            // Close uMod after game exits
            if (uModEnabled && uMod != null && !uMod.HasExited)
            {
                Logger.Info<Altair>("Closing uMod");
                uMod.CloseMainWindow();
                uMod.Dispose();
            }

            // Dispose of game process
            game?.Dispose();
        }
    }
}