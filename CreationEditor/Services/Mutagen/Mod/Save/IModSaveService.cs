using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public interface IModSaveService {
    /// <summary>
    /// Save the given mod. Overwrite 
    /// </summary>
    /// <param name="mod">Mod to save</param>
    void SaveMod(IMod mod);

    /// <summary>
    /// Backup the currently saved mod. If limit is set, delete older backups.
    /// </summary>
    /// <param name="mod">Mod to save a backup for</param>
    /// <param name="limit">Limit of backups</param>
    void BackupMod(IMod mod, int limit = -1);
}
