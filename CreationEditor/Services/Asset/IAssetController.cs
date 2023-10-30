namespace CreationEditor.Services.Asset;

public interface IAssetController {
    /// <summary>
    /// Move a file or directory to a new location.
    /// </summary>
    /// <param name="path">Path of the file or directory that should be moved relative to the data directory</param>
    /// <param name="destination">Path of the new location of the file or directory, relative to the data directory</param>
    /// <param name="token">A CancellationToken can be used to cancel the task as long as it hasn't modified anything</param>
    void Move(string path, string destination, CancellationToken token = default);

    /// <summary>
    /// Rename a file or directory.
    /// </summary>
    /// <param name="path">Path to the file or directory to rename, relative to the data directory</param>
    /// <param name="newName">New name of the file or directory</param>
    /// <param name="token">A CancellationToken can be used to cancel the task as long as it hasn't modified anything</param>
    void Rename(string path, string newName, CancellationToken token = default);

    /// <summary>
    /// Delete a file or directory.
    /// </summary>
    /// <param name="path">Path to the file or directory to delete, relative to the data directory</param>
    /// <param name="token">A CancellationToken can be used to cancel the task as long as it hasn't modified anything</param>
    void Delete(string path, CancellationToken token = default);

    void Redo();
    void Undo();
}
