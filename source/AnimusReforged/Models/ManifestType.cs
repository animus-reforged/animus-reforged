namespace AnimusReforged.Models;

/// <summary>
/// Defines the available manifest types that can be used by the application.
/// </summary>
public enum ManifestType
{
    /// <summary>
    /// The Altair manifest containing mods for the Altair game
    /// </summary>
    Altair,

    /// <summary>
    /// The Ezio manifest containing mods for the Ezio game
    /// </summary>
    Ezio,

    /// <summary>
    /// The Brotherhood manifest containing mods for the Brotherhood game
    /// </summary>
    Brotherhood,

    /// <summary>
    /// The Revelations manifest containing mods for the Revelations game
    /// </summary>
    Revelations
}