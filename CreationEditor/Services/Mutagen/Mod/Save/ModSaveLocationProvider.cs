using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public sealed class ModSaveLocationProvider : IModSaveLocationProvider {
    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;

    private string _directoryPath;

    public ModSaveLocationProvider(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider) {
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;

        _directoryPath = _dataDirectoryProvider.Path;
    }

    public void SaveInDataFolder() => _directoryPath = _dataDirectoryProvider.Path;
    public void SaveInCustomDirectory(string fullPath) => _directoryPath = fullPath;

    public string GetSaveLocation() => _directoryPath;
    public string GetSaveLocation(ModKey modKey) => _fileSystem.Path.Combine(_directoryPath, modKey.FileName);
    public string GetSaveLocation(IModKeyed mod) => GetSaveLocation(mod.ModKey.FileName);

    public string GetBackupSaveLocation() => _fileSystem.Path.Combine(_directoryPath, "Backup");
    public string GetBackupSaveLocation(ModKey modKey) => _fileSystem.Path.Combine(GetBackupSaveLocation(), modKey.FileName);
    public string GetBackupSaveLocation(IModKeyed mod) => GetBackupSaveLocation(mod.ModKey.FileName);
}
