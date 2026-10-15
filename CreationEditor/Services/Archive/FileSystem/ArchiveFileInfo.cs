using System.IO.Abstractions;
using Mutagen.Bethesda.Archives;
using Noggog;
namespace CreationEditor.Services.Archive.FileSystem;

/// <summary>
/// Read-only file info for a file inside an <see cref="ArchiveFileSystem"/>.
/// All write operations throw a <see cref="NotSupportedException"/>.
/// </summary>
public sealed class ArchiveFileInfo(ArchiveFileSystem archiveFileSystem, string fullPath) : FileInfoBase(archiveFileSystem) {
    public override string FullName => fullPath;
    public override string Name => archiveFileSystem.Path.GetFileName(fullPath);
    public override string Extension => archiveFileSystem.Path.GetExtension(fullPath);
    public override bool Exists => archiveFileSystem.FileExists(fullPath);
    public override long Length => GetArchiveFile().Size;

    public override string? DirectoryName {
        get {
            var directory = archiveFileSystem.Path.GetDirectoryName(fullPath);
            return directory.IsNullOrEmpty() ? null : directory;
        }
    }

    public override IDirectoryInfo? Directory {
        get {
            var directoryName = DirectoryName;
            return directoryName.IsNullOrEmpty() ? null : new ArchiveDirectoryInfo(archiveFileSystem, directoryName);
        }
    }

    public override bool IsReadOnly {
        get => true;
        set => throw ArchiveFileSystem.ReadOnly();
    }

    public override FileAttributes Attributes {
        get => Exists ? FileAttributes.ReadOnly : 0;
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

    public override string? LinkTarget => null;

    public override FileSystemStream Open(FileMode mode, FileAccess access, FileShare share)
        => archiveFileSystem.File.Open(fullPath, mode, access, share);

    public override FileSystemStream Open(FileMode mode)
        => archiveFileSystem.File.Open(fullPath, mode);

    public override FileSystemStream Open(FileMode mode, FileAccess access)
        => archiveFileSystem.File.Open(fullPath, mode, access);

    public override FileSystemStream Open(FileStreamOptions options)
        => archiveFileSystem.File.Open(fullPath, options);

    public override FileSystemStream OpenRead() => archiveFileSystem.File.OpenRead(fullPath);

    public override StreamReader OpenText() => archiveFileSystem.File.OpenText(fullPath);

    public override void Refresh() {}

    public override void Delete() => throw ArchiveFileSystem.ReadOnly();
    public override StreamWriter AppendText() => throw ArchiveFileSystem.ReadOnly();
    public override IFileInfo CopyTo(string destFileName) => throw ArchiveFileSystem.ReadOnly();
    public override IFileInfo CopyTo(string destFileName, bool overwrite) => throw ArchiveFileSystem.ReadOnly();
    public override FileSystemStream Create() => throw ArchiveFileSystem.ReadOnly();
    public override StreamWriter CreateText() => throw ArchiveFileSystem.ReadOnly();
    public override void Decrypt() => throw ArchiveFileSystem.ReadOnly();
    public override void Encrypt() => throw ArchiveFileSystem.ReadOnly();
    public override void MoveTo(string destFileName) => throw ArchiveFileSystem.ReadOnly();
    public override void MoveTo(string destFileName, bool overwrite) => throw ArchiveFileSystem.ReadOnly();
    public override FileSystemStream OpenWrite() => throw ArchiveFileSystem.ReadOnly();
    public override IFileInfo Replace(string destinationFileName, string destinationBackupFileName) => throw ArchiveFileSystem.ReadOnly();
    public override IFileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors) => throw ArchiveFileSystem.ReadOnly();
    public override void CreateAsSymbolicLink(string pathToTarget) => throw ArchiveFileSystem.ReadOnly();
    public override IFileSystemInfo ResolveLinkTarget(bool returnFinalTarget) => throw ArchiveFileSystem.ReadOnly();

    private IArchiveFile GetArchiveFile()
        => archiveFileSystem.GetArchiveFile(fullPath) ?? throw new FileNotFoundException(null, fullPath);
}
