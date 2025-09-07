using System.Diagnostics;
using AnimusReforged.Paths;

namespace AnimusReforged.Launcher;

public class Helper
{
    public static Process LaunchuMod()
    {
        Process uMod = new Process();
        uMod.StartInfo.FileName = AppPaths.uModExecutable;
        uMod.StartInfo.WorkingDirectory = AppPaths.uMod;
        uMod.StartInfo.UseShellExecute = true;
        uMod.Start();
        return uMod;
    }

    public static Process LaunchGame(string executablePath)
    {
        Process game = new Process();
        game.StartInfo.FileName = executablePath;
        game.StartInfo.WorkingDirectory = AppPaths.Base;
        game.StartInfo.UseShellExecute = true;
        game.Start();
        return game;   
    }
}