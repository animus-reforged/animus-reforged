using AnimusReforged.Mods.Utilities;
using AnimusReforged.Paths;

namespace AnimusReforged.Mods.Altair;

public static class ModManager
{
    // Constants
    private const string ASI_LOADER_URL = "https://github.com/ThirteenAG/Ultimate-ASI-Loader/releases/latest/download/Ultimate-ASI-Loader.zip";
    private const string EAGLE_PATCH_URL = "https://github.com/Sergeanur/EaglePatch/releases/latest/download/EaglePatchAC1.rar";

    // Variables
    private static readonly DownloadManager _downloadManager = new DownloadManager();

    // ASI Loader
    public static async Task DownloadAsiLoader(Action<int> progressCallback)
    {
        Logger.Info("Downloading Ultimate ASI Loader");
        _downloadManager.ProgressChanged += progressCallback;
        string savePath = Path.Combine(AppPaths.Downloads, Path.GetFileName(ASI_LOADER_URL));
        Logger.Debug($"Save path: {savePath}");
        try
        {
            await _downloadManager.DownloadFileAsync(ASI_LOADER_URL, savePath);
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to download Ultimate ASI Loader");
            Logger.LogExceptionDetails(ex);
            throw new Exception("Failed to extract Ultimate ASI Loader");
        }
        Logger.Info("Download complete");
    }

    public static async Task InstallAsiLoader()
    {
        Logger.Info("Extracting Ultimate ASI Loader");
        string zipFile = Path.Combine(AppPaths.Downloads, Path.GetFileName(ASI_LOADER_URL));
        string outputPath = AppPaths.Base;
        Logger.Debug($"Zip file location: {zipFile}");
        Logger.Debug($"Extraction path: {outputPath}");
        try
        {
            Extractor.ExtractZip(zipFile, outputPath);
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to extract Ultimate ASI Loader");
            Logger.LogExceptionDetails(ex);
            throw new Exception("Failed to extract Ultimate ASI Loader");
        }

        // Create a scripts folder if it's missing
        if (!Directory.Exists(AppPaths.Scripts))
        {
            Directory.CreateDirectory(AppPaths.Scripts);
        }
        await Task.Delay(1);
    }

    // EaglePatch
    public static async Task DownloadEaglePatch(Action<int> progressCallback)
    {
        Logger.Info("Downloading EaglePatch mod");
        _downloadManager.ProgressChanged += progressCallback;
        string savePath = Path.Combine(AppPaths.Downloads, Path.GetFileName(EAGLE_PATCH_URL));
        Logger.Debug($"Save path: {savePath}");
        try
        {
            await _downloadManager.DownloadFileAsync(EAGLE_PATCH_URL, savePath);
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to download EaglePatch mod");
            Logger.LogExceptionDetails(ex);
            throw new Exception("Failed to extract EaglePatch mod");
        }
        Logger.Info("Download complete");
    }

    public static async Task InstallEaglePatch()
    {
        Logger.Info("Extracting EaglePatch mod");
        string zipFile = Path.Combine(AppPaths.Downloads, Path.GetFileName(EAGLE_PATCH_URL));
        string outputPath = AppPaths.Scripts;
        Logger.Debug($"Zip file location: {zipFile}");
        Logger.Debug($"Extraction path: {outputPath}");
        try
        {
            Extractor.ExtractRar(zipFile, outputPath, [".ini", ".asi"]);
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to extract EaglePatch mod");
            Logger.LogExceptionDetails(ex);
            throw new Exception("Failed to extract EaglePatch mod");
        }
        await Task.Delay(1);
    }

    // uMod
    public static async Task DownloaduMod(Action<int> progressCallback)
    {
        Logger.Info("Downloading uMod");
    }

    public static async Task InstalluMod()
    {
        Logger.Info("Installing uMod");
    }

    // Overhaul
    public static async Task DownloadOverhaul(Action<int> progressCallback)
    {
        Logger.Info("Downloading Overhaul mod");
    }

    public static async Task InstallOverhaul()
    {
        Logger.Info("Installing Overhaul mod");
    }
}