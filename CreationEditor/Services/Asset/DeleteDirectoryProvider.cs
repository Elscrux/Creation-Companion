using System.IO.Abstractions;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Services.Asset;

public sealed class DeleteDirectoryProvider : IDeleteDirectoryProvider {
    private const string DeleteDirectoryName = "_DELETE_";

    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;

    public DeleteDirectoryProvider(
        IFileSystem fileSystem,
        IIgnoredDirectoriesProvider ignoredDirectoriesProvider,
        IDataDirectoryProvider dataDirectoryProvider) {
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;
        ignoredDirectoriesProvider.AddIgnoredDirectory(new DataRelativePath(DeleteDirectoryName));
    }

    public string DeleteDirectory => _fileSystem.Path.Combine(_dataDirectoryProvider.Path, DeleteDirectoryName);
}
