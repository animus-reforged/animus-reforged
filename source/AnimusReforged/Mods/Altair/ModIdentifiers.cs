namespace AnimusReforged.Mods.Altair;

/// <summary>
/// Contains constant identifiers for different mod types supported by the manager.
/// </summary>
public static class ModIdentifiers
{
    /// <summary>
    /// Identifier for the ASI loader mod
    /// </summary>
    public const string AsiLoader = "asi_loader";

    /// <summary>
    /// Identifier for the Eagle patch mod
    /// </summary>
    public const string EaglePatch = "eagle_patch";

    /// <summary>
    /// Identifier for the Altair fix mod
    /// </summary>
    public const string AltairFix = "altair_fix";

    /// <summary>
    /// Identifier for the ReShade mod
    /// </summary>
    public const string ReShade = "reshade";

    /// <summary>
    /// Identifier for the UMod mod
    /// </summary>
    public const string UMod = "umod";

    /// <summary>
    /// Identifier for the overhaul mod
    /// </summary>
    public const string Overhaul = "overhaul";

    /// <summary>
    /// Converts ModIdentifier to a formatted string
    /// </summary>
    /// <param name="modIdentifier">ModIdentifier</param>
    /// <returns>ModIdentifier as a properly formatted string, if it can't find correct conversion returns ModIdentifier</returns>
    public static string GetModName(string modIdentifier)
    {
        return modIdentifier switch
        {
            AsiLoader => "ASI Loader",
            EaglePatch => "Eagle Patch",
            AltairFix => "Altair Fix",
            ReShade => "ReShade",
            UMod => "UMod",
            Overhaul => "Overhaul",
            _ => modIdentifier
        };
    }
}