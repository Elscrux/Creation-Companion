using CreationEditor.Services.DataSource;
namespace CreationEditor.Services.Asset;

public interface IDataSourceWatcher : IDisposable {
    /// <summary>
    /// The data source that this watcher is watching.
    /// </summary>
    IDataSource DataSource { get; }

    /// <summary>
    /// Emits the event args of a newly created file or directory in the data source.
    /// </summary>
    IObservable<FileSystemEventArgs> Created { get; }
    /// <summary>
    /// Emits the event args of a newly created file in the data source.
    /// </summary>
    IObservable<FileSystemEventArgs> CreatedFile { get; }
    /// <summary>
    /// Emits the event args of a newly created directory in the data source.
    /// </summary>
    IObservable<FileSystemEventArgs> CreatedDirectory { get; }

    /// <summary>
    /// Emits the event args of a deleted file or directory in the data source.
    /// </summary>
    IObservable<FileSystemEventArgs> Deleted { get; }
    /// <summary>
    /// Emits the event args of a deleted file in the data source.
    /// </summary>
    IObservable<FileSystemEventArgs> DeletedFile { get; }
    /// <summary>
    /// Emits the event args of a deleted directory in the data source.
    /// </summary>
    IObservable<FileSystemEventArgs> DeletedDirectory { get; }

    /// <summary>
    /// Emits the event args of a changed file or directory in the data source.
    /// </summary>
    IObservable<FileSystemEventArgs> Changed { get; }
    /// <summary>
    /// Emits the event args of a changed file in the data source.
    /// </summary>
    IObservable<FileSystemEventArgs> ChangedFile { get; }
    /// <summary>
    /// Emits the event args of a changed directory in the data source.
    /// </summary>
    IObservable<FileSystemEventArgs> ChangedDirectory { get; }

    /// <summary>
    /// Emits the event args of a renamed file or directory in the data source.
    /// </summary>
    IObservable<RenamedEventArgs> Renamed { get; }
    /// <summary>
    /// Emits the event args of a renamed file in the data source.
    /// </summary>
    IObservable<RenamedEventArgs> RenamedFile { get; }
    /// <summary>
    /// Emits the event args of a renamed directory in the data source.
    /// </summary>
    IObservable<RenamedEventArgs> RenamedDirectory { get; }
}
