using System.Reactive.Linq;
using System.Reactive.Subjects;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using Noggog;
using ReactiveMarbles.ObservableEvents;
namespace CreationEditor.Services.Asset;

public sealed class FileSystemDataSourceWatcher : IDataSourceWatcher {
    private readonly DisposableBucket _disposables = new();

    private readonly IDeleteDirectoryProvider _deleteDirectoryProvider;
    private readonly IAssetTypeService _assetTypeService;

    public IDataSource DataSource { get; }

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

    public FileSystemDataSourceWatcher(
        IDataSource dataSource,
        IDeleteDirectoryProvider deleteDirectoryProvider,
        IAssetTypeService assetTypeService) {
        DataSource = dataSource;
        _deleteDirectoryProvider = deleteDirectoryProvider;
        _assetTypeService = assetTypeService;
        var watcher = dataSource.FileSystem.FileSystemWatcher.New(DataSource.Path).DisposeWith(_disposables);
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
                if (DataSource.FileSystem.Directory.Exists(x.FullPath)) {
                    directoryObserver.OnNext(x);
                } else {
                    var extension = DataSource.FileSystem.Path.GetExtension(x.FullPath);
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
