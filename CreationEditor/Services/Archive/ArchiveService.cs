using System.IO.Abstractions;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Services.Archive;

public sealed class ArchiveService(
    IFileSystem fileSystem,
    IGetArchiveIniListings archiveIniListings,
    IArchiveExtensionProvider archiveExtensionProvider,
    IEditorEnvironment editorEnvironment)
    : IArchiveService {
    private readonly Dictionary<DataSourceLink, IArchiveReader> _archiveReaders = new();
    private readonly Dictionary<IArchiveReader, IReadOnlyList<string>> _archiveDirectories = new();

    public string GetExtension() => archiveExtensionProvider.Get();

    public IEnumerable<string> GetArchiveLoadOrder() {
        return archiveIniListings.Get().Select(fileName => fileName.String)
            .Concat(editorEnvironment.LinkCache.ListedOrder.SelectMany(GetModArchiveFiles))
            .Distinct();

        IEnumerable<string> GetModArchiveFiles(IModGetter mod) {
            var extension = GetExtension();

            yield return mod.ModKey.Name + extension;
            yield return mod.ModKey.Name + " - Textures" + extension;
        }
    }

    public IArchiveReader GetReader(DataSourceLink link) {
        if (_archiveReaders.TryGetValue(link, out var reader)) return reader;

        var archiveReader =
            global::Mutagen.Bethesda.Archives.Archive.CreateReader(editorEnvironment.GameEnvironment.GameRelease, link.FullPath, link.FileSystem);
        _archiveReaders.Add(link, archiveReader);
        return archiveReader;
    }

    private IArchiveFile? GetArchiveFile(IArchiveReader archiveReader, DataRelativePath filePath) {
        // Search for file in archives
        var directoryPath = fileSystem.Path.GetDirectoryName(filePath.Path);
        if (directoryPath is null) return null;

        if (!archiveReader.TryGetFolder(directoryPath, out var archiveFolder)) return null;

        return archiveFolder.Files.FirstOrDefault(f => DataRelativePath.PathComparer.Equals(f.Path, filePath));
    }

    public Stream? TryGetFileStream(IArchiveReader archiveReader, DataRelativePath filePath) {
        var archiveFile = GetArchiveFile(archiveReader, filePath);
        return archiveFile?.AsStream();
    }

    public string? TryGetFileAsTempFile(IArchiveReader archiveReader, DataRelativePath filePath) {
        var archiveFile = GetArchiveFile(archiveReader, filePath);
        if (archiveFile is null) return null;

        var tempFilePath = fileSystem.Path.GetTempFileName() + fileSystem.Path.GetExtension(filePath.Path);
        using var tempFileStream = fileSystem.File.Create(tempFilePath);
        archiveFile.AsStream().CopyTo(tempFileStream);

        return tempFilePath;
    }

    public IEnumerable<string> GetFilesInDirectory(IArchiveReader archiveReader, DataRelativePath directoryPath) {
        if (!archiveReader.TryGetFolder(directoryPath.Path, out var archiveFolder)) yield break;

        foreach (var archiveFile in archiveFolder.Files) {
            yield return archiveFile.Path;
        }
    }

    public IEnumerable<string> GetSubdirectories(IArchiveReader archiveReader, DataRelativePath directoryPath) {
        // Compile directories in archive
        if (!_archiveDirectories.TryGetValue(archiveReader, out var archiveDirectories)) {
            archiveDirectories = archiveReader.Files
                .Select(archiveFile => fileSystem.Path.GetDirectoryName(archiveFile.Path))
                .WhereNotNull()
                .Distinct()
                .SelectMany(DirectoriesSelector)
                .Distinct()
                .ToArray();

            _archiveDirectories.Add(archiveReader, archiveDirectories);
        }

        return archiveDirectories
            .Where(dir => {
                if (!directoryPath.Path.StartsWith(dir, DataRelativePath.PathComparison)) return false;

                var relativePath = fileSystem.Path.GetRelativePath(dir, directoryPath.Path);
                return relativePath.Split(fileSystem.Path.DirectorySeparatorChar, fileSystem.Path.AltDirectorySeparatorChar).Length == 1;
            });

        IEnumerable<string> DirectoriesSelector(string dirPath) {
            var directoryNames = dirPath.Split(fileSystem.Path.DirectorySeparatorChar, fileSystem.Path.AltDirectorySeparatorChar);

            var currentPath = string.Empty;
            foreach (var directoryName in directoryNames) {
                currentPath = fileSystem.Path.Combine(currentPath, directoryName);
                yield return currentPath;
            }
        }
    }
}
