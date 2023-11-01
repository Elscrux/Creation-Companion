using System.IO.Abstractions;
using System.Reactive.Linq;
using Mutagen.Bethesda.Environments.DI;
using Noggog;
using ReactiveMarbles.ObservableEvents;
namespace CreationEditor.Services.Asset;

public sealed class DataDirectoryService : IDataDirectoryService {
    private readonly IDisposableDropoff _disposables = new DisposableBucket();

    private readonly IAssetTypeService _assetTypeService;
    private readonly IFileSystem _fileSystem;

    public string Path { get; }

    public IObservable<FileSystemEventArgs> Created { get; }
    public IObservable<FileSystemEventArgs> Deleted { get; }
    public IObservable<FileSystemEventArgs> Changed { get; }
    public IObservable<RenamedEventArgs> Renamed { get; }

    public DataDirectoryService(
        IDataDirectoryProvider dataDirectoryProvider,
        IDeleteDirectoryProvider deleteDirectoryProvider,
        IFileSystem fileSystem,
        IAssetTypeService assetTypeService) {
        Path = dataDirectoryProvider.Path;
        var deleteDirectoryProvider1 = deleteDirectoryProvider;
        _fileSystem = fileSystem;
        _assetTypeService = assetTypeService;
        var watcher = _fileSystem.FileSystemWatcher.New(Path).DisposeWith(_disposables);
        // _watcher.Filters.AddRange(_assetTypeService.GetFileMasks()); doesn't allow directory changes
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        Created = watcher.Events().Created
            .Where(e =>
                !string.Equals(e.FullPath, deleteDirectoryProvider1.DeleteDirectory, AssetCompare.PathComparison)
             && !_fileSystem.Path.HasExtension(e.FullPath) || IsFileRelevant(e.FullPath));

        Deleted = watcher.Events().Deleted
            .Where(e =>
                !string.Equals(e.FullPath, deleteDirectoryProvider1.DeleteDirectory, AssetCompare.PathComparison)
             && !_fileSystem.Path.HasExtension(e.FullPath) || IsFileRelevant(e.FullPath));

        Changed = watcher.Events().Changed
            .Where(e =>
                !string.Equals(e.FullPath, deleteDirectoryProvider1.DeleteDirectory, AssetCompare.PathComparison)
             && !_fileSystem.Path.HasExtension(e.FullPath) || IsFileRelevant(e.FullPath));

        Renamed = watcher.Events().Renamed
            .Where(e =>
                !string.Equals(e.FullPath, deleteDirectoryProvider1.DeleteDirectory, AssetCompare.PathComparison)
             && !_fileSystem.Path.HasExtension(e.FullPath) || IsFileRelevant(e.FullPath));
    }

    private bool IsFileRelevant(string file) {
        var extension = _fileSystem.Path.GetExtension(file);

        return _assetTypeService.FileExtensions.Contains(extension);
    }

    public void Dispose() => _disposables.Dispose();
}
