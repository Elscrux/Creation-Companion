using Mutagen.Bethesda.Archives;
using Noggog;
namespace CreationEditor.Skyrim.Tests.Archive;

internal sealed class FakeArchiveFile(string path, params byte[] bytes) : IArchiveFile {
    public string Path => path;
    public uint Size => (uint) bytes.Length;

    public byte[] GetBytes() => bytes;

    public ReadOnlySpan<byte> GetSpan() => bytes;

    public ReadOnlyMemorySlice<byte> GetMemorySlice() => [with(bytes)];

    public Stream AsStream() => new MemoryStream(bytes);
}

/// <summary>
/// Simulates a compressed archive entry stream (e.g. LZ4 in BSAs): readable, but not seekable.
/// </summary>
internal sealed class FakeNonSeekableStream(Stream stream) : Stream {
    public override bool CanRead => stream.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => stream.CanWrite;
    public override long Length => stream.Length;
    public override long Position {
        get => stream.Position;
        set => throw new InvalidOperationException("Operation Seek is not allowed");
    }

    public override void Flush() => stream.Flush();
    public override int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count);
    public override long Seek(long offset, SeekOrigin origin) => throw new InvalidOperationException("Operation Seek is not allowed");
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count);
}

internal sealed class FakeCompressedArchiveFile(string path, params byte[] bytes) : IArchiveFile {
    public string Path => path;
    public uint Size => (uint) bytes.Length;

    public byte[] GetBytes() => bytes;

    public ReadOnlySpan<byte> GetSpan() => bytes;

    public ReadOnlyMemorySlice<byte> GetMemorySlice() => [with(bytes)];

    public Stream AsStream() => new FakeNonSeekableStream(new MemoryStream(bytes));
}

internal sealed class FakeArchiveFolder(string? path, params IReadOnlyCollection<IArchiveFile> files) : IArchiveFolder {
    public string? Path => path;
    public IReadOnlyCollection<IArchiveFile> Files => files;
}

internal sealed class FakeArchiveReader(params IArchiveFile[] files) : IArchiveReader {
    private readonly Dictionary<string, IArchiveFolder> _folders = files
        .GroupBy(file => Path.GetDirectoryName(file.Path) ?? string.Empty, StringComparer.OrdinalIgnoreCase)
        .ToDictionary(
            group => group.Key,
            IArchiveFolder (group) => new FakeArchiveFolder(group.Key, [.. group]));

    public IEnumerable<IArchiveFile> Files => files;

    public bool TryGetFolder(string path, out IArchiveFolder folder) => _folders.TryGetValue(path, out folder!);
}
