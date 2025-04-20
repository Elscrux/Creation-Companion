using System.IO.Abstractions;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

/// <summary>
/// A data source that represents a game archive.
/// </summary>
public sealed class ArchiveDataSource : IDataSource {
    public IArchiveReader ArchiveReader { get; }
    public FileSystemLink ArchiveLink { get; }

    public string Name { get; }
    public string Path { get; }
    public bool IsReadOnly => true;
    public IFileSystem FileSystem { get; }

    public ArchiveDataSource(IFileSystem fileSystem, string fullPath, IArchiveReader archiveReader, FileSystemLink archiveLink) {
        ArchiveReader = archiveReader;
        ArchiveLink = archiveLink;
        FileSystem = fileSystem;
        Name = FileSystem.Path.GetFileName(fullPath);
        Path = fullPath;
    }

    public DataSourceMemento CreateMemento() {
        return new DataSourceMemento(DataSourceType.Archive, Name, Path, IsReadOnly);
    }

    public string GetFullPath(DataRelativePath path) {
        return FileSystem.Path.Combine(Path, path.Path);
    }

    public bool FileExists(DataRelativePath path) {
        var fullPath = GetFullPath(path.Path);
        return FileSystem.File.Exists(fullPath);
    }

    private bool Equals(FileSystemDataSource other) => DataRelativePath.PathComparer.Equals(Path, other.Path);
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is FileSystemDataSource other && Equals(other);
    public override int GetHashCode() => Path.GetHashCode();
}
