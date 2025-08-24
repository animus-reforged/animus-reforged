using AnimusReforged.Mods.Utilities;
using AnimusReforged.Paths;

namespace AnimusReforged.Mods.Altair;

public static class ModManager
{
    // Constants
    private const string ASI_LOADER_URL = "https://github.com/ThirteenAG/Ultimate-ASI-Loader/releases/latest/download/Ultimate-ASI-Loader.zip";
    private const string EAGLE_PATCH_URL = "https://github.com/Sergeanur/EaglePatch/releases/latest/download/EaglePatchAC1.rar";
    private const string UMOD_URL = "https://github.com/animus-reforged/uMod/releases/latest/download/uMod.zip";
    private const string OVERHAUL_MOD_URL = "https://github.com/animus-reforged/mods/releases/download/altair/Overhaul.zip";

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
            throw new Exception("Failed to download Ultimate ASI Loader");
        }
        Logger.Info("Download complete");
    }

    public static void InstallAsiLoader()
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
        Directory.CreateDirectory(AppPaths.Scripts);
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
            throw new Exception("Failed to download EaglePatch mod");
        }
        Logger.Info("Download complete");
    }

    public static void InstallEaglePatch()
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
    }

    // uMod
    public static async Task DownloaduMod(Action<int> progressCallback)
    {
        Logger.Info("Downloading uMod");
        _downloadManager.ProgressChanged += progressCallback;
        string savePath = Path.Combine(AppPaths.Downloads, Path.GetFileName(UMOD_URL));
        Logger.Debug($"Save path: {savePath}");
        try
        {
            await _downloadManager.DownloadFileAsync(UMOD_URL, savePath);
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to download uMod");
            Logger.LogExceptionDetails(ex);
            throw new Exception("Failed to download uMod");
        }
        Logger.Info("Download complete");
    }

    public static void InstalluMod()
    {
        Logger.Info("Installing uMod");
        string zipFile = Path.Combine(AppPaths.Downloads, Path.GetFileName(UMOD_URL));
        string outputPath = AppPaths.uMod;
        Logger.Debug($"Zip file location: {zipFile}");
        Logger.Debug($"Extraction path: {outputPath}");
        try
        {
            Directory.CreateDirectory(outputPath);
            Extractor.ExtractZip(zipFile, outputPath);
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to extract uMod");
            Logger.LogExceptionDetails(ex);
            throw new Exception("Failed to extract uMod");
        }
        Directory.CreateDirectory(AppPaths.Mods);
    }

    // Overhaul
    public static async Task DownloadOverhaul(Action<int> progressCallback)
    {
        Logger.Info("Downloading Overhaul mod");
        _downloadManager.ProgressChanged += progressCallback;
        string savePath = Path.Combine(AppPaths.Downloads, Path.GetFileName(OVERHAUL_MOD_URL));
        Logger.Debug($"Save path: {savePath}");
        try
        {
            await _downloadManager.DownloadFileAsync(OVERHAUL_MOD_URL, savePath);
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to download Overhaul mod");
            Logger.LogExceptionDetails(ex);
            throw new Exception("Failed to download Overhaul mod");
        }
        Logger.Info("Download complete");
    }

    public static void InstallOverhaul()
    {
        Logger.Info("Installing Overhaul mod");
        string zipFile = Path.Combine(AppPaths.Downloads, Path.GetFileName(OVERHAUL_MOD_URL));
        string outputPath = AppPaths.AltairOverhaulMod;
        Logger.Debug($"Zip file location: {zipFile}");
        Logger.Debug($"Extraction path: {outputPath}");
        try
        {
            Directory.CreateDirectory(outputPath);
            Extractor.ExtractZip(zipFile, outputPath);
        }
        catch (Exception ex)
        {
            Logger.Error("Failed to extract Overhaul mod");
            Logger.LogExceptionDetails(ex);
            throw new Exception("Failed to extract Overhaul mod");
        }
    }
}