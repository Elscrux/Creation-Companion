using System.IO.Abstractions;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

/// <summary>
/// A data source that lies in a file system.
/// </summary>
public sealed class FileSystemDataSource : IDataSource {
    public string Name { get; }
    public string Path { get; }
    public bool IsReadOnly { get; }
    public IFileSystem FileSystem { get; }

    public FileSystemDataSource(IFileSystem fileSystem, string fullPath, bool isReadOnly = false) {
        FileSystem = fileSystem;
        Name = FileSystem.Path.GetFileName(fullPath);
        Path = fullPath;
        IsReadOnly = isReadOnly;
    }

    public DataSourceMemento CreateMemento() {
        return new DataSourceMemento(DataSourceType.FileSystem, Name, Path, IsReadOnly);
    }

    public DataSourceLink GetRootLink() {
        return new DataSourceLink(this, string.Empty);
    }

    public string GetFullPath(DataRelativePath path) {
        return FileSystem.Path.Combine(Path, path.Path);
    }

    public bool FileExists(DataRelativePath path) {
        var fullPath = GetFullPath(path.Path);
        return FileSystem.File.Exists(fullPath);
    }

    public bool DirectoryExists(DataRelativePath path) {
        var fullPath = GetFullPath(path.Path);
        return FileSystem.Directory.Exists(fullPath);
    }
    public IEnumerable<DataRelativePath> EnumerateFiles(DataRelativePath path, string searchPattern = "*", bool includeSubDirectories = false) {
        var fullPath = GetFullPath(path.Path);
        var files = FileSystem.Directory
            .EnumerateFiles(fullPath, searchPattern, includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            var relativePath = FileSystem.Path.GetRelativePath(Path, file);
            yield return new DataRelativePath(relativePath);
        }
    }
    public IEnumerable<DataRelativePath> EnumerateDirectories(DataRelativePath path, string searchPattern = "*", bool includeSubDirectories = false) {
        var fullPath = GetFullPath(path.Path);
        var directories = FileSystem.Directory
            .EnumerateDirectories(fullPath, searchPattern, includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var directory in directories) {
            var relativePath = FileSystem.Path.GetRelativePath(Path, directory);
            yield return new DataRelativePath(relativePath);
        }
    }

    public Stream Create(DataRelativePath path) {
        var fullPath = GetFullPath(path.Path);
        var directory = FileSystem.Path.GetDirectoryName(fullPath);
        if (directory is not null && !FileSystem.Directory.Exists(directory)) {
            FileSystem.Directory.CreateDirectory(directory);
        }
        return FileSystem.File.Create(fullPath);
    }

    public Stream Open(DataRelativePath path, FileMode mode, FileAccess access, FileShare share) {
        var fullPath = GetFullPath(path.Path);
        return FileSystem.File.Open(fullPath, mode, access, share);
    }

    public void Delete(DataRelativePath path) {
        var fullPath = GetFullPath(path.Path);
        if (FileSystem.File.Exists(fullPath)) {
            FileSystem.File.Delete(fullPath);
        }
    }

    public void Move(DataRelativePath path, string destination, bool overwrite = false) {
        var fullPath = GetFullPath(path.Path);
        var destinationPath = FileSystem.Path.Combine(Path, destination);
        if (FileSystem.File.Exists(fullPath)) {
            FileSystem.File.Move(fullPath, destinationPath, overwrite);
        }
    }

    public void Copy(DataRelativePath path, string destination, bool overwrite = false) {
        var fullPath = GetFullPath(path.Path);
        var destinationPath = FileSystem.Path.Combine(Path, destination);
        if (FileSystem.File.Exists(fullPath)) {
            FileSystem.File.Copy(fullPath, destinationPath, overwrite);
        }
    }

    private bool Equals(FileSystemDataSource other) => DataRelativePath.PathComparer.Equals(Path, other.Path);
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is FileSystemDataSource other && Equals(other);
    public override int GetHashCode() => Path.GetHashCode();
}
