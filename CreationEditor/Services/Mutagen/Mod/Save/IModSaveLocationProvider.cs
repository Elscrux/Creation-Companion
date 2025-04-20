using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.Mod.Save;

// todo: replace with data sources
public interface IModSaveLocationProvider {
    /// <summary>
    /// Set the save location to the data folder.
    /// </summary>
    void SaveInDataFolder();

    /// <summary>
    /// Set the save location to a custom directory.
    /// </summary>
    /// <param name="fullPath">Full path set as the save location</param>
    void SaveInCustomDirectory(string fullPath);

    /// <summary>
    /// Get the current save location.
    /// </summary>
    /// <returns>Current save location</returns>
    string GetSaveLocation();

    /// <summary>
    /// Get the save location of a specific mod key.
    /// </summary>
    /// <param name="modKey">Mod key to get the save location for</param>
    /// <returns>Save location of the specified mod key</returns>
    string GetSaveLocation(ModKey modKey);

    /// <summary>
    /// Get the save location of a specific mod.
    /// </summary>
    /// <param name="mod">Mod to get the save location for</param>
    /// <returns>Save location of the specified mod</returns>
    string GetSaveLocation(IModKeyed mod);

    /// <summary>
    /// Get the backup save location.
    /// </summary>
    /// <returns></returns>
    string GetBackupSaveLocation();

    /// <summary>
    /// Get the backup save location of a specific mod key.
    /// </summary>
    /// <param name="modKey">Mod key to get the backup save location for</param>
    /// <returns>Backup save location of the specified mod key</returns>
    string GetBackupSaveLocation(ModKey modKey);

    /// <summary>
    /// Get the backup save location of a specific mod.
    /// </summary>
    /// <param name="mod">Mod to get the backup save location for</param>
    /// <returns>Backup save location of the specified mod</returns>
    string GetBackupSaveLocation(IModKeyed mod);
}
