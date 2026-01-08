using System.Text.Json.Serialization;

namespace AnimusReforged.Models.Mods;

/// <summary>
/// Represents the definition of a mod containing all necessary information for downloading and installing it.
/// </summary>
/// <param name="Name">The display name of the mod</param>
/// <param name="Url">The download URL for the mod archive</param>
/// <param name="ArchiveType">The type of archive format (e.g., zip, rar) used by the mod</param>
/// <param name="Version">The version of the mod</param>
public record ModDefinition(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("archive_type")]
    string ArchiveType,
    [property: JsonPropertyName("version")]
    string Version
);