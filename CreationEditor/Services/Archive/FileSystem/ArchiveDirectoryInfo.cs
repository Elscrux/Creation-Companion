using System.IO.Abstractions;
using Noggog;
namespace CreationEditor.Services.Archive.FileSystem;

/// <summary>
/// Read-only directory info for a directory inside an <see cref="ArchiveFileSystem"/>.
/// All write operations throw a <see cref="NotSupportedException"/>.
/// </summary>
public sealed class ArchiveDirectoryInfo(ArchiveFileSystem archiveFileSystem, string fullPath) : DirectoryInfoBase(archiveFileSystem) {
    public override string FullName => fullPath;
    public override string Name => archiveFileSystem.Path.GetFileName(fullPath);
    public override string Extension => archiveFileSystem.Path.GetExtension(fullPath);
    public override bool Exists => archiveFileSystem.DirectoryExists(fullPath);
    public override string? LinkTarget => null;

    public override IDirectoryInfo? Parent {
        get {
            var parentDirectory = archiveFileSystem.Path.GetDirectoryName(fullPath);
            if (parentDirectory.IsNullOrEmpty()) return null;
            if (archiveFileSystem.GetArchiveRelativePath(parentDirectory) is null) return null;

            return new ArchiveDirectoryInfo(archiveFileSystem, parentDirectory);
        }
    }

    public override IDirectoryInfo Root => new ArchiveDirectoryInfo(archiveFileSystem, archiveFileSystem.RootPath);

    public override FileAttributes Attributes {
        get => Exists ? FileAttributes.ReadOnly | FileAttributes.Directory : 0;
        set => throw ArchiveFileSystem.ReadOnly();
    }

    public override DateTime CreationTime {
        get => throw ArchiveFileSystem.NoDateTime();
        set => throw ArchiveFileSystem.ReadOnly();
    }
    public override DateTime CreationTimeUtc {
        get => throw ArchiveFileSystem.NoDateTime();
        set => throw ArchiveFileSystem.ReadOnly();
    }
    public override DateTime LastAccessTime {
        get => throw ArchiveFileSystem.NoDateTime();
        set => throw ArchiveFileSystem.ReadOnly();
    }
    public override DateTime LastAccessTimeUtc {
        get => throw ArchiveFileSystem.NoDateTime();
        set => throw ArchiveFileSystem.ReadOnly();
    }
    public override DateTime LastWriteTime {
        get => throw ArchiveFileSystem.NoDateTime();
        set => throw ArchiveFileSystem.ReadOnly();
    }
    public override DateTime LastWriteTimeUtc {
        get => throw ArchiveFileSystem.NoDateTime();
        set => throw ArchiveFileSystem.ReadOnly();
    }

    public override IEnumerable<IDirectoryInfo> EnumerateDirectories()
        => EnumerateDirectories("*", SearchOption.TopDirectoryOnly);

    public override IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern)
        => EnumerateDirectories(searchPattern, SearchOption.TopDirectoryOnly);

    public override IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
        => archiveFileSystem
            .EnumerateDirectories(fullPath, searchPattern, searchOption == SearchOption.AllDirectories)
            .Select(directory => new ArchiveDirectoryInfo(archiveFileSystem, directory));

    public override IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, EnumerationOptions enumerationOptions)
        => EnumerateDirectories(searchPattern, enumerationOptions.RecurseSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

    public override IEnumerable<IFileInfo> EnumerateFiles()
        => EnumerateFiles("*", SearchOption.TopDirectoryOnly);

    public override IEnumerable<IFileInfo> EnumerateFiles(string searchPattern)
        => EnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly);

    public override IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
        => archiveFileSystem
            .EnumerateFiles(fullPath, searchPattern, searchOption == SearchOption.AllDirectories)
            .Select(file => new ArchiveFileInfo(archiveFileSystem, file));

    public override IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, EnumerationOptions enumerationOptions)
        => EnumerateFiles(searchPattern, enumerationOptions.RecurseSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

    public override IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos()
        => EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly);

    public override IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
        => EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly);

    public override IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
        => EnumerateFiles(searchPattern, searchOption).Cast<IFileSystemInfo>()
            .Concat(EnumerateDirectories(searchPattern, searchOption));

    public override IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, EnumerationOptions enumerationOptions)
        => EnumerateFileSystemInfos(searchPattern, enumerationOptions.RecurseSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

    public override IFileInfo[] GetFiles() => EnumerateFiles().ToArray();
    public override IFileInfo[] GetFiles(string searchPattern) => EnumerateFiles(searchPattern).ToArray();
    public override IFileInfo[] GetFiles(string searchPattern, SearchOption searchOption) => EnumerateFiles(searchPattern, searchOption).ToArray();
    public override IFileInfo[] GetFiles(string searchPattern, EnumerationOptions enumerationOptions) => EnumerateFiles(searchPattern, enumerationOptions).ToArray();

    public override IDirectoryInfo[] GetDirectories() => EnumerateDirectories().ToArray();
    public override IDirectoryInfo[] GetDirectories(string searchPattern) => EnumerateDirectories(searchPattern).ToArray();
    public override IDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption) => EnumerateDirectories(searchPattern, searchOption).ToArray();
    public override IDirectoryInfo[] GetDirectories(string searchPattern, EnumerationOptions enumerationOptions) =>
        EnumerateDirectories(searchPattern, enumerationOptions).ToArray();

    public override IFileSystemInfo[] GetFileSystemInfos() => EnumerateFileSystemInfos().ToArray();
    public override IFileSystemInfo[] GetFileSystemInfos(string searchPattern) => EnumerateFileSystemInfos(searchPattern).ToArray();
    public override IFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption) =>
        EnumerateFileSystemInfos(searchPattern, searchOption).ToArray();
    public override IFileSystemInfo[] GetFileSystemInfos(string searchPattern, EnumerationOptions enumerationOptions) =>
        EnumerateFileSystemInfos(searchPattern, enumerationOptions).ToArray();

    public override void Refresh() {}

    public override void Create() => throw ArchiveFileSystem.ReadOnly();
    public override IDirectoryInfo CreateSubdirectory(string path) => throw ArchiveFileSystem.ReadOnly();
    public override void Delete() => throw ArchiveFileSystem.ReadOnly();
    public override void Delete(bool recursive) => throw ArchiveFileSystem.ReadOnly();
    public override void MoveTo(string destDirName) => throw ArchiveFileSystem.ReadOnly();
    public override void CreateAsSymbolicLink(string pathToTarget) => throw ArchiveFileSystem.ReadOnly();
    public override IFileSystemInfo ResolveLinkTarget(bool returnFinalTarget) => throw ArchiveFileSystem.ReadOnly();
}
