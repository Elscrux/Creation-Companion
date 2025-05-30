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
    IObservable<FileSystemLink> Created { get; }
    /// <summary>
    /// Emits the event args of a newly created file in the data source.
    /// </summary>
    IObservable<FileSystemLink> CreatedFile { get; }
    /// <summary>
    /// Emits the event args of a newly created directory in the data source.
    /// </summary>
    IObservable<FileSystemLink> CreatedDirectory { get; }

    /// <summary>
    /// Emits the event args of a deleted file or directory in the data source.
    /// </summary>
    IObservable<FileSystemLink> Deleted { get; }
    /// <summary>
    /// Emits the event args of a deleted file in the data source.
    /// </summary>
    IObservable<FileSystemLink> DeletedFile { get; }
    /// <summary>
    /// Emits the event args of a deleted directory in the data source.
    /// </summary>
    IObservable<FileSystemLink> DeletedDirectory { get; }

    /// <summary>
    /// Emits the event args of a changed file or directory in the data source.
    /// </summary>
    IObservable<FileSystemLink> Changed { get; }
    /// <summary>
    /// Emits the event args of a changed file in the data source.
    /// </summary>
    IObservable<FileSystemLink> ChangedFile { get; }
    /// <summary>
    /// Emits the event args of a changed directory in the data source.
    /// </summary>
    IObservable<FileSystemLink> ChangedDirectory { get; }

    /// <summary>
    /// Emits the event args of a renamed file or directory in the data source.
    /// </summary>
    IObservable<(FileSystemLink Old, FileSystemLink New)> Renamed { get; }
    /// <summary>
    /// Emits the event args of a renamed file in the data source.
    /// </summary>
    IObservable<(FileSystemLink Old, FileSystemLink New)> RenamedFile { get; }
    /// <summary>
    /// Emits the event args of a renamed directory in the data source.
    /// </summary>
    IObservable<(FileSystemLink Old, FileSystemLink New)> RenamedDirectory { get; }
}
