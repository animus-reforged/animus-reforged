using System.Text;
using AnimusReforged.Paths;

namespace AnimusReforged.Mods.Utilities;

public static class UModManager
{
    public static async Task SetupAppdata(string gamePath)
    {
        Logger.Debug("Setting up uMod AppData");
        Directory.CreateDirectory(AppPaths.uModAppdata);
        Logger.Debug($"Encoded path: {gamePath}");
        if (File.Exists(AppPaths.uModConfig))
        {
            Logger.Debug("uMod AppData config file already exists, appending path");
            await File.AppendAllTextAsync(AppPaths.uModConfig, Environment.NewLine + gamePath, System.Text.Encoding.Unicode);
        }
        else
        {
            Logger.Debug("Creating new uMod AppData config file (uMod_DX9.txt)");
            await File.WriteAllTextAsync(AppPaths.uModConfig, gamePath, System.Text.Encoding.Unicode);
        }
    }

    public static async Task SetupSaveFile(string gamePath, string templateName)
    {
        Directory.CreateDirectory(AppPaths.uModTemplates);
        if (!File.Exists(AppPaths.uModStatusFile))
        {
            Logger.Debug("Creating uMod status file");
            await File.WriteAllTextAsync(AppPaths.uModStatusFile, "Enabled=1");
        }
        Logger.Debug("Setting up uMod template");
        string templatePath = Path.Combine(AppPaths.uModTemplates, templateName);
        File.WriteAllLines(templatePath, [
            "SaveAllTextures:0",
            "SaveSingleTexture:0",
            "FontColour:255,0,0",
            "TextureColour:0,255,0",
            $"Add_true:{AppPaths.AltairOverhaulModFile}\n"
        ]);
        string saveFileEntry = $"{gamePath}|{templatePath}\n";
        Logger.Debug($"Save file entry: {saveFileEntry}");
        await File.AppendAllTextAsync(AppPaths.uModSaveFiles, saveFileEntry, new UnicodeEncoding(false, false));
    }
}