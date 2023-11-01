namespace CreationEditor.Services.Asset;

public interface IDataDirectoryService : IDisposable {
    string Path { get; }

    IObservable<FileSystemEventArgs> Created { get; }
    IObservable<FileSystemEventArgs> Deleted { get; }
    IObservable<FileSystemEventArgs> Changed { get; }
    IObservable<RenamedEventArgs> Renamed { get; }
}
