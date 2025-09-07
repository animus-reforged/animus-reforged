using System.Diagnostics;
using AnimusReforged.Paths;

namespace AnimusReforged.Launcher;

public class Altair
{
    public static async Task Launch(bool uModEnabled = false)
    {
        Process? uMod = null;
        if (uModEnabled)
        {
            Logger.Info("Launching uMod");
            uMod = Helper.LaunchuMod();
        }
        Logger.Info("Launching game");
        Process game = Helper.LaunchGame(AppPaths.AltairGameExecutable);
        
        Logger.Info("Waiting for game to exit");
        await game.WaitForExitAsync();
        Logger.Info("Game exited");
        
        Logger.Info("Closing uMod");
        if (uModEnabled && uMod != null && !uMod.HasExited)
        {
            uMod.CloseMainWindow();
        }
    }
}