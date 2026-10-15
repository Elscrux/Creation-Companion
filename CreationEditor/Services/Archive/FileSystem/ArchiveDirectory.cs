using System.IO.Abstractions;
using Noggog;
namespace CreationEditor.Services.Archive.FileSystem;

/// <summary>
/// Read-only directory operations on an <see cref="ArchiveFileSystem"/>.
/// All write operations throw a <see cref="NotSupportedException"/>.
/// </summary>
public sealed class ArchiveDirectory(ArchiveFileSystem archiveFileSystem) : DirectoryBase(archiveFileSystem) {
    public override bool Exists(string path) => archiveFileSystem.DirectoryExists(path);

    public override IEnumerable<string> EnumerateFiles(string path)
        => EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        => EnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        => archiveFileSystem.EnumerateFiles(path, searchPattern, searchOption == SearchOption.AllDirectories);

    public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, EnumerationOptions enumerationOptions)
        => EnumerateFiles(path, searchPattern, enumerationOptions.RecurseSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

    public override IEnumerable<string> EnumerateDirectories(string path)
        => EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        => EnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
        => archiveFileSystem.EnumerateDirectories(path, searchPattern, searchOption == SearchOption.AllDirectories);

    public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions)
        => EnumerateDirectories(path, searchPattern, enumerationOptions.RecurseSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

    public override IEnumerable<string> EnumerateFileSystemEntries(string path)
        => EnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);

    public override IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
        => EnumerateFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);

    public override IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        => EnumerateFiles(path, searchPattern, searchOption).Concat(EnumerateDirectories(path, searchPattern, searchOption));

    public override IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, EnumerationOptions enumerationOptions)
        => EnumerateFileSystemEntries(path, searchPattern, enumerationOptions.RecurseSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

    public override string[] GetFiles(string path) => EnumerateFiles(path).ToArray();
    public override string[] GetFiles(string path, string searchPattern) => EnumerateFiles(path, searchPattern).ToArray();
    public override string[] GetFiles(string path, string searchPattern, SearchOption searchOption) => EnumerateFiles(path, searchPattern, searchOption).ToArray();
    public override string[] GetFiles(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        EnumerateFiles(path, searchPattern, enumerationOptions).ToArray();

    public override string[] GetDirectories(string path) => EnumerateDirectories(path).ToArray();
    public override string[] GetDirectories(string path, string searchPattern) => EnumerateDirectories(path, searchPattern).ToArray();
    public override string[] GetDirectories(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateDirectories(path, searchPattern, searchOption).ToArray();
    public override string[] GetDirectories(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        EnumerateDirectories(path, searchPattern, enumerationOptions).ToArray();

    public override string[] GetFileSystemEntries(string path) => EnumerateFileSystemEntries(path).ToArray();
    public override string[] GetFileSystemEntries(string path, string searchPattern) => EnumerateFileSystemEntries(path, searchPattern).ToArray();
    public override string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption) =>
        EnumerateFileSystemEntries(path, searchPattern, searchOption).ToArray();
    public override string[] GetFileSystemEntries(string path, string searchPattern, EnumerationOptions enumerationOptions) =>
        EnumerateFileSystemEntries(path, searchPattern, enumerationOptions).ToArray();

    public override IDirectoryInfo? GetParent(string path) {
        var parentDirectory = archiveFileSystem.Path.GetDirectoryName(path);
        if (parentDirectory.IsNullOrEmpty()) return null;
        if (archiveFileSystem.GetArchiveRelativePath(parentDirectory) is null) return null;

        return new ArchiveDirectoryInfo(archiveFileSystem, parentDirectory);
    }

    public override string GetCurrentDirectory() => archiveFileSystem.RootPath;

    public override string GetDirectoryRoot(string path)
        => archiveFileSystem.Path.GetPathRoot(path) ?? string.Empty;

    public override string[] GetLogicalDrives() {
        var root = archiveFileSystem.Path.GetPathRoot(archiveFileSystem.RootPath);
        return root.IsNullOrEmpty() ? [] : [root];
    }

    public override DateTime GetCreationTime(string path) => throw ArchiveFileSystem.NoDateTime();
    public override DateTime GetCreationTimeUtc(string path) => throw ArchiveFileSystem.NoDateTime();
    public override DateTime GetLastAccessTime(string path) => throw ArchiveFileSystem.NoDateTime();
    public override DateTime GetLastAccessTimeUtc(string path) => throw ArchiveFileSystem.NoDateTime();
    public override DateTime GetLastWriteTime(string path) => throw ArchiveFileSystem.NoDateTime();
    public override DateTime GetLastWriteTimeUtc(string path) => throw ArchiveFileSystem.NoDateTime();

    public override IDirectoryInfo CreateDirectory(string path) => throw ArchiveFileSystem.ReadOnly();
    public override IDirectoryInfo CreateDirectory(string path, UnixFileMode unixCreateMode) => throw ArchiveFileSystem.ReadOnly();
    public override IFileSystemInfo CreateSymbolicLink(string path, string pathToTarget) => throw ArchiveFileSystem.ReadOnly();
    public override IDirectoryInfo CreateTempSubdirectory(string? prefix = null) => throw ArchiveFileSystem.ReadOnly();
    public override void Delete(string path) => throw ArchiveFileSystem.ReadOnly();
    public override void Delete(string path, bool recursive) => throw ArchiveFileSystem.ReadOnly();
    public override void Move(string sourceDirName, string destDirName) => throw ArchiveFileSystem.ReadOnly();
    public override void SetCreationTime(string path, DateTime creationTime) => throw ArchiveFileSystem.ReadOnly();
    public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc) => throw ArchiveFileSystem.ReadOnly();
    public override void SetCurrentDirectory(string path) => throw ArchiveFileSystem.ReadOnly();
    public override void SetLastAccessTime(string path, DateTime lastAccessTime) => throw ArchiveFileSystem.ReadOnly();
    public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) => throw ArchiveFileSystem.ReadOnly();
    public override void SetLastWriteTime(string path, DateTime lastWriteTime) => throw ArchiveFileSystem.ReadOnly();
    public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) => throw ArchiveFileSystem.ReadOnly();
    public override IFileSystemInfo ResolveLinkTarget(string linkPath, bool returnFinalTarget) => throw ArchiveFileSystem.ReadOnly();
}
