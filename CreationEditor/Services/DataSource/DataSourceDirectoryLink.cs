using System.IO.Abstractions;
using Mutagen.Bethesda.Assets;
using Noggog;
namespace CreationEditor.Services.DataSource;

public sealed class DataSourceDirectoryLink(IDataSource dataSource, DataRelativePath dataRelativePath) : IDataSourceLink {
    public IDataSource DataSource { get; } = dataSource;
    public DataRelativePath DataRelativePath { get; } = dataRelativePath;

    public IFileSystem FileSystem => DataSource.FileSystem;
    public string Name => FileSystem.Path.GetFileName(DataRelativePath.Path);
    public string NameWithoutExtension => FileSystem.Path.GetFileNameWithoutExtension(DataRelativePath.Path);
    public string FullPath => FileSystem.Path.Combine(DataSource.Path, DataRelativePath.Path);

    public DataSourceDirectoryLink? ParentDirectory {
        get {
            var parentDirectoryPath = FileSystem.Path.GetDirectoryName(DataRelativePath.Path);
            if (parentDirectoryPath is null) return null;

            return new DataSourceDirectoryLink(DataSource, parentDirectoryPath);
        }
    }

    public bool Exists() => FileSystem.Directory.Exists(FullPath);

    public bool Contains(IDataSourceLink link) {
        if (!link.DataSource.Equals(DataSource)) return false;

        return DataRelativePath.Path.IsNullOrEmpty()
         || link.DataRelativePath.Path.StartsWith(DataRelativePath.Path + FileSystem.Path.DirectorySeparatorChar,
                DataRelativePath.PathComparison);
    }

    public IEnumerable<DataSourceFileLink> EnumerateFileLinks(bool includeSubDirectories) {
        return EnumerateFileLinks("*", includeSubDirectories);
    }

    public IEnumerable<DataSourceFileLink> EnumerateFileLinks(string searchPattern, bool includeSubDirectories) {
        if (!FileSystem.Directory.Exists(FullPath)) yield break;

        var files = FileSystem.Directory.EnumerateFiles(
            FullPath,
            searchPattern,
            includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            var relativePath = FileSystem.Path.GetRelativePath(DataSource.Path, file);
            yield return new DataSourceFileLink(DataSource, new DataRelativePath(relativePath));
        }
    }

    public IEnumerable<DataSourceDirectoryLink> EnumerateDirectoryLinks(bool includeSubDirectories) {
        return EnumerateDirectoryLinks("*", includeSubDirectories);
    }

    public IEnumerable<DataSourceDirectoryLink> EnumerateDirectoryLinks(string searchPattern, bool includeSubDirectories) {
        if (!FileSystem.Directory.Exists(FullPath)) yield break;

        var files = FileSystem.Directory.EnumerateDirectories(
            FullPath,
            searchPattern,
            includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            var relativePath = FileSystem.Path.GetRelativePath(DataSource.Path, file);
            yield return new DataSourceDirectoryLink(DataSource, new DataRelativePath(relativePath));
        }
    }

    public IEnumerable<IDataSourceLink> EnumerateAllLinks(bool includeSubDirectories) {
        return EnumerateFileLinks(includeSubDirectories)
            .Concat<IDataSourceLink>(EnumerateDirectoryLinks(includeSubDirectories));
    }

    public override bool Equals(object? obj) {
        return obj is DataSourceDirectoryLink other && Equals(other);
    }

    public bool Equals(IDataSourceLink? obj) {
        return obj is DataSourceDirectoryLink other && Equals(other);
    }

    public bool Equals(DataSourceDirectoryLink? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return DataSource.Equals(other.DataSource) && DataRelativePath.Equals(other.DataRelativePath);
    }

    public override int GetHashCode() => HashCode.Combine(DataSource, DataRelativePath);

    public int CompareTo(IDataSourceLink? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;

        if (DataSource.Equals(other.DataSource)) {
            return DataRelativePath.PathComparer.Compare(DataRelativePath.Path, other.DataRelativePath.Path);
        }

        return DataRelativePath.PathComparer.Compare(DataSource.Path, other.DataSource.Path);
    }

    public int CompareToDirectoriesFirst(IDataSourceLink? other) {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;

        if (other is DataSourceFileLink) return -1;

        if (DataSource.Equals(other.DataSource)) {
            return DataRelativePath.PathComparer.Compare(DataRelativePath.Path, other.DataRelativePath.Path);
        }

        return DataRelativePath.PathComparer.Compare(DataSource.Path, other.DataSource.Path);
    }

    public override string ToString() {
        return $"[{DataSource.Path}] {DataRelativePath.Path}";
    }
}
