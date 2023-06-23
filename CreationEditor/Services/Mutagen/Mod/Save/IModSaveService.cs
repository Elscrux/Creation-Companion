using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public interface IModSaveService {
    void SaveMod(IMod mod);
    void BackupMod(IMod mod, int limit = -1);
}
