namespace AnimusReforged.Models.Mods;

/// <summary>
/// Represents a mod item with its full file path and derived name.
/// </summary>
public class UModItem
{
    /// <summary>
    /// Gets or sets the full file path of the mod item.
    /// </summary>
    public string FullPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets the name of the mod item, derived from the file name without extension.
    /// </summary>
    public string Name => Path.GetFileNameWithoutExtension(FullPath);
}

/// <summary>
/// Provides extension methods for UModItem objects.
/// </summary>
public static class UModItemExtensions
{
    /// <summary>
    /// Converts an enumerable collection of UModItem objects to a list of their full paths.
    /// </summary>
    /// <param name="mods">The collection of UModItem objects to convert.</param>
    /// <returns>A list of strings representing the full paths of the mod items.</returns>
    public static List<string> ToList(this IEnumerable<UModItem> mods)
    {
        return mods.Select(mod => mod.FullPath).ToList();
    }
}