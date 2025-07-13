using System.Reactive.Linq;
using System.Reactive.Subjects;
using CreationEditor.Services.DataSource;
using Noggog;
using ReactiveMarbles.ObservableEvents;
namespace CreationEditor.Services.Asset;

public sealed class FileSystemDataSourceWatcher : IDataSourceWatcher {
    private readonly DisposableBucket _disposables = new();

    private readonly IIgnoredDirectoriesProvider _ignoredDirectoriesProvider;

    public IDataSource DataSource { get; }

    public IObservable<DataSourceLink> Created => _createdFile.Merge(_createdDirectory);
    public IObservable<DataSourceLink> CreatedFile => _createdFile;
    public IObservable<DataSourceLink> CreatedDirectory => _createdDirectory;
    private readonly Subject<DataSourceLink> _createdFile = new();
    private readonly Subject<DataSourceLink> _createdDirectory = new();

    public IObservable<DataSourceLink> Deleted => _deletedFile.Merge(_deletedDirectory);
    public IObservable<DataSourceLink> DeletedFile => _deletedFile;
    public IObservable<DataSourceLink> DeletedDirectory => _deletedDirectory;
    private readonly Subject<DataSourceLink> _deletedFile = new();
    private readonly Subject<DataSourceLink> _deletedDirectory = new();

    public IObservable<DataSourceLink> Changed => _changedFile.Merge(_changedDirectory);
    public IObservable<DataSourceLink> ChangedFile => _changedFile;
    public IObservable<DataSourceLink> ChangedDirectory => _changedDirectory;
    private readonly Subject<DataSourceLink> _changedFile = new();
    private readonly Subject<DataSourceLink> _changedDirectory = new();

    public IObservable<(DataSourceLink Old, DataSourceLink New)> Renamed => _renamedFile.Merge(_renamedDirectory);
    public IObservable<(DataSourceLink Old, DataSourceLink New)> RenamedFile => _renamedFile;
    public IObservable<(DataSourceLink Old, DataSourceLink New)> RenamedDirectory => _renamedDirectory;
    private readonly Subject<(DataSourceLink Old, DataSourceLink New)> _renamedFile = new();
    private readonly Subject<(DataSourceLink Old, DataSourceLink New)> _renamedDirectory = new();

    public FileSystemDataSourceWatcher(
        IDataSource dataSource,
        IIgnoredDirectoriesProvider ignoredDirectoriesProvider) {
        DataSource = dataSource;
        _ignoredDirectoriesProvider = ignoredDirectoriesProvider;
        var watcher = DataSource.FileSystem.FileSystemWatcher.New(DataSource.Path).DisposeWith(_disposables);
        // _watcher.Filters.AddRange(_assetTypeService.GetFileMasks()); doesn't allow directory changes
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        var fileSystemWatcherEvents = watcher.Events();
        HandleFileSystemEvent(fileSystemWatcherEvents.Created, _createdFile, _createdDirectory, ToDataSourceLink);
        HandleFileSystemEvent(fileSystemWatcherEvents.Deleted, _deletedFile, _deletedDirectory, ToDataSourceLink);
        HandleFileSystemEvent(fileSystemWatcherEvents.Changed, _changedFile, _changedDirectory, ToDataSourceLink);
        HandleFileSystemEvent(fileSystemWatcherEvents.Renamed, _renamedFile, _renamedDirectory, ToUpdatedDataSourceLink);

        DataSourceLink ToDataSourceLink(FileSystemEventArgs arg) {
            if (DataSource.TryGetLink(arg.FullPath, out var link)) {
                return link;
            }

            throw new InvalidOperationException($"File system link for path '{arg.FullPath}' not found in data source '{DataSource.Name}'.");
        }

        (DataSourceLink Old, DataSourceLink New) ToUpdatedDataSourceLink(RenamedEventArgs arg) {
            if (DataSource.TryGetLink(arg.OldFullPath, out var oldLink) &&
                DataSource.TryGetLink(arg.FullPath, out var newLink)) {
                return (Old: oldLink, New: newLink);
            }

            throw new InvalidOperationException(
                $"File system link for path '{arg.OldFullPath}' or '{arg.FullPath}' not found in data source '{DataSource.Name}'.");
        }
    }

    private void HandleFileSystemEvent<TEvent, T>(
        IObservable<TEvent> observable,
        IObserver<T> fileObserver,
        IObserver<T> directoryObserver,
        Func<TEvent, T> select)
        where TEvent : FileSystemEventArgs {
        observable
            .Where(Filter)
            .Subscribe(x => {
                if (DataSource.FileSystem.Directory.Exists(x.FullPath)) {
                    directoryObserver.OnNext(select(x));
                } else {
                    fileObserver.OnNext(select(x));
                }
            })
            .DisposeWith(_disposables);

        bool Filter(TEvent t) {
            if (!DataSource.TryGetDataRelativePath(t.FullPath, out var relativePath)) return false;

            return !_ignoredDirectoriesProvider.IsIgnored(relativePath);
        }
    }

    public void Dispose() => _disposables.Dispose();
}
