using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Archive;

public interface IArchiveService {
    /// <summary>
    /// The extension of the archive files this service can read.
    /// </summary>
    /// <returns>string of the file extension, including the starting dot</returns>
    string GetExtension();

    /// <summary>
    /// Get the load order of archives to load based on the game environment.
    /// </summary>
    /// <returns>Load order of archives sorted from highest to lowest priority</returns>
    IEnumerable<string> GetArchiveLoadOrder();

    /// <summary>
    /// Get an archive reader for an archive.
    /// </summary>
    /// <param name="link">File system link to the archive</param>
    /// <returns>Archive reader</returns>
    IArchiveReader GetReader(DataSourceLink link);

    /// <summary>
    /// Get files of archives inside a directory.
    /// </summary>
    /// <param name="archiveReader">Archive reader of the archive</param>
    /// <param name="directoryPath">Path to directory in the archive relative to the data directory</param>
    /// <returns>Enumerable of file paths in the directory relative to the data directory</returns>
    IEnumerable<string> GetFilesInDirectory(IArchiveReader archiveReader, DataRelativePath directoryPath);

    /// <summary>
    /// Get subdirectory of archive inside a directory.
    /// </summary>
    /// <param name="archiveReader">Archive reader of the archive</param>
    /// <param name="directoryPath">Path to directory in the archive relative to the data directory</param>
    /// <returns>Enumerable of subdirectories paths in the directory relative to the data directory</returns>
    IEnumerable<string> GetSubdirectories(IArchiveReader archiveReader, DataRelativePath directoryPath);

    /// <summary>
    /// Tries to get a file stream of a file in an archive.
    /// </summary>
    /// <param name="archiveReader">Archive reader of the archive</param>
    /// <param name="filePath">Full path to the file in the archive</param>
    /// <returns>FileStream of the file in the archive</returns>
    Stream? TryGetFileStream(IArchiveReader archiveReader, DataRelativePath filePath);

    /// <summary>
    /// Tries to copy the content of a file in an archive to a temporary file.
    /// </summary>
    /// <param name="archiveReader">Archive reader of the archive</param>
    /// <param name="filePath">Full path to the file in the archive</param>
    /// <returns>The full path of the temporary file</returns>
    string? TryGetFileAsTempFile(IArchiveReader archiveReader, DataRelativePath filePath);
}
