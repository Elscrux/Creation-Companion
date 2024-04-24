namespace CreationEditor.Services.Asset;

public interface IDataDirectoryService : IDisposable {
    /// <summary>
    /// Full path to the data directory.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Emits the event args of a newly created file or directory.
    /// </summary>
    IObservable<FileSystemEventArgs> Created { get; }
    /// <summary>
    /// Emits the event args of a newly created file.
    /// </summary>
    IObservable<FileSystemEventArgs> CreatedFile { get; }
    /// <summary>
    /// Emits the event args of a newly created directory.
    /// </summary>
    IObservable<FileSystemEventArgs> CreatedDirectory { get; }

    /// <summary>
    /// Emits the event args of a deleted file or directory.
    /// </summary>
    IObservable<FileSystemEventArgs> Deleted { get; }
    /// <summary>
    /// Emits the event args of a deleted file.
    /// </summary>
    IObservable<FileSystemEventArgs> DeletedFile { get; }
    /// <summary>
    /// Emits the event args of a deleted directory.
    /// </summary>
    IObservable<FileSystemEventArgs> DeletedDirectory { get; }

    /// <summary>
    /// Emits the event args of a changed file or directory.
    /// </summary>
    IObservable<FileSystemEventArgs> Changed { get; }
    /// <summary>
    /// Emits the event args of a changed file.
    /// </summary>
    IObservable<FileSystemEventArgs> ChangedFile { get; }
    /// <summary>
    /// Emits the event args of a changed directory.
    /// </summary>
    IObservable<FileSystemEventArgs> ChangedDirectory { get; }

    /// <summary>
    /// Emits the event args of a renamed file or directory.
    /// </summary>
    IObservable<RenamedEventArgs> Renamed { get; }
    /// <summary>
    /// Emits the event args of a renamed file.
    /// </summary>
    IObservable<RenamedEventArgs> RenamedFile { get; }
    /// <summary>
    /// Emits the event args of a renamed directory.
    /// </summary>
    IObservable<RenamedEventArgs> RenamedDirectory { get; }
}
