using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Services.Asset;

public class DeleteDirectoryProvider : IDeleteDirectoryProvider {
    private const string DeleteDirectoryName = "_DELETE_";

    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;

    public string DeleteDirectory => _fileSystem.Path.Combine(_dataDirectoryProvider.Path, DeleteDirectoryName);

    public DeleteDirectoryProvider(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider) {
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;
    }
}
