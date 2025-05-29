using System.IO.Abstractions;
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
            var relativePath = FileSystem.Path.GetRelativePath(DataSource.Path, file);
            var dataRelativePath = new DataRelativePath(relativePath);
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
            var relativePath = FileSystem.Path.GetRelativePath(DataSource.Path, file);
            var dataRelativePath = new DataRelativePath(relativePath);
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
