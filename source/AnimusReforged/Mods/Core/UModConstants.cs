using AnimusReforged.Utilities;

namespace AnimusReforged.Mods.Core;

/// <summary>
/// Contains constant paths and locations for uMod functionality.
/// </summary>
public abstract class UModConstants
{
    /// <summary>
    /// Gets the base location of the uMod installation directory.
    /// </summary>
    public static readonly string UModLocation = AbsolutePath.GetFullPath("uMod");

    /// <summary>
    /// Gets the path to the uMod application data directory in the user's Application Data folder.
    /// </summary>
    public static readonly string UModAppdata = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "uMod");

    /// <summary>
    /// Gets the path to the uMod configuration file (uMod_DX9.txt).
    /// </summary>
    public static readonly string UModConfig = Path.Combine(UModAppdata, "uMod_DX9.txt");

    /// <summary>
    /// Gets the path to the uMod templates directory.
    /// </summary>
    public static readonly string UModTemplates = Path.Combine(UModLocation, "templates");

    /// <summary>
    /// Gets the path to the uMod status file.
    /// </summary>
    public static readonly string UModStatusFile = Path.Combine(UModLocation, "Status.txt");

    /// <summary>
    /// Gets the path to the uMod save files configuration.
    /// </summary>
    public static readonly string UModSaveFiles = Path.Combine(UModLocation, "uMod_SaveFiles.txt");
}