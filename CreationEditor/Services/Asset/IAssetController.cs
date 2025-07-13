using CreationEditor.Services.DataSource;
namespace CreationEditor.Services.Asset;

public interface IAssetController {
    /// <summary>
    /// Open a file or directory.
    /// </summary>
    /// <param name="link">Link to the file or directory that should be opened</param>
    void Open(FileSystemLink link);

    /// <summary>
    /// Move a file or directory to a new location.
    /// </summary>
    /// <param name="origin">Link to the file or directory that should be moved</param>
    /// <param name="destination">Link to the new location of the file or directory</param>
    /// <param name="token">A CancellationToken can be used to cancel the task as long as it hasn't modified anything</param>
    void Move(FileSystemLink origin, FileSystemLink destination, CancellationToken token = default);

    /// <summary>
    /// Copy a file or directory to a new location.
    /// </summary>
    /// <param name="origin">Link to the file or directory that should be copied</param>
    /// <param name="destination">Link to the new location of the file or directory</param>
    /// <param name="token">A CancellationToken can be used to cancel the task as long as it hasn't modified anything</param>
    void Copy(FileSystemLink origin, FileSystemLink destination, CancellationToken token = default);

    /// <summary>
    /// Rename a file or directory.
    /// </summary>
    /// <param name="origin">Link to the file or directory to rename</param>
    /// <param name="newName">New name of the file or directory</param>
    /// <param name="token">A CancellationToken can be used to cancel the task as long as it hasn't modified anything</param>
    void Rename(FileSystemLink origin, string newName, CancellationToken token = default);

    /// <summary>
    /// Delete a file or directory.
    /// </summary>
    /// <param name="link">Link to the file or directory to delete</param>
    /// <param name="token">A CancellationToken can be used to cancel the task as long as it hasn't modified anything</param>
    void Delete(FileSystemLink link, CancellationToken token = default);

    /// <summary>
    /// Remap references to a directory to a new directory.
    /// </summary>
    /// <param name="oldLink">Link to the directory that should be remapped</param>
    /// <param name="newLink">Link to the new directory</param>
    void RemapDirectoryReferences(FileSystemLink oldLink, FileSystemLink newLink);

    /// <summary>
    /// Remap references to a file to a new file.
    /// </summary>
    /// <param name="oldLink">Link to the file that should be remapped</param>
    /// <param name="newLink">Link to the new file</param>
    void RemapFileReferences(FileSystemLink oldLink, FileSystemLink newLink);

    void Redo();
    void Undo();
}
