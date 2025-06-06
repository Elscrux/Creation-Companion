﻿using System.Diagnostics;
using System.IO.Abstractions;
using Mutagen.Bethesda.Assets;
using Noggog;
namespace CreationEditor.Services.DataSource;

[DebuggerDisplay("{ToString()}")]
public sealed class FileSystemLink(IDataSource dataSource, DataRelativePath dataRelativePath)
    : IComparable<FileSystemLink>, IEquatable<FileSystemLink> {
    public string FullPath => FileSystem.Path.Combine(DataSource.Path, DataRelativePath.Path);
    public IFileSystem FileSystem => DataSource.FileSystem;

    private bool? _isFile;
    public bool IsFile => _isFile ??= FileSystem.File.Exists(FullPath);
    public bool IsDirectory => _isFile.HasValue ? !IsFile : FileSystem.Directory.Exists(FullPath);

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

            return new FileSystemLink(DataSource, parentDirectoryPath);
        }
    }

    public IDataSource DataSource { get; } = dataSource;
    public DataRelativePath DataRelativePath { get; init; } = dataRelativePath;

    public bool Exists() => IsFile
        ? FileSystem.File.Exists(FullPath)
        : FileSystem.Directory.Exists(FullPath);

    public bool Contains(FileSystemLink fileSystemLink) {
        if (!fileSystemLink.DataSource.Equals(DataSource)) return false;

        return IsFile
            ? fileSystemLink.DataRelativePath.Path.StartsWith(DataRelativePath.Path, DataRelativePath.PathComparison)
            : DataRelativePath.Path.IsNullOrEmpty()
         || fileSystemLink.DataRelativePath.Path.StartsWith(DataRelativePath.Path + FileSystem.Path.DirectorySeparatorChar,
                DataRelativePath.PathComparison);
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
            yield return new FileSystemLink(DataSource, new DataRelativePath(relativePath));
        }
    }

    public IEnumerable<FileSystemLink> EnumerateDirectoryLinks(bool includeSubDirectories) {
        return EnumerateDirectoryLinks("*", includeSubDirectories);
    }

    public IEnumerable<FileSystemLink> EnumerateDirectoryLinks(string searchPattern, bool includeSubDirectories) {
        if (!FileSystem.Directory.Exists(FullPath)) yield break;

        var files = FileSystem.Directory.EnumerateDirectories(
            FullPath,
            "*",
            includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

        foreach (var file in files) {
            var relativePath = FileSystem.Path.GetRelativePath(DataSource.Path, file);
            yield return new FileSystemLink(DataSource, new DataRelativePath(relativePath));
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

    public override bool Equals(object? obj) {
        return Equals(obj as FileSystemLink);
    }

    public bool Equals(FileSystemLink? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return DataSource.Equals(other.DataSource) && DataRelativePath.Equals(other.DataRelativePath);
    }

    public override int GetHashCode() => HashCode.Combine(DataSource, DataRelativePath);

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

    public void Deconstruct(out IDataSource dataSource, out DataRelativePath dataRelativePath) {
        dataSource = DataSource;
        dataRelativePath = DataRelativePath;
    }

    public override string ToString() {
        return $"[{DataSource.Path}] {DataRelativePath.Path}";
    }
}
