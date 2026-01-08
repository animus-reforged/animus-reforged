using System.Text.Json.Serialization;

namespace AnimusReforged.Models.Mods;

/// <summary>
/// Represents a manifest containing metadata about available mods.
/// The manifest includes the schema version and a dictionary of mod definitions keyed by mod ID.
/// </summary>
/// <param name="SchemaVersion">The version of the manifest schema</param>
/// <param name="Mods">A dictionary mapping mod IDs to their corresponding definitions</param>
public record ModManifest(
    [property: JsonPropertyName("schema_version")]
    string SchemaVersion,
    [property: JsonPropertyName("mods")] Dictionary<string, ModDefinition> Mods
);