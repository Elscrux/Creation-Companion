using System.IO.Abstractions;
using System.Text;
using Microsoft.Win32.SafeHandles;
using Mutagen.Bethesda.Archives;
namespace CreationEditor.Services.Archive.FileSystem;

/// <summary>
/// Read-only file operations on an <see cref="ArchiveFileSystem"/>.
/// All write operations throw a <see cref="NotSupportedException"/>.
/// </summary>
public sealed class ArchiveFile(ArchiveFileSystem archiveFileSystem) : FileBase(archiveFileSystem) {
    public override bool Exists(string path) => archiveFileSystem.FileExists(path);

    public override FileSystemStream Open(string path, FileMode mode, FileAccess access, FileShare share) {
        if (mode is FileMode.Create or FileMode.CreateNew or FileMode.Truncate or FileMode.Append
         || access.HasFlag(FileAccess.Write)) {
            throw ArchiveFileSystem.ReadOnly();
        }

        var archiveFile = GetArchiveFile(path);
        var stream = archiveFile.AsStream();

        // Compressed archive entries (e.g. LZ4 in BSAs) decompress into non-seekable streams.
        // Buffer them into memory so the returned stream behaves like a regular file stream.
        if (!stream.CanSeek) {
            var buffered = new MemoryStream();
            using (stream) {
                stream.CopyTo(buffered);
            }

            buffered.Position = 0;
            stream = buffered;
        }

        return new ArchiveFileStream(stream, path, false);
    }

    public override FileSystemStream Open(string path, FileMode mode)
        => Open(path, mode, mode is FileMode.Open or FileMode.OpenOrCreate ? FileAccess.Read : FileAccess.Write, FileShare.Read);

    public override FileSystemStream Open(string path, FileMode mode, FileAccess access)
        => Open(path, mode, access, FileShare.Read);

    public override FileSystemStream Open(string path, FileStreamOptions options)
        => Open(path, options.Mode, options.Access, options.Share);

    public override FileSystemStream OpenRead(string path)
        => Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

    public override StreamReader OpenText(string path) => new(OpenRead(path));

    public override byte[] ReadAllBytes(string path) => GetArchiveFile(path).GetBytes();

    public override string ReadAllText(string path) => ReadAllText(path, Encoding.UTF8);

    public override string ReadAllText(string path, Encoding encoding) {
        using var reader = new StreamReader(OpenRead(path), encoding);
        return reader.ReadToEnd();
    }

    public override string[] ReadAllLines(string path) => ReadAllLines(path, Encoding.UTF8);

    public override string[] ReadAllLines(string path, Encoding encoding) {
        using var reader = new StreamReader(OpenRead(path), encoding);
        var lines = new List<string>();
        while (reader.ReadLine() is {} line) {
            lines.Add(line);
        }
        return [.. lines];
    }

    public override IEnumerable<string> ReadLines(string path) => ReadAllLines(path);

    public override IEnumerable<string> ReadLines(string path, Encoding encoding) => ReadAllLines(path, encoding);

    public override Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
        => Task.FromResult(ReadAllText(path));

    public override Task<string> ReadAllTextAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
        => Task.FromResult(ReadAllText(path, encoding));

    public override Task<string[]> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default)
        => Task.FromResult(ReadAllLines(path));

    public override Task<string[]> ReadAllLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
        => Task.FromResult(ReadAllLines(path, encoding));

    public override Task<byte[]> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
        => Task.FromResult(ReadAllBytes(path));

    public override IAsyncEnumerable<string> ReadLinesAsync(string path, CancellationToken cancellationToken = default)
        => ReadAllLines(path).ToAsyncEnumerable();

    public override IAsyncEnumerable<string> ReadLinesAsync(string path, Encoding encoding, CancellationToken cancellationToken = default)
        => ReadAllLines(path, encoding).ToAsyncEnumerable();

    public override void Delete(string path) => throw ArchiveFileSystem.ReadOnly();
    public override void AppendAllBytes(string path, byte[] bytes) => throw ArchiveFileSystem.ReadOnly();
    public override void AppendAllBytes(string path, ReadOnlySpan<byte> bytes) => throw ArchiveFileSystem.ReadOnly();
    public override void AppendAllLines(string path, IEnumerable<string> contents) => throw ArchiveFileSystem.ReadOnly();
    public override void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding) => throw ArchiveFileSystem.ReadOnly();
    public override void AppendAllText(string path, string? contents) => throw ArchiveFileSystem.ReadOnly();
    public override void AppendAllText(string path, string? contents, Encoding encoding) => throw ArchiveFileSystem.ReadOnly();
    public override void AppendAllText(string path, ReadOnlySpan<char> contents) => throw ArchiveFileSystem.ReadOnly();
    public override void AppendAllText(string path, ReadOnlySpan<char> contents, Encoding encoding) => throw ArchiveFileSystem.ReadOnly();
    public override StreamWriter AppendText(string path) => throw ArchiveFileSystem.ReadOnly();
    public override void Copy(string sourceFileName, string destFileName) => throw ArchiveFileSystem.ReadOnly();
    public override void Copy(string sourceFileName, string destFileName, bool overwrite) => throw ArchiveFileSystem.ReadOnly();
    public override FileSystemStream Create(string path) => throw ArchiveFileSystem.ReadOnly();
    public override FileSystemStream Create(string path, int bufferSize) => throw ArchiveFileSystem.ReadOnly();
    public override FileSystemStream Create(string path, int bufferSize, FileOptions options) => throw ArchiveFileSystem.ReadOnly();
    public override IFileSystemInfo CreateSymbolicLink(string path, string pathToTarget) => throw ArchiveFileSystem.ReadOnly();
    public override StreamWriter CreateText(string path) => throw ArchiveFileSystem.ReadOnly();
    public override void Decrypt(string path) => throw ArchiveFileSystem.ReadOnly();
    public override void Encrypt(string path) => throw ArchiveFileSystem.ReadOnly();
    public override void Move(string sourceFileName, string destFileName) => throw ArchiveFileSystem.ReadOnly();
    public override void Move(string sourceFileName, string destFileName, bool overwrite) => throw ArchiveFileSystem.ReadOnly();
    public override FileSystemStream OpenWrite(string path) => throw ArchiveFileSystem.ReadOnly();
    public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName) => throw ArchiveFileSystem.ReadOnly();
    public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors) =>
        throw ArchiveFileSystem.ReadOnly();
    public override void WriteAllBytes(string path, byte[] bytes) => throw ArchiveFileSystem.ReadOnly();
    public override void WriteAllBytes(string path, ReadOnlySpan<byte> bytes) => throw ArchiveFileSystem.ReadOnly();
    public override void WriteAllLines(string path, IEnumerable<string> contents) => throw ArchiveFileSystem.ReadOnly();
    public override void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding) => throw ArchiveFileSystem.ReadOnly();
    public override void WriteAllLines(string path, string[] contents) => throw ArchiveFileSystem.ReadOnly();
    public override void WriteAllLines(string path, string[] contents, Encoding encoding) => throw ArchiveFileSystem.ReadOnly();
    public override void WriteAllText(string path, string? contents) => throw ArchiveFileSystem.ReadOnly();
    public override void WriteAllText(string path, string? contents, Encoding encoding) => throw ArchiveFileSystem.ReadOnly();
    public override void WriteAllText(string path, ReadOnlySpan<char> contents) => throw ArchiveFileSystem.ReadOnly();
    public override void WriteAllText(string path, ReadOnlySpan<char> contents, Encoding encoding) => throw ArchiveFileSystem.ReadOnly();

    public override Task WriteAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default) => throw ArchiveFileSystem.ReadOnly();
    public override Task WriteAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = default) =>
        throw ArchiveFileSystem.ReadOnly();
    public override Task WriteAllTextAsync(string path, ReadOnlyMemory<char> contents, CancellationToken cancellationToken = default) =>
        throw ArchiveFileSystem.ReadOnly();
    public override Task WriteAllTextAsync(string path, ReadOnlyMemory<char> contents, Encoding encoding, CancellationToken cancellationToken = default) =>
        throw ArchiveFileSystem.ReadOnly();
    public override Task WriteAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default) =>
        throw ArchiveFileSystem.ReadOnly();
    public override Task WriteAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default) =>
        throw ArchiveFileSystem.ReadOnly();
    public override Task WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default) => throw ArchiveFileSystem.ReadOnly();
    public override Task WriteAllBytesAsync(string path, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default) => throw ArchiveFileSystem.ReadOnly();
    public override Task AppendAllTextAsync(string path, string? contents, CancellationToken cancellationToken = default) => throw ArchiveFileSystem.ReadOnly();
    public override Task AppendAllTextAsync(string path, string? contents, Encoding encoding, CancellationToken cancellationToken = default) =>
        throw ArchiveFileSystem.ReadOnly();
    public override Task AppendAllTextAsync(string path, ReadOnlyMemory<char> contents, CancellationToken cancellationToken = default) =>
        throw ArchiveFileSystem.ReadOnly();
    public override Task AppendAllTextAsync(string path, ReadOnlyMemory<char> contents, Encoding encoding, CancellationToken cancellationToken = default) =>
        throw ArchiveFileSystem.ReadOnly();
    public override Task AppendAllLinesAsync(string path, IEnumerable<string> contents, CancellationToken cancellationToken = default) =>
        throw ArchiveFileSystem.ReadOnly();
    public override Task AppendAllLinesAsync(string path, IEnumerable<string> contents, Encoding encoding, CancellationToken cancellationToken = default) =>
        throw ArchiveFileSystem.ReadOnly();
    public override Task AppendAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default) => throw ArchiveFileSystem.ReadOnly();
    public override Task AppendAllBytesAsync(string path, ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default) =>
        throw ArchiveFileSystem.ReadOnly();

    public override FileAttributes GetAttributes(string path) => archiveFileSystem.FileExists(path) ? FileAttributes.ReadOnly : 0;
    public override FileAttributes GetAttributes(SafeFileHandle fileHandle) => throw ArchiveFileSystem.ReadOnly();

    public override DateTime GetCreationTime(string path) => throw ArchiveFileSystem.NoDateTime();
    public override DateTime GetCreationTime(SafeFileHandle fileHandle) => throw ArchiveFileSystem.ReadOnly();

    public override DateTime GetCreationTimeUtc(string path) => throw ArchiveFileSystem.NoDateTime();
    public override DateTime GetCreationTimeUtc(SafeFileHandle fileHandle) => throw ArchiveFileSystem.ReadOnly();

    public override DateTime GetLastAccessTime(string path) => throw ArchiveFileSystem.NoDateTime();
    public override DateTime GetLastAccessTime(SafeFileHandle fileHandle) => throw ArchiveFileSystem.ReadOnly();

    public override DateTime GetLastAccessTimeUtc(string path) => throw ArchiveFileSystem.NoDateTime();
    public override DateTime GetLastAccessTimeUtc(SafeFileHandle fileHandle) => throw ArchiveFileSystem.ReadOnly();

    public override DateTime GetLastWriteTime(string path) => throw ArchiveFileSystem.NoDateTime();
    public override DateTime GetLastWriteTime(SafeFileHandle fileHandle) => throw ArchiveFileSystem.ReadOnly();

    public override DateTime GetLastWriteTimeUtc(string path) => throw ArchiveFileSystem.NoDateTime();
    public override DateTime GetLastWriteTimeUtc(SafeFileHandle fileHandle) => throw ArchiveFileSystem.ReadOnly();

    public override UnixFileMode GetUnixFileMode(string path) => throw ArchiveFileSystem.ReadOnly();
    public override UnixFileMode GetUnixFileMode(SafeFileHandle fileHandle) => throw ArchiveFileSystem.ReadOnly();

    public override IFileSystemInfo ResolveLinkTarget(string linkPath, bool returnFinalTarget) => throw ArchiveFileSystem.ReadOnly();

    public override void SetAttributes(string path, FileAttributes fileAttributes) => throw ArchiveFileSystem.ReadOnly();
    public override void SetAttributes(SafeFileHandle fileHandle, FileAttributes fileAttributes) => throw ArchiveFileSystem.ReadOnly();

    public override void SetCreationTime(string path, DateTime creationTime) => throw ArchiveFileSystem.ReadOnly();
    public override void SetCreationTime(SafeFileHandle fileHandle, DateTime creationTime) => throw ArchiveFileSystem.ReadOnly();

    public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc) => throw ArchiveFileSystem.ReadOnly();
    public override void SetCreationTimeUtc(SafeFileHandle fileHandle, DateTime creationTimeUtc) => throw ArchiveFileSystem.ReadOnly();

    public override void SetLastAccessTime(string path, DateTime lastAccessTime) => throw ArchiveFileSystem.ReadOnly();
    public override void SetLastAccessTime(SafeFileHandle fileHandle, DateTime lastAccessTime) => throw ArchiveFileSystem.ReadOnly();

    public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) => throw ArchiveFileSystem.ReadOnly();
    public override void SetLastAccessTimeUtc(SafeFileHandle fileHandle, DateTime lastAccessTimeUtc) => throw ArchiveFileSystem.ReadOnly();

    public override void SetLastWriteTime(string path, DateTime lastWriteTime) => throw ArchiveFileSystem.ReadOnly();
    public override void SetLastWriteTime(SafeFileHandle fileHandle, DateTime lastWriteTime) => throw ArchiveFileSystem.ReadOnly();

    public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) => throw ArchiveFileSystem.ReadOnly();
    public override void SetLastWriteTimeUtc(SafeFileHandle fileHandle, DateTime lastWriteTimeUtc) => throw ArchiveFileSystem.ReadOnly();

    public override void SetUnixFileMode(string path, UnixFileMode mode) => throw ArchiveFileSystem.ReadOnly();
    public override void SetUnixFileMode(SafeFileHandle fileHandle, UnixFileMode mode) => throw ArchiveFileSystem.ReadOnly();

    private IArchiveFile GetArchiveFile(string path)
        => archiveFileSystem.GetArchiveFile(path) ?? throw new FileNotFoundException(null, path);
}
