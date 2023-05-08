using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public interface IModSaveLocationProvider {
    void SaveInDataFolder();
    void SaveInCustomDirectory(string absolutePath);

    string GetSaveLocation();
    string GetSaveLocation(ModKey modKey);
    string GetSaveLocation(IModKeyed mod);

    string GetBackupSaveLocation();
    string GetBackupSaveLocation(ModKey modKey);
    string GetBackupSaveLocation(IModKeyed mod);
}
