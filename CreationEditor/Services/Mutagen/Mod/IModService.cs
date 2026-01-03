using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.Mod;

public interface IModService {
    bool HasMasterTransitive(ModKey modKey, ModKey master);
    bool IsModOrHasMasterTransitive(ModKey modKey, ModKey master);
    bool TryGetMastersTransitive(ModKey modKey, out HashSet<ModKey> masters);
}
