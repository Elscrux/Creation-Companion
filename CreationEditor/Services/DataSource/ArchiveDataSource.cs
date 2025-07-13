using System.IO.Abstractions;
using CreationEditor.Services.Filter;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

/// <summary>
/// A data source that represents a game archive.
/// </summary>
public sealed class ArchiveDataSource : IDataSource {
    private readonly WildcardSearchFilter _searchFilter = new();

    public IArchiveReader ArchiveReader { get; }
    public DataSourceLink ArchiveLink { get; }

    public string Name { get; }
    public string Path { get; }
    public bool IsReadOnly => true;
    public IFileSystem FileSystem { get; }

    public ArchiveDataSource(IFileSystem fileSystem, string fullPath, IArchiveReader archiveReader, DataSourceLink archiveLink) {
        ArchiveReader = archiveReader;
        ArchiveLink = archiveLink;
        FileSystem = fileSystem;
        Name = FileSystem.Path.GetFileName(fullPath);
        Path = fullPath;
    }

    public DataSourceMemento CreateMemento() {
        return new DataSourceMemento(DataSourceType.Archive, Name, Path, IsReadOnly);
    }

    public DataSourceLink GetRootLink() => ArchiveLink;

    public string GetFullPath(DataRelativePath path) {
        return FileSystem.Path.Combine(Path, path.Path);
    }

    public bool FileExists(DataRelativePath path) {
        var directoryPath = FileSystem.Path.GetDirectoryName(path.Path);
        if (directoryPath is null) return false;
        if (!ArchiveReader.TryGetFolder(directoryPath, out var folder)) return false;

        var fileName = FileSystem.Path.GetFileName(path.Path);
        return folder.Files.Any(f => DataRelativePath.PathComparer.Equals(FileSystem.Path.GetFileName(f.Path), fileName));
    }

    public bool DirectoryExists(DataRelativePath path) {
        var directoryPath = FileSystem.Path.GetDirectoryName(path.Path);
        if (directoryPath is null) return false;

        return ArchiveReader.TryGetFolder(directoryPath, out _);
    }
    public IEnumerable<DataRelativePath> EnumerateFiles(DataRelativePath path, string searchPattern = "*", bool includeSubDirectories = false) {
        var directoryPath = FileSystem.Path.GetDirectoryName(path.Path);
        if (directoryPath is null || !ArchiveReader.TryGetFolder(directoryPath, out var folder)) yield break;

        foreach (var file in folder.Files.Select(x => x.Path)) {
            if (!_searchFilter.Filter(file, searchPattern)) continue;

            var relativePath = FileSystem.Path.GetRelativePath(Path, file);
            yield return new DataRelativePath(relativePath);
        }
    }

    public IEnumerable<DataRelativePath> EnumerateDirectories(DataRelativePath path, string searchPattern = "*", bool includeSubDirectories = false) {
        var directoryPath = FileSystem.Path.GetDirectoryName(path.Path);
        if (directoryPath is null || !ArchiveReader.TryGetFolder(directoryPath, out var folder)) yield break;

        // foreach (var subFolder in folder.SubFolders) {
        //     if (!_searchFilter.Filter(subFolder.Path, searchPattern)) continue;
        //
        //     var relativePath = FileSystem.Path.GetRelativePath(Path, subFolder.Path);
        //     yield return new DataRelativePath(relativePath);
        // }
    }

    private bool Equals(ArchiveDataSource other) => DataRelativePath.PathComparer.Equals(Path, other.Path);
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is ArchiveDataSource other && Equals(other);
    public override int GetHashCode() => Path.GetHashCode();
}
