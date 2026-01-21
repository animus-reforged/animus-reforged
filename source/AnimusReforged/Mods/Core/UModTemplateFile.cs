using System.Text;
using AnimusReforged.Logging;

namespace AnimusReforged.Mods.Core;

/// <summary>
/// Manages a mod template file that tracks enabled and disabled mods.
/// </summary>
public class UModTemplateFile
{
    /// <summary>
    /// The prefix used to identify enabled mod entries in the template file.
    /// </summary>
    private const string EnabledModPrefix = "Add_true:";

    /// <summary>
    /// The file pattern used to search for mod files in the mods directory.
    /// </summary>
    private const string ModFilePattern = "*.tpf";

    /// <summary>
    /// The path to the template file being managed.
    /// </summary>
    private readonly string _templatePath;

    /// <summary>
    /// The directory path where mod files are located.
    /// </summary>
    private readonly string _modsPath;

    /// <summary>
    /// A list of header lines from the template file that are not mod entries.
    /// </summary>
    private readonly List<string> _headerLines = [];

    /// <summary>
    /// A list of file paths representing mods that are currently enabled in the template.
    /// </summary>
    private readonly List<string> _enabledModPaths = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="UModTemplateFile"/> class.
    /// </summary>
    /// <param name="templatePath">The path to the template file to manage.</param>
    /// <param name="modsPath">The directory path where mod files are located.</param>
    /// <exception cref="ArgumentNullException">Thrown when either parameter is null.</exception>
    public UModTemplateFile(string templatePath, string modsPath)
    {
        _templatePath = templatePath ?? throw new ArgumentNullException(nameof(templatePath));
        _modsPath = modsPath ?? throw new ArgumentNullException(nameof(modsPath));
        ParseTemplate();
    }

    /// <summary>
    /// Parses the template file to extract header lines and enabled mod entries.
    /// </summary>
    private void ParseTemplate()
    {
        if (!File.Exists(_templatePath))
        {
            Logger.Debug<UModTemplateFile>($"Template file not found ({_templatePath})");
            throw new FileNotFoundException("Template file not found", _templatePath);
        }

        _headerLines.Clear();
        _enabledModPaths.Clear();

        // Single pass parsing - reads file once instead of twice
        foreach (string line in File.ReadLines(_templatePath))
        {
            if (line.StartsWith(EnabledModPrefix, StringComparison.Ordinal))
            {
                string modPath = line[EnabledModPrefix.Length..].Trim();
                Logger.Debug<UModTemplateFile>($"Found enabled mod: {modPath}");
                _enabledModPaths.Add(modPath);
            }
            else
            {
                Logger.Debug<UModTemplateFile>($"Found header line: {line}");
                _headerLines.Add(line);
            }
        }
    }

    /// <summary>
    /// Loads the list of available mods from the mods directory and determines which are enabled/disabled.
    /// </summary>
    /// <returns>A tuple containing the list of enabled mods and the list of disabled mods.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when the mods directory does not exist.</exception>
    public (IReadOnlyList<string> EnabledMods, IReadOnlyList<string> DisabledMods) LoadMods()
    {
        if (!Directory.Exists(_modsPath))
        {
            Logger.Debug<UModTemplateFile>($"Mods directory not found ({_modsPath})");
            throw new DirectoryNotFoundException($"Mods directory not found ({_modsPath})");
        }

        Logger.Debug<UModTemplateFile>($"Searching mods folder: {_modsPath}");
        // Use EnumerateFiles for lazy evaluation
        List<string> allMods = Directory.EnumerateFiles(_modsPath, ModFilePattern, SearchOption.AllDirectories).ToList();

        Logger.Info<UModTemplateFile>($"Found {allMods.Count} mod file(s) in folder");
        allMods.ForEach(mod => Logger.Debug<UModTemplateFile>($"Mod found: {mod}"));

        List<string> disabledMods = allMods.Except(_enabledModPaths, StringComparer.OrdinalIgnoreCase).ToList();

        Logger.Info<UModTemplateFile>($"Found {disabledMods.Count} disabled mod(s)");
        disabledMods.ForEach(mod => Logger.Debug<UModTemplateFile>($"Disabled mod: {mod}"));

        return ([.. _enabledModPaths], disabledMods);
    }

    /// <summary>
    /// Asynchronously loads the list of available mods from the mods directory and determines which are enabled/disabled.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to allow cancellation of the operation.</param>
    /// <returns>A task that resolves to a tuple containing the list of enabled mods and the list of disabled mods.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown when the mods directory does not exist.</exception>
    public async Task<(IReadOnlyList<string> EnabledMods, IReadOnlyList<string> DisabledMods)> LoadModsAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_modsPath))
        {
            Logger.Debug<UModTemplateFile>($"Mods directory not found ({_modsPath})");
            throw new DirectoryNotFoundException($"Mods directory not found ({_modsPath})");
        }

        Logger.Debug<UModTemplateFile>($"Searching mods folder: {_modsPath}");
        List<string> allMods = await Task.Run(() => Directory.EnumerateFiles(_modsPath, ModFilePattern, SearchOption.AllDirectories).ToList(), cancellationToken);

        Logger.Info<UModTemplateFile>($"Found {allMods.Count} mod file(s) in folder");
        allMods.ForEach(mod => Logger.Debug<UModTemplateFile>($"Mod found: {mod}"));

        List<string> disabledMods = allMods.Except(_enabledModPaths, StringComparer.OrdinalIgnoreCase).ToList();

        Logger.Info<UModTemplateFile>($"Found {disabledMods.Count} disabled mod(s)");
        disabledMods.ForEach(mod => Logger.Debug<UModTemplateFile>($"Disabled mod: {mod}"));

        return ([.. _enabledModPaths], disabledMods);
    }

    /// <summary>
    /// Saves the specified list of enabled mods to the template file.
    /// </summary>
    /// <param name="enabledMods">An enumerable collection of mod file paths to mark as enabled.</param>
    /// <exception cref="ArgumentNullException">Thrown when enabledMods is null.</exception>
    public void SaveEnabledMods(IEnumerable<string> enabledMods)
    {
        ArgumentNullException.ThrowIfNull(enabledMods);

        Logger.Info<UModTemplateFile>("Saving changes to template file");

        // Use StringBuilder for efficient string concatenation
        StringBuilder contentBuilder = new StringBuilder();

        foreach (string line in _headerLines)
        {
            contentBuilder.Append(line).Append('\n');
        }

        foreach (string modPath in enabledMods)
        {
            contentBuilder.Append(EnabledModPrefix).Append(modPath).Append('\n');
        }

        File.WriteAllText(_templatePath, contentBuilder.ToString(), new UTF8Encoding(false));

        Logger.Info<UModTemplateFile>("Saving changes to template file done");
    }

    /// <summary>
    /// Reloads the template from disk.
    /// </summary>
    public void Refresh() => ParseTemplate();
}