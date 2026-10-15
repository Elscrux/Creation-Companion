using System.IO.Abstractions;
using Microsoft.Win32.SafeHandles;
namespace CreationEditor.Services.Archive.FileSystem;

/// <summary>
/// Creates <see cref="ArchiveFileInfo"/> instances for files inside an <see cref="ArchiveFileSystem"/>.
/// </summary>
public sealed class ArchiveFileInfoFactory(ArchiveFileSystem archiveFileSystem) : IFileInfoFactory {
    public IFileSystem FileSystem => archiveFileSystem;

    public IFileInfo New(string fileName) => new ArchiveFileInfo(archiveFileSystem, fileName);

    public IFileInfo? Wrap(FileInfo? fileInfo)
        => fileInfo is null ? null : new FileInfoWrapper(archiveFileSystem, fileInfo);
}

/// <summary>
/// Creates <see cref="ArchiveDirectoryInfo"/> instances for directories inside an <see cref="ArchiveFileSystem"/>.
/// </summary>
public sealed class ArchiveDirectoryInfoFactory(ArchiveFileSystem archiveFileSystem) : IDirectoryInfoFactory {
    public IFileSystem FileSystem => archiveFileSystem;

    public IDirectoryInfo New(string path) => new ArchiveDirectoryInfo(archiveFileSystem, path);

    public IDirectoryInfo? Wrap(DirectoryInfo? directoryInfo)
        => directoryInfo is null ? null : new DirectoryInfoWrapper(archiveFileSystem, directoryInfo);
}

/// <summary>
/// Creates read-only file streams for files inside an <see cref="ArchiveFileSystem"/>.
/// All write operations throw a <see cref="NotSupportedException"/>.
/// </summary>
public sealed class ArchiveFileStreamFactory(ArchiveFileSystem archiveFileSystem) : IFileStreamFactory {
    public IFileSystem FileSystem => archiveFileSystem;

    public FileSystemStream Wrap(FileStream fileStream) => new ArchiveFileStream(fileStream, fileStream.Name, false);
    public FileSystemStream New(string path, FileMode mode) => archiveFileSystem.File.Open(path, mode);
    public FileSystemStream New(string path, FileMode mode, FileAccess access) => archiveFileSystem.File.Open(path, mode, access);
    public FileSystemStream New(string path, FileMode mode, FileAccess access, FileShare share) => archiveFileSystem.File.Open(path, mode, access, share);
    public FileSystemStream New(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) =>
        archiveFileSystem.File.Open(path, mode, access, share);
    public FileSystemStream New(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) =>
        archiveFileSystem.File.Open(path, mode, access, share);
    public FileSystemStream New(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options) =>
        archiveFileSystem.File.Open(path, mode, access, share);
    public FileSystemStream New(string path, FileStreamOptions options) => archiveFileSystem.File.Open(path, options);

    public FileSystemStream New(SafeFileHandle handle, FileAccess access) => throw ArchiveFileSystem.ReadOnly();
    public FileSystemStream New(SafeFileHandle handle, FileAccess access, int bufferSize) => throw ArchiveFileSystem.ReadOnly();
    public FileSystemStream New(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) => throw ArchiveFileSystem.ReadOnly();
}
