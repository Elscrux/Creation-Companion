using System.IO.Abstractions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments.DI;
using Noggog;
using ReactiveMarbles.ObservableEvents;
namespace CreationEditor.Services.Asset;

public sealed class DataDirectoryService : IDataDirectoryService {
    private readonly DisposableBucket _disposables = new();

    private readonly IAssetTypeService _assetTypeService;
    private readonly IDeleteDirectoryProvider _deleteDirectoryProvider;
    private readonly IFileSystem _fileSystem;

    public string Path { get; }

    public IObservable<FileSystemEventArgs> Created => _createdFile.Merge(_createdDirectory);
    public IObservable<FileSystemEventArgs> CreatedFile => _createdFile;
    public IObservable<FileSystemEventArgs> CreatedDirectory => _createdDirectory;
    private readonly Subject<FileSystemEventArgs> _createdFile = new();
    private readonly Subject<FileSystemEventArgs> _createdDirectory = new();

    public IObservable<FileSystemEventArgs> Deleted => _deletedFile.Merge(_deletedDirectory);
    public IObservable<FileSystemEventArgs> DeletedFile => _deletedFile;
    public IObservable<FileSystemEventArgs> DeletedDirectory => _deletedDirectory;
    private readonly Subject<FileSystemEventArgs> _deletedFile = new();
    private readonly Subject<FileSystemEventArgs> _deletedDirectory = new();

    public IObservable<FileSystemEventArgs> Changed => _changedFile.Merge(_changedDirectory);
    public IObservable<FileSystemEventArgs> ChangedFile => _changedFile;
    public IObservable<FileSystemEventArgs> ChangedDirectory => _changedDirectory;
    private readonly Subject<FileSystemEventArgs> _changedFile = new();
    private readonly Subject<FileSystemEventArgs> _changedDirectory = new();

    public IObservable<RenamedEventArgs> Renamed => _renamedFile.Merge(_renamedDirectory);
    public IObservable<RenamedEventArgs> RenamedFile => _renamedFile;
    public IObservable<RenamedEventArgs> RenamedDirectory => _renamedDirectory;
    private readonly Subject<RenamedEventArgs> _renamedFile = new();
    private readonly Subject<RenamedEventArgs> _renamedDirectory = new();

    public DataDirectoryService(
        IDataDirectoryProvider dataDirectoryProvider,
        IDeleteDirectoryProvider deleteDirectoryProvider,
        IFileSystem fileSystem,
        IAssetTypeService assetTypeService) {
        Path = dataDirectoryProvider.Path;
        _deleteDirectoryProvider = deleteDirectoryProvider;
        _fileSystem = fileSystem;
        _assetTypeService = assetTypeService;
        var watcher = _fileSystem.FileSystemWatcher.New(Path).DisposeWith(_disposables);
        // _watcher.Filters.AddRange(_assetTypeService.GetFileMasks()); doesn't allow directory changes
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        var fileSystemWatcherEvents = watcher.Events();
        HandleFileSystemEvent(fileSystemWatcherEvents.Created, _createdFile, _createdDirectory);
        HandleFileSystemEvent(fileSystemWatcherEvents.Deleted, _deletedFile, _deletedDirectory);
        HandleFileSystemEvent(fileSystemWatcherEvents.Changed, _changedFile, _changedDirectory);
        HandleFileSystemEvent(fileSystemWatcherEvents.Renamed, _renamedFile, _renamedDirectory);
    }

    private void HandleFileSystemEvent<T>(IObservable<T> observable, IObserver<T> fileObserver, IObserver<T> directoryObserver)
        where T : FileSystemEventArgs {
        observable
            .Where(Filter)
            .Subscribe(x => {
                if (_fileSystem.Directory.Exists(x.FullPath)) {
                    directoryObserver.OnNext(x);
                } else {
                    var extension = _fileSystem.Path.GetExtension(x.FullPath);
                    if (_assetTypeService.FileExtensions.Contains(extension)) {
                        fileObserver.OnNext(x);
                    }
                }
            })
            .DisposeWith(_disposables);

        bool Filter(T t) {
            // Ignore changes in the delete directory
            if (t.FullPath.StartsWith(_deleteDirectoryProvider.DeleteDirectory, DataRelativePath.PathComparison)) return false;

            return true;
        }
    }

    public void Dispose() => _disposables.Dispose();
}
