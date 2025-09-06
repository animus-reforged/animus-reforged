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

public class uModTemplateParser
{
    // Variables
    private readonly string _templatePath;
    private readonly string _modsPath;
    private List<string> _otherLines { get; set; } = [];

    // Constructor
    public uModTemplateParser(string templatePath, string modsPath)
    {
        _templatePath = templatePath;
        _modsPath = modsPath;
        LoadLines();
    }

    // Methods
    private void LoadLines()
    {
        if (!File.Exists(_templatePath))
        {
            Logger.Debug($"Template file not found ({_templatePath})");
            throw new FileNotFoundException($"Template file not found ({_templatePath})");
        }

        _otherLines.Clear();

        foreach (string line in File.ReadAllLines(_templatePath))
        {
            if (!line.StartsWith("Add_true:"))
            {
                Logger.Debug($"Found text line: {line}");
                _otherLines.Add(line);
            }
        }
    }
    
    public (List<string> enabledMods, List<string> disabledMods) LoadMods()
    {
        if (!Directory.Exists(_modsPath))
        {
            Logger.Debug($"Mods directory not found ({_modsPath})");
            throw new DirectoryNotFoundException($"Mods directory not found ({_modsPath})");
        }
        List<string> enabledMods = LoadEnabledMods();
        Logger.Debug($"Searching mods folder: {_modsPath}");
        List<string> allMods = Directory.GetFiles(_modsPath, "*.tpf", SearchOption.AllDirectories).ToList();
        Logger.Info($"Found {allMods.Count} mod file(s) in folder");
        foreach (string mod in allMods)
        {
            Logger.Debug($"Mod found: {mod}");
        }
        List<string> disabledMods = allMods.Except(enabledMods, StringComparer.OrdinalIgnoreCase).ToList();
        Logger.Info($"Found {disabledMods.Count} disabled mod(s)");
        foreach (string mod in disabledMods)
        {
            Logger.Debug($"Disabled mod: {mod}");
        }
        return (enabledMods, disabledMods);
    }

    private List<string> LoadEnabledMods()
    {
        if (!File.Exists(_templatePath))
        {
            Logger.Debug($"Template file not found ({_templatePath})");
            throw new FileNotFoundException($"Template file not found ({_templatePath})");
        }
        Logger.Debug("Loading enabled mods from template file");
        List<string> enabledMods = new List<string>();
        foreach (string line in File.ReadAllLines(_templatePath))
        {
            if (line.StartsWith("Add_true:"))
            {
                string enabledMod = line.Substring("Add_true:".Length).Trim();
                Logger.Debug($"Found enabled mod: {enabledMod}");
                enabledMods.Add(enabledMod);
            }
        }

        return enabledMods;
    }
    
    public void SaveEnabledMods(List<string> enabledMods)
    {
        Logger.Info("Saving changes to template file");
        List<string> templateFileLines = [];
        Logger.Info("Writing lines to template file");
        templateFileLines.AddRange(_otherLines);
        if (enabledMods.Count > 0)
        {
            Logger.Info("Writing enabled mods to template file");
            templateFileLines.AddRange(enabledMods.Select(modPath => $"Add_true:{modPath}"));
        }
        File.WriteAllLines(_templatePath, templateFileLines);
        Logger.Info("Saving changes to template file done");
    }
}