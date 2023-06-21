using Mutagen.Bethesda.Archives;
namespace CreationEditor.Services.Archive;

public interface IArchiveService : IDisposable {
    /// <summary>
    /// The extension of the archive files this service can read.
    /// </summary>
    /// <returns>string of the file extension, including the starting dot</returns>
    string GetExtension();

    IArchiveReader GetReader(string path);
    IEnumerable<string> GetFilesInFolder(string path);
    IEnumerable<string> GetSubdirectories(string path);
    
    /// <summary>
    /// Tries to copy the content of a file in an archive to a temporary file.
    /// </summary>
    /// <param name="filePath">Path to the file in the archive</param>
    /// <returns>The path of the temporary file</returns>
    string? TryGetFileAsTempFile(string filePath);

    /// <summary>
    /// Archives ordered by their priority from low to high priority.
    /// </summary>
    public IReadOnlyList<string> Archives { get; }
}
