using System.IO.Abstractions;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public sealed class ModSaveLocationProvider : IModSaveLocationProvider {
    private readonly IFileSystem _fileSystem;
    private readonly IEnvironmentContext _environmentContext;

    private string _directoryPath;

    public ModSaveLocationProvider(
        IFileSystem fileSystem,
        IEnvironmentContext environmentContext) {
        _fileSystem = fileSystem;
        _environmentContext = environmentContext;

        _directoryPath = _environmentContext.DataDirectoryProvider.Path;
    }

    public void SaveInDataFolder() => _directoryPath = _environmentContext.DataDirectoryProvider.Path;
    public void SaveInCustomDirectory(string absolutePath) => _directoryPath = absolutePath;

    public string GetSaveLocation() => _directoryPath;
    public string GetSaveLocation(ModKey modKey) => _fileSystem.Path.Combine(_directoryPath, modKey.FileName);
    public string GetSaveLocation(IModKeyed mod) => GetSaveLocation(mod.ModKey.FileName);

    public string GetBackupSaveLocation() => _fileSystem.Path.Combine(_directoryPath, "Backup");
    public string GetBackupSaveLocation(ModKey modKey) => _fileSystem.Path.Combine(GetBackupSaveLocation(), modKey.FileName);
    public string GetBackupSaveLocation(IModKeyed mod) => GetBackupSaveLocation(mod.ModKey.FileName);
}
