using System.IO.Abstractions;
namespace CreationEditor.Services.Archive.FileSystem;

/// <summary>
/// A <see cref="FileSystemStream"/> over the contents of a file inside an archive.
/// </summary>
public sealed class ArchiveFileStream(Stream stream, string path, bool isAsync) : FileSystemStream(stream, path, isAsync);
