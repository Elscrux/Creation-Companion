using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using CreationEditor.Core;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

// TODO: Use data source for anything that needs to be accessed by a path
// TODO: Replace instances of DataDirectoryProvider with data sources
// TODO: Also replace IDataDirectoryService dataDirectoryService
// Add vanilla data folder by default
// Use sources as a way to access any data
// Replace asset controller methods to have inputs that are a "data source" + a "data relative path"
// TODO replace instances where GameEnvironment.DataFolderPath is used with a data source
public record FileSystemLink(IDataSource DataSource, DataRelativePath DataRelativePath) : IComparable<FileSystemLink> {
    public string FullPath => FileSystem.Path.Combine(DataSource.Path, DataRelativePath.Path);
    public IFileSystem FileSystem => DataSource.FileSystem;

    public bool IsFile => FileSystem.Path.HasExtension(DataRelativePath.Path);
    public bool IsDirectory => !IsFile;

    /// <summary>
    /// Gets the name of the file or directory.
    /// </summary>
    public string Name => FileSystem.Path.GetFileName(DataRelativePath.Path);

    public string NameWithoutExtension => FileSystem.Path.GetFileNameWithoutExtension(DataRelativePath.Path);
    public string Extension => FileSystem.Path.GetExtension(DataRelativePath.Path);

    public FileSystemLink? ParentDirectory {
        get {
            var parentDirectoryPath = FileSystem.Path.GetDirectoryName(DataRelativePath.Path);
            if (parentDirectoryPath is null) return null;

            return this with { DataRelativePath = parentDirectoryPath };
        }
    }

    public bool Exists() => FileSystem.File.Exists(FullPath);

    public bool Contains(FileSystemLink fileSystemLink) {
        if (!fileSystemLink.DataSource.Equals(DataSource)) return false;

        return fileSystemLink.DataRelativePath.Path.StartsWith(DataRelativePath.Path, DataRelativePath.PathComparison);
    }

    public IEnumerable<FileSystemLink> EnumerateFileLinks(bool includeSubDirectories) {
        return EnumerateFileLinks("*", includeSubDirectories);
    }

    public IEnumerable<FileSystemLink> EnumerateFileLinks(string searchPattern, bool includeSubDirectories) {
        if (!FileSystem.Directory.Exists(FullPath)) yield break;

        var files = FileSystem.Directory.EnumerateFiles(
            FullPath,
            searchPattern,
            includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            var dataRelativePath = new DataRelativePath(file);
            yield return this with { DataRelativePath = dataRelativePath };
        }
    }

    public IEnumerable<FileSystemLink> EnumerateDirectoryLinks(bool includeSubDirectories) {
        if (!FileSystem.Directory.Exists(FullPath)) yield break;

        var files = FileSystem.Directory.EnumerateDirectories(
            FullPath,
            "*",
            includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            var dataRelativePath = new DataRelativePath(file);
            yield return this with { DataRelativePath = dataRelativePath };
        }
    }

    public IEnumerable<FileSystemLink> EnumerateAllLinks(bool includeSubDirectories) {
        return EnumerateFileLinks(includeSubDirectories)
            .Concat(EnumerateDirectoryLinks(includeSubDirectories));
    }

    public Stream? ReadAsFile() {
        if (IsDirectory) throw new InvalidOperationException($"Trying to read a directory {FullPath} as a file");

        if (!FileSystem.File.Exists(FullPath)) return null;

        return FileSystem.File.OpenRead(FullPath);
    }

    public int CompareTo(FileSystemLink? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;

        if (DataSource.Equals(other.DataSource)) {
            return DataRelativePath.PathComparer.Compare(DataRelativePath.Path, other.DataRelativePath.Path);
        }

        return DataRelativePath.PathComparer.Compare(DataSource.Path, other.DataSource.Path);
    }

    public int CompareToDirectoriesFirst(FileSystemLink? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;

        if (IsDirectory && other.IsFile) return -1;
        if (IsFile && other.IsDirectory) return 1;

        if (DataSource.Equals(other.DataSource)) {
            return DataRelativePath.PathComparer.Compare(DataRelativePath.Path, other.DataRelativePath.Path);
        }

        return DataRelativePath.PathComparer.Compare(DataSource.Path, other.DataSource.Path);
    }
}

public static class DataSourceExtensions {
    public static bool TryGetFileSystemLink(this IDataSource dataSource, string fullPath, [MaybeNullWhen(false)] out FileSystemLink link) {
        if (!fullPath.StartsWith(dataSource.Path, DataRelativePath.PathComparison)) {
            link = null;
            return false;
        }

        var relativePath = dataSource.FileSystem.Path.GetRelativePath(dataSource.Path, fullPath);
        link = new FileSystemLink(dataSource, relativePath);
        return true;
    }
}

// TODO analyze asset browser startup and see what makes it so slow
// TODO cut any unneeded initialization that is not necessary - try lazy or so otherwise

/// <summary>
/// Represents a data source that acts as a data folder for the game.
/// </summary>
public interface IDataSource : IMementoProvider<DataSourceMemento> {
    string Name { get; }
    string Path { get; }
    bool IsReadOnly { get; }
    // todo replace all use of file system anywhere with this
    IFileSystem FileSystem { get; }

    string GetFullPath(DataRelativePath path);
    bool FileExists(DataRelativePath path);
}

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

    public string GetFullPath(DataRelativePath path) {
        return FileSystem.Path.Combine(Path, path.Path);
    }

    public bool FileExists(DataRelativePath path) {
        var fullPath = GetFullPath(path.Path);
        return FileSystem.File.Exists(fullPath);
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
