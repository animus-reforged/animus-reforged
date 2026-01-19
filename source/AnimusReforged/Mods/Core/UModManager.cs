using System.Text;
using AnimusReforged.Logging;
using AnimusReforged.Utilities;

namespace AnimusReforged.Mods.Core;

/// <summary>
/// Manages uMod configuration and setup for games.
/// </summary>
public class UModManager
{
    private static readonly UTF8Encoding Utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
    private static readonly UnicodeEncoding UnicodeNoBom = new UnicodeEncoding(bigEndian: false, byteOrderMark: false);

    /// <summary>
    /// Sets up the uMod AppData directory and config file with the specified game path.
    /// </summary>
    /// <param name="gamePath">The path to the game executable or directory.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <exception cref="ArgumentException">Thrown when gamePath is null or empty.</exception>
    public static async Task SetupAppdata(string gamePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gamePath);

        Logger.Debug<UModManager>("Setting up uMod AppData");
        Directory.CreateDirectory(FilePaths.UModAppdata);
        Logger.Debug<UModManager>($"Game path: {gamePath}");

        if (File.Exists(FilePaths.UModConfig))
        {
            await AppendGamePathIfMissing(gamePath, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            Logger.Debug<UModManager>("Creating new uMod AppData config file");
            await File.WriteAllTextAsync(FilePaths.UModConfig, gamePath, UnicodeNoBom, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Appends the game executable path to uMod's AppData config file
    /// </summary>
    /// <param name="gamePath">Path to the game.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    private static async Task AppendGamePathIfMissing(string gamePath, CancellationToken cancellationToken)
    {
        Logger.Debug<UModManager>("Checking existing config file");

        string[] lines = await File.ReadAllLinesAsync(FilePaths.UModConfig, UnicodeNoBom, cancellationToken).ConfigureAwait(false);

        HashSet<string> existingPaths = new HashSet<string>(lines, StringComparer.OrdinalIgnoreCase);

        if (existingPaths.Contains(gamePath))
        {
            Logger.Debug<UModManager>("Path already exists in config file");
        }
        else
        {
            Logger.Debug<UModManager>("Appending path to config file");

            string fileContent = await File.ReadAllTextAsync(FilePaths.UModConfig, UnicodeNoBom, cancellationToken).ConfigureAwait(false);
            bool endsWithNewline = fileContent.EndsWith(Environment.NewLine) || fileContent.EndsWith("\n") || fileContent.EndsWith("\r\n");

            string textToAppend = endsWithNewline ? gamePath : Environment.NewLine + gamePath;
            await File.AppendAllTextAsync(FilePaths.UModConfig, textToAppend, UnicodeNoBom, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Removes the specified game path from the uMod config file.
    /// </summary>
    /// <param name="gamePath">The path to remove from the config file.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <exception cref="ArgumentException">Thrown when gamePath is null or empty.</exception>
    public static async Task RemoveGameFromAppdata(string gamePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gamePath);

        if (!File.Exists(FilePaths.UModConfig))
        {
            Logger.Error<UModManager>("uMod config file not found");
            return;
        }

        string[] lines = await File.ReadAllLinesAsync(FilePaths.UModConfig, UnicodeNoBom, cancellationToken).ConfigureAwait(false);

        // Filter out matching lines
        string[] updatedLines = lines
            .Where(line => !line.Trim().Equals(gamePath, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (updatedLines.Length < lines.Length)
        {
            await File.WriteAllLinesAsync(FilePaths.UModConfig, updatedLines, UnicodeNoBom, cancellationToken).ConfigureAwait(false);
            Logger.Debug<UModManager>($"Removed '{gamePath}' from uMod config");
        }
        else
        {
            Logger.Warning<UModManager>($"'{gamePath}' not found in config file");
        }
    }

    /// <summary>
    /// Sets up the uMod save file and template for the specified game.
    /// </summary>
    /// <param name="gamePath">The path to the game executable or directory.</param>
    /// <param name="templateName">The name of the template file to create.</param>
    /// <param name="modFilePaths">Collection of mod file paths to include in the template as enabled mods.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <exception cref="ArgumentException">Thrown when gamePath or templateName is null or empty.</exception>
    public static async Task SetupSaveFile(string gamePath, string templateName, IEnumerable<string>? modFilePaths = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gamePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(templateName);

        Directory.CreateDirectory(FilePaths.UModTemplates);

        // Ensure status file exists
        if (!File.Exists(FilePaths.UModStatusFile))
        {
            Logger.Debug<UModManager>("Creating uMod status file");
            await File.WriteAllTextAsync(FilePaths.UModStatusFile, "Enabled=1", Encoding.ASCII, cancellationToken).ConfigureAwait(false);
        }

        Logger.Debug<UModManager>("Setting up uMod template");
        string templatePath = Path.Combine(FilePaths.UModTemplates, templateName);

        // Build template content
        string content = BuildTemplateContent(modFilePaths);
        await File.WriteAllTextAsync(templatePath, content, Utf8NoBom, cancellationToken).ConfigureAwait(false);

        // Append to save files
        string saveFileEntry = $"{gamePath}|{templatePath}\n";
        Logger.Debug<UModManager>($"Save file entry: {saveFileEntry}");
        await File.AppendAllTextAsync(FilePaths.UModSaveFiles, saveFileEntry, UnicodeNoBom, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates `template.txt` file used to load all uMod mods
    /// </summary>
    /// <param name="modFilePaths">Paths to the mod files.</param>
    /// <returns>template.txt content as a string.</returns>
    private static string BuildTemplateContent(IEnumerable<string>? modFilePaths = null)
    {
        StringBuilder sb = new StringBuilder(256);
        sb.Append("SaveAllTextures:0\n");
        sb.Append("SaveSingleTexture:0\n");
        sb.Append("FontColour:255,0,0\n");
        sb.Append("TextureColour:0,255,0\n");

        if (modFilePaths?.Any() == true)
        {
            foreach (string modPath in modFilePaths.Where(path => !string.IsNullOrEmpty(path)))
            {
                sb.Append($"Add_true:{modPath}\n");
            }
        }

        return sb.ToString();
    }
}