using System.IO.Abstractions;
using CreationEditor.Services.Archive.FileSystem;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

/// <summary>
/// A data source that represents a game archive.
/// </summary>
public sealed class ArchiveDataSource : IDataSource {
    public IArchiveReader ArchiveReader { get; }
    public DataSourceFileLink ArchiveFileLink { get; }

    public string Name { get; }
    public string Path { get; }
    public bool IsReadOnly => true;
    public DataSourceDirectoryLink DeleteDirectoryLink { get; }
    public IFileSystem FileSystem { get; }

    public ArchiveDataSource(IFileSystem fileSystem, string fullPath, IArchiveReader archiveReader, DataSourceFileLink archiveFileLink) {
        ArchiveReader = archiveReader;
        ArchiveFileLink = archiveFileLink;
        FileSystem = new ArchiveFileSystem(fileSystem, archiveReader, fullPath);
        Name = fileSystem.Path.GetFileName(fullPath);
        Path = fullPath;
        DeleteDirectoryLink = new DataSourceDirectoryLink(this, FileSystemDataSource.DeleteDirectoryName);
    }

    public DataSourceMemento CreateMemento() => new(DataSourceType.Archive, Name, Path, IsReadOnly);

    public string GetFullPath(DataRelativePath path) => FileSystem.Path.Combine(Path, path.Path);

    public bool FileExists(DataRelativePath path) => FileSystem.File.Exists(GetFullPath(path));

    public bool DirectoryExists(DataRelativePath path) => FileSystem.Directory.Exists(GetFullPath(path));

    public IEnumerable<DataSourceFileLink> EnumerateFiles(DataRelativePath path, string searchPattern = "*", bool includeSubDirectories = false) {
        var fullPath = GetFullPath(path.Path);
        var files = FileSystem.Directory.EnumerateFiles(fullPath, searchPattern, includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            var relativePath = FileSystem.Path.GetRelativePath(Path, file);
            yield return new DataSourceFileLink(this, relativePath);
        }
    }

    public IEnumerable<DataSourceDirectoryLink> EnumerateDirectories(DataRelativePath path, string searchPattern = "*", bool includeSubDirectories = false) {
        var fullPath = GetFullPath(path.Path);
        var directories =
            FileSystem.Directory.EnumerateDirectories(fullPath, searchPattern, includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var directory in directories) {
            var relativePath = FileSystem.Path.GetRelativePath(Path, directory);
            yield return new DataSourceDirectoryLink(this, relativePath);
        }
    }

    private bool Equals(ArchiveDataSource other) => DataRelativePath.PathComparer.Equals(Path, other.Path);
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is ArchiveDataSource other && Equals(other));
    public override int GetHashCode() => Path.GetHashCode();
}
