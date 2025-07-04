﻿using System.Reactive.Linq;
using System.Reactive.Subjects;
using CreationEditor.Services.DataSource;
using Noggog;
using ReactiveMarbles.ObservableEvents;
namespace CreationEditor.Services.Asset;

public sealed class FileSystemDataSourceWatcher : IDataSourceWatcher {
    private readonly DisposableBucket _disposables = new();

    private readonly IIgnoredDirectoriesProvider _ignoredDirectoriesProvider;

    public IDataSource DataSource { get; }

    public IObservable<FileSystemLink> Created => _createdFile.Merge(_createdDirectory);
    public IObservable<FileSystemLink> CreatedFile => _createdFile;
    public IObservable<FileSystemLink> CreatedDirectory => _createdDirectory;
    private readonly Subject<FileSystemLink> _createdFile = new();
    private readonly Subject<FileSystemLink> _createdDirectory = new();

    public IObservable<FileSystemLink> Deleted => _deletedFile.Merge(_deletedDirectory);
    public IObservable<FileSystemLink> DeletedFile => _deletedFile;
    public IObservable<FileSystemLink> DeletedDirectory => _deletedDirectory;
    private readonly Subject<FileSystemLink> _deletedFile = new();
    private readonly Subject<FileSystemLink> _deletedDirectory = new();

    public IObservable<FileSystemLink> Changed => _changedFile.Merge(_changedDirectory);
    public IObservable<FileSystemLink> ChangedFile => _changedFile;
    public IObservable<FileSystemLink> ChangedDirectory => _changedDirectory;
    private readonly Subject<FileSystemLink> _changedFile = new();
    private readonly Subject<FileSystemLink> _changedDirectory = new();

    public IObservable<(FileSystemLink Old, FileSystemLink New)> Renamed => _renamedFile.Merge(_renamedDirectory);
    public IObservable<(FileSystemLink Old, FileSystemLink New)> RenamedFile => _renamedFile;
    public IObservable<(FileSystemLink Old, FileSystemLink New)> RenamedDirectory => _renamedDirectory;
    private readonly Subject<(FileSystemLink Old, FileSystemLink New)> _renamedFile = new();
    private readonly Subject<(FileSystemLink Old, FileSystemLink New)> _renamedDirectory = new();

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
        HandleFileSystemEvent(fileSystemWatcherEvents.Created, _createdFile, _createdDirectory, ToFileSystemLink);
        HandleFileSystemEvent(fileSystemWatcherEvents.Deleted, _deletedFile, _deletedDirectory, ToFileSystemLink);
        HandleFileSystemEvent(fileSystemWatcherEvents.Changed, _changedFile, _changedDirectory, ToFileSystemLink);
        HandleFileSystemEvent(fileSystemWatcherEvents.Renamed, _renamedFile, _renamedDirectory, ToUpdatedFileSystemLink);

        FileSystemLink ToFileSystemLink(FileSystemEventArgs arg) {
            if (DataSource.TryGetFileSystemLink(arg.FullPath, out var link)) {
                return link;
            }

            throw new InvalidOperationException($"File system link for path '{arg.FullPath}' not found in data source '{DataSource.Name}'.");
        }

        (FileSystemLink Old, FileSystemLink New) ToUpdatedFileSystemLink(RenamedEventArgs arg) {
            if (DataSource.TryGetFileSystemLink(arg.OldFullPath, out var oldLink) &&
                DataSource.TryGetFileSystemLink(arg.FullPath, out var newLink)) {
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
