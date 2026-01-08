using System.Text.Json;
using AnimusReforged.Logging;
using AnimusReforged.Models;
using AnimusReforged.Models.Mods;

namespace AnimusReforged.Mods.Core;

/// <summary>
/// Provides functionality for fetching, caching, and accessing mod manifests from a remote source.
/// The manifest contains information about available mods including their names, download URLs,
/// archive types, and required status.
/// </summary>
public class ManifestService
{
    private const string ALTAR_MANIFEST_URL = "https://raw.githubusercontent.com/animus-reforged/mods/main/altair_manifest.json";
    private const string EZIO_MANIFEST_URL = "https://raw.githubusercontent.com/animus-reforged/mods/main/ezio_manifest.json";
    private const string BROTHERHOOD_MANIFEST_URL = "https://raw.githubusercontent.com/animus-reforged/mods/main/brotherhood_manifest.json";
    private const string REVELATIONS_MANIFEST_URL = "https://raw.githubusercontent.com/animus-reforged/mods/main/revelations_manifest.json";

    private static Dictionary<ManifestType, ModManifest?> _cachedManifests = new Dictionary<ManifestType, ModManifest?>
    {
        { ManifestType.Altair, null },
        { ManifestType.Ezio, null },
        { ManifestType.Brotherhood, null },
        { ManifestType.Revelations, null }
    };

    private static readonly HttpClient _httpClient = new HttpClient();

    /// <summary>
    /// Gets the URL for the specified manifest type.
    /// </summary>
    /// <param name="manifestType">The type of manifest to get the URL for</param>
    /// <returns>The URL for the specified manifest type</returns>
    private static string GetManifestUrl(ManifestType manifestType)
    {
        return manifestType switch
        {
            ManifestType.Ezio => EZIO_MANIFEST_URL,
            ManifestType.Brotherhood => BROTHERHOOD_MANIFEST_URL,
            ManifestType.Revelations => REVELATIONS_MANIFEST_URL,
            _ => ALTAR_MANIFEST_URL // Default to Altair
        };
    }

    /// <summary>
    /// Retrieves the specified mod manifest from the remote source, with optional caching.
    /// If the manifest is already cached and forceRefresh is false, returns the cached version.
    /// Otherwise, fetches the latest manifest from the remote URL and caches it.
    /// </summary>
    /// <param name="manifestType">The type of manifest to retrieve</param>
    /// <param name="forceRefresh">If true, ignores the cached manifest and fetches a new one</param>
    /// <returns>A task representing the asynchronous operation that yields the mod manifest</returns>
    /// <exception cref="InvalidOperationException">Thrown when the manifest cannot be deserialized</exception>
    public static async Task<ModManifest> GetManifestAsync(ManifestType manifestType, bool forceRefresh = false)
    {
        if (_cachedManifests.ContainsKey(manifestType) && _cachedManifests[manifestType] != null && !forceRefresh)
        {
            return _cachedManifests[manifestType]!;
        }

        string manifestUrl = GetManifestUrl(manifestType);
        Logger.Info<ManifestService>($"Fetching {manifestType} mod manifest from remote");
        try
        {
            string json = await _httpClient.GetStringAsync(manifestUrl);
            ModManifest manifest = JsonSerializer.Deserialize<ModManifest>(json) ?? throw new InvalidOperationException("Failed to deserialize manifest");

            _cachedManifests[manifestType] = manifest;

            Logger.Info<ManifestService>($"{manifestType} manifest loaded: {manifest.Mods.Count} mods available");
            return manifest;
        }
        catch (Exception ex)
        {
            Logger.Error<ManifestService>($"Failed to fetch {manifestType} mod manifest");
            Logger.LogExceptionDetails<ManifestService>(ex);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific mod definition from the cached manifest by its ID.
    /// Uses the Altair manifest by default.
    /// </summary>
    /// <param name="modId">The unique identifier of the mod to retrieve</param>
    /// <returns>The mod definition containing information about the requested mod</returns>
    /// <exception cref="InvalidOperationException">Thrown when the manifest has not been loaded yet</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the specified mod ID is not found in the manifest</exception>
    public static ModDefinition GetMod(string modId)
    {
        return GetMod(ManifestType.Altair, modId);
    }

    /// <summary>
    /// Retrieves a specific mod definition from the specified manifest by its ID.
    /// </summary>
    /// <param name="manifestType">The type of manifest to retrieve the mod from</param>
    /// <param name="modId">The unique identifier of the mod to retrieve</param>
    /// <returns>The mod definition containing information about the requested mod</returns>
    /// <exception cref="InvalidOperationException">Thrown when the specified manifest has not been loaded yet</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the specified mod ID is not found in the manifest</exception>
    public static ModDefinition GetMod(ManifestType manifestType, string modId)
    {
        if (!_cachedManifests.ContainsKey(manifestType) || _cachedManifests[manifestType] == null)
        {
            throw new InvalidOperationException($"{manifestType} manifest not loaded. Call GetManifestAsync first.");
        }

        ModManifest manifest = _cachedManifests[manifestType]!;
        if (!manifest.Mods.TryGetValue(modId, out ModDefinition? mod))
        {
            throw new KeyNotFoundException($"Mod '{modId}' not found in {manifestType} manifest");
        }

        return mod;
    }

    /// <summary>
    /// Clears the cached manifest for the specified manifest type.
    /// </summary>
    /// <param name="manifestType">The type of manifest to clear from cache</param>
    public static void ClearCache(ManifestType manifestType)
    {
        _cachedManifests[manifestType] = null;
    }

    /// <summary>
    /// Clears all cached manifests.
    /// </summary>
    public static void ClearAllCache()
    {
        foreach (ManifestType key in _cachedManifests.Keys.ToList())
        {
            _cachedManifests[key] = null;
        }
    }
}