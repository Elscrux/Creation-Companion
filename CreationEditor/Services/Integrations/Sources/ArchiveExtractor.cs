using System.IO.Abstractions;
using SharpCompress.Archives;
using SharpCompress.Common;
namespace CreationEditor.Services.Integrations.Sources;

/// <summary>
/// Extracts downloaded archives (zip, 7z) into a target folder and locates the primary executable
/// within the extracted contents.
/// </summary>
public sealed class ArchiveExtractor(IFileSystem fileSystem) {
    private static readonly string[] ArchiveExtensions = [".zip", ".7z"];

    /// <summary>Whether the given file path is a supported archive.</summary>
    public bool IsArchive(string path) {
        var extension = fileSystem.Path.GetExtension(path);
        return ArchiveExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Extracts the archive at <paramref name="archivePath"/> into <paramref name="destinationFolder"/>.
    /// Returns the path to the primary executable found within the extracted contents, or null if none found.
    /// </summary>
    public string? Extract(string archivePath, string destinationFolder) {
        fileSystem.Directory.CreateDirectory(destinationFolder);

        ArchiveFactory.WriteToDirectory(archivePath,
            destinationFolder,
            new ExtractionOptions {
                ExtractFullPath = true,
                Overwrite = true,
            });

        return FindExecutable(destinationFolder);
    }

    /// <summary>
    /// Finds the primary executable within the extracted contents. Prefers a top-level executable,
    /// then searches recursively for the first executable file.
    /// </summary>
    private string? FindExecutable(string folder) {
        var topLevel = fileSystem.Directory
            .GetFiles(folder, "*.exe", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();

        if (topLevel is not null) return topLevel;

        return fileSystem.Directory
            .GetFiles(folder, "*.exe", SearchOption.AllDirectories)
            .FirstOrDefault();
    }
}
