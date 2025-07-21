using System.Diagnostics;
using System.IO.Abstractions;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.DataSource;

[DebuggerDisplay("{ToString()}")]
public sealed class DataSourceFileLink(IDataSource dataSource, DataRelativePath dataRelativePath) : IDataSourceLink {
    public IDataSource DataSource { get; } = dataSource;
    public DataRelativePath DataRelativePath { get; } = dataRelativePath;

    public IFileSystem FileSystem => DataSource.FileSystem;
    public string Name => FileSystem.Path.GetFileName(DataRelativePath.Path);
    public string NameWithoutExtension => FileSystem.Path.GetFileNameWithoutExtension(DataRelativePath.Path);
    public string Extension => FileSystem.Path.GetExtension(DataRelativePath.Path);
    public string FullPath => FileSystem.Path.Combine(DataSource.Path, DataRelativePath.Path);

    public DataSourceDirectoryLink? ParentDirectory {
        get {
            var parentDirectoryPath = FileSystem.Path.GetDirectoryName(DataRelativePath.Path);
            if (parentDirectoryPath is null) return null;

            return new DataSourceDirectoryLink(DataSource, parentDirectoryPath);
        }
    }

    public bool Exists() => FileSystem.File.Exists(FullPath);

    public Stream? ReadFile() {
        if (!FileSystem.File.Exists(FullPath)) return null;

        return FileSystem.File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public override bool Equals(object? obj) {
        return obj is DataSourceFileLink other && Equals(other);
    }

    public bool Equals(IDataSourceLink? obj) {
        return obj is DataSourceFileLink other && Equals(other);
    }

    public bool Equals(DataSourceFileLink? other) {
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
        if (other is not DataSourceFileLink) return 1;

        if (DataSource.Equals(other.DataSource)) {
            return DataRelativePath.PathComparer.Compare(DataRelativePath.Path, other.DataRelativePath.Path);
        }

        return DataRelativePath.PathComparer.Compare(DataSource.Path, other.DataSource.Path);
    }

    public override string ToString() {
        return $"[{DataSource.Path}] {DataRelativePath.Path}";
    }
}
