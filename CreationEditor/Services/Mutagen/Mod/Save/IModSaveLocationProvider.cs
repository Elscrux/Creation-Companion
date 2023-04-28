using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public interface IModSaveLocationProvider {
    void SaveInDataFolder();
    void SaveInCustomDirectory(DirectoryPath absolutePath);

    DirectoryPath GetSaveLocation();
    FilePath GetSaveLocation(ModKey modKey);
    FilePath GetSaveLocation(IModKeyed mod);

    DirectoryPath GetBackupSaveLocation();
    FilePath GetBackupSaveLocation(ModKey modKey);
    FilePath GetBackupSaveLocation(IModKeyed mod);

}
