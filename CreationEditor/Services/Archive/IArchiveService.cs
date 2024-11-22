using Mutagen.Bethesda.Archives;
namespace CreationEditor.Services.Archive;

public interface IArchiveService : IDisposable {
    /// <summary>
    /// The extension of the archive files this service can read.
    /// </summary>
    /// <returns>string of the file extension, including the starting dot</returns>
    string GetExtension();

    /// <summary>
    /// Get an archive reader for an archive.
    /// </summary>
    /// <param name="path">Full path to the archive</param>
    /// <returns>Archive reader</returns>
    IArchiveReader GetReader(string path);

    /// <summary>
    /// Get files of archives inside a directory.
    /// </summary>
    /// <param name="directoryPath">Path to directory in archives relative to the data directory</param>
    /// <returns>Enumerable of file paths in the directory relative to the data directory</returns>
    IEnumerable<string> GetFilesInDirectory(string directoryPath);

    /// <summary>
    /// Get subdirectory of archives inside a directory.
    /// </summary>
    /// <param name="directoryPath">Path to directory in archives relative to the data directory</param>
    /// <returns>Enumerable of subdirectories paths in the directory relative to the data directory</returns>
    IEnumerable<string> GetSubdirectories(string directoryPath);

    /// <summary>
    /// Tries to get a file stream of a file in an archive.
    /// </summary>
    /// <param name="filePath">Full path to the file in the archive</param>
    /// <returns>FileStream of the file in the archive</returns>
    Stream? TryGetFileStream(string filePath);

    /// <summary>
    /// Tries to copy the content of a file in an archive to a temporary file.
    /// </summary>
    /// <param name="filePath">Full path to the file in the archive</param>
    /// <returns>The full path of the temporary file</returns>
    string? TryGetFileAsTempFile(string filePath);

    /// <summary>
    /// Names of archives ordered by their priority from low to high priority.
    /// </summary>
    IReadOnlyList<string> Archives { get; }

    /// <summary>
    /// Emits the name of a newly created archives.
    /// </summary>
    IObservable<string> ArchiveCreated { get; }

    /// <summary>
    /// Emits the name of deleted archives.
    /// </summary>
    IObservable<string> ArchiveDeleted { get; }

    /// <summary>
    /// Emits the name of changed archives.
    /// </summary>
    IObservable<string> ArchiveChanged { get; }

    /// <summary>
    /// Emits the old and new name of renamed archives.
    /// </summary>
    IObservable<(string OldName, string NewName)> ArchiveRenamed { get; }
}
