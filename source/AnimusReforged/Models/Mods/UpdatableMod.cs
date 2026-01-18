using CommunityToolkit.Mvvm.ComponentModel;

namespace AnimusReforged.Models.Mods;

/// <summary>
/// Represents a mod that has an available update, containing information about both the currently installed version and the latest available version.
/// </summary>
public partial class UpdatableMod : ObservableObject
{
    /// <summary>
    /// Gets or sets the unique identifier of the mod.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the display name of the mod.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the version of the mod that is currently installed.
    /// </summary>
    public string CurrentVersion { get; set; }

    /// <summary>
    /// Gets or sets the latest available version of the mod.
    /// </summary>
    public string LatestVersion { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the mod is currently being updated.
    /// </summary>
    [ObservableProperty] private bool isUpdating;

    /// <summary>
    /// Gets a value indicating whether the mod is not currently being updated.
    /// </summary>
    public bool IsNotUpdating => !IsUpdating;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatableMod"/> class.
    /// </summary>
    /// <param name="id">The unique identifier of the mod</param>
    /// <param name="name">The display name of the mod</param>
    /// <param name="currentVersion">The version of the mod that is currently installed</param>
    /// <param name="latestVersion">The latest available version of the mod</param>
    public UpdatableMod(string id, string name, string currentVersion, string latestVersion)
    {
        Id = id;
        Name = name;
        CurrentVersion = currentVersion;
        LatestVersion = latestVersion;
        IsUpdating = false;
    }
}