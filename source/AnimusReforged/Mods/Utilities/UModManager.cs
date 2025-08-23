using AnimusReforged.Paths;

namespace AnimusReforged.Mods.Utilities;

public static class UModManager
{
    public static async Task SetupAppdata(string gamePath)
    {
        Logger.Debug("Setting up uMod AppData");
        Directory.CreateDirectory(AppPaths.uModAppdata);
        string encodedPath = Encode(gamePath);
        Logger.Debug($"Encoded path: {encodedPath}");
        await File.WriteAllTextAsync(AppPaths.uModConfig, encodedPath);
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
        File.WriteAllLines(templatePath, new[]
        {
            "SaveAllTextures:0",
            "SaveSingleTexture:0",
            "FontColour:255,0,0",
            "TextureColour:0,255,0",
            $"Add_true:{AppPaths.AltairOverhaulModFile}\n"
        });
        string saveFileEntry = $"{gamePath}|{templatePath}";
        Logger.Debug($"Save file entry: {saveFileEntry}");
        string encodedSaveFileEntry = Encode(saveFileEntry);
        Logger.Debug($"Encoded save file entry: {encodedSaveFileEntry}");
        await File.WriteAllTextAsync(AppPaths.uModSaveFiles, encodedSaveFileEntry);
    }

    private static string Encode(string input)
    {
        List<char> charList = new();
        for (int i = 0; i < input.Length; i++)
        {
            // null before every character except the first
            if (i > 0)
            {
                charList.Add('\0');
            }
            charList.Add(input[i]);
        }

        // null at the end
        charList.Add('\0');

        return new string(charList.ToArray());
    }
}