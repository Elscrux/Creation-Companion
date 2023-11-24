namespace CreationEditor.Services.Asset;

public interface IDataDirectoryService : IDisposable {
    /// <summary>
    /// Full path to the data directory.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// Emits the event args of a newly created file.
    /// </summary>
    IObservable<FileSystemEventArgs> Created { get; }

    /// <summary>
    /// Emits the event args of a deleted file.
    /// </summary>
    IObservable<FileSystemEventArgs> Deleted { get; }

    /// <summary>
    /// Emits the event args of a changed file.
    /// </summary>
    IObservable<FileSystemEventArgs> Changed { get; }

    /// <summary>
    /// Emits the event args of a renamed file.
    /// </summary>
    IObservable<RenamedEventArgs> Renamed { get; }
}
