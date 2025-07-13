using System.Reactive.Linq;
using System.Reactive.Subjects;
using CreationEditor.Services.DataSource;
using Noggog;
using ReactiveMarbles.ObservableEvents;
namespace CreationEditor.Services.Asset;

public interface IUpdate<out T> {
    T Old { get; }
    T New { get; }
}

public sealed record Update<T>(T Old, T New) : IUpdate<T>;

public sealed class FileSystemDataSourceWatcher : IDataSourceWatcher {
    private readonly DisposableBucket _disposables = new();

    private readonly IIgnoredDirectoriesProvider _ignoredDirectoriesProvider;

    public IDataSource DataSource { get; }

    public IObservable<IDataSourceLink> Created => CreatedFile.Merge<IDataSourceLink>(CreatedDirectory);
    public IObservable<DataSourceFileLink> CreatedFile => _createdFile;
    public IObservable<DataSourceDirectoryLink> CreatedDirectory => _createdDirectory;
    private readonly Subject<DataSourceFileLink> _createdFile = new();
    private readonly Subject<DataSourceDirectoryLink> _createdDirectory = new();

    public IObservable<IDataSourceLink> Deleted => _deletedFile.Merge<IDataSourceLink>(_deletedDirectory);
    public IObservable<DataSourceFileLink> DeletedFile => _deletedFile;
    public IObservable<DataSourceDirectoryLink> DeletedDirectory => _deletedDirectory;
    private readonly Subject<DataSourceFileLink> _deletedFile = new();
    private readonly Subject<DataSourceDirectoryLink> _deletedDirectory = new();

    public IObservable<IDataSourceLink> Changed => _changedFile.Merge<IDataSourceLink>(_changedDirectory);
    public IObservable<DataSourceFileLink> ChangedFile => _changedFile;
    public IObservable<DataSourceDirectoryLink> ChangedDirectory => _changedDirectory;
    private readonly Subject<DataSourceFileLink> _changedFile = new();
    private readonly Subject<DataSourceDirectoryLink> _changedDirectory = new();

    public IObservable<IUpdate<IDataSourceLink>> Renamed => RenamedFile.Merge<IUpdate<IDataSourceLink>>(RenamedDirectory);
    public IObservable<IUpdate<DataSourceFileLink>> RenamedFile => _renamedFile;
    public IObservable<IUpdate<DataSourceDirectoryLink>> RenamedDirectory => _renamedDirectory;
    private readonly Subject<IUpdate<DataSourceFileLink>> _renamedFile = new();
    private readonly Subject<IUpdate<DataSourceDirectoryLink>> _renamedDirectory = new();

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
        HandleFileSystemEvent(fileSystemWatcherEvents.Created, _createdFile, _createdDirectory, ToDataSourceFileLink, ToDataSourceDirectoryLink);
        HandleFileSystemEvent(fileSystemWatcherEvents.Deleted, _deletedFile, _deletedDirectory, ToDataSourceFileLink, ToDataSourceDirectoryLink);
        HandleFileSystemEvent(fileSystemWatcherEvents.Changed, _changedFile, _changedDirectory, ToDataSourceFileLink, ToDataSourceDirectoryLink);
        HandleFileSystemEvent(fileSystemWatcherEvents.Renamed, _renamedFile, _renamedDirectory, ToUpdatedDataSourceFileLink, ToUpdatedDataSourceDirectoryLink);

        DataSourceFileLink ToDataSourceFileLink(FileSystemEventArgs arg) {
            if (DataSource.TryGetFileLink(arg.FullPath, out var link)) {
                return link;
            }

            throw new InvalidOperationException($"File system link for path '{arg.FullPath}' not found in data source '{DataSource.Name}'.");
        }

        DataSourceDirectoryLink ToDataSourceDirectoryLink(FileSystemEventArgs arg) {
            if (DataSource.TryGetDirectoryLink(arg.FullPath, out var link)) {
                return link;
            }

            throw new InvalidOperationException($"File system link for path '{arg.FullPath}' not found in data source '{DataSource.Name}'.");
        }

        IUpdate<DataSourceFileLink> ToUpdatedDataSourceFileLink(RenamedEventArgs arg) {
            if (DataSource.TryGetFileLink(arg.OldFullPath, out var oldLink) &&
                DataSource.TryGetFileLink(arg.FullPath, out var newLink)) {
                return new Update<DataSourceFileLink>(oldLink, newLink);
            }

            throw new InvalidOperationException(
                $"File system link for path '{arg.OldFullPath}' or '{arg.FullPath}' not found in data source '{DataSource.Name}'.");
        }

        IUpdate<DataSourceDirectoryLink> ToUpdatedDataSourceDirectoryLink(RenamedEventArgs arg) {
            if (DataSource.TryGetDirectoryLink(arg.OldFullPath, out var oldLink) &&
                DataSource.TryGetDirectoryLink(arg.FullPath, out var newLink)) {
                return new Update<DataSourceDirectoryLink>(oldLink, newLink);
            }

            throw new InvalidOperationException(
                $"File system link for path '{arg.OldFullPath}' or '{arg.FullPath}' not found in data source '{DataSource.Name}'.");
        }
    }

    private void HandleFileSystemEvent<TEvent, TFile, TDirectory>(
        IObservable<TEvent> observable,
        Subject<TFile> fileObserver,
        Subject<TDirectory> directoryObserver,
        Func<TEvent, TFile> selectFile,
        Func<TEvent, TDirectory> selectDir)
        where TEvent : FileSystemEventArgs {
        observable
            .Where(Filter)
            .Subscribe(x => {
                if (DataSource.FileSystem.Directory.Exists(x.FullPath)) {
                    directoryObserver.OnNext(selectDir(x));
                } else {
                    fileObserver.OnNext(selectFile(x));
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
