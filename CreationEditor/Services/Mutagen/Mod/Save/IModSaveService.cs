using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public interface IModSaveService {
    void SaveMod(ILinkCache linkCache, IMod mod);
    void BackupMod(IMod mod, int limit = -1);
}
