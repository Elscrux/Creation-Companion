using System.IO.Abstractions;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public sealed class ModSaveLocationProvider : IModSaveLocationProvider {
    private readonly IFileSystem _fileSystem;
    private readonly IEnvironmentContext _environmentContext;

    private DirectoryPath _directoryPath;

    public ModSaveLocationProvider(
        IFileSystem fileSystem,
        IEnvironmentContext environmentContext) {
        _fileSystem = fileSystem;
        _environmentContext = environmentContext;

        _directoryPath = _environmentContext.DataDirectoryProvider.Path;
    }

    public void SaveInDataFolder() => _directoryPath = _environmentContext.DataDirectoryProvider.Path;
    public void SaveInCustomDirectory(DirectoryPath absolutePath) => _directoryPath = absolutePath;

    public DirectoryPath GetSaveLocation() => _directoryPath;
    public FilePath GetSaveLocation(ModKey modKey) => _fileSystem.Path.Combine(_directoryPath, modKey.FileName);
    public FilePath GetSaveLocation(IModKeyed mod) => GetSaveLocation(mod.ModKey.FileName);

    public DirectoryPath GetBackupSaveLocation() => _fileSystem.Path.Combine(_directoryPath, "Backup");
    public FilePath GetBackupSaveLocation(ModKey modKey) => _fileSystem.Path.Combine(GetBackupSaveLocation(), modKey.FileName);
    public FilePath GetBackupSaveLocation(IModKeyed mod) => GetBackupSaveLocation(mod.ModKey.FileName);
}
