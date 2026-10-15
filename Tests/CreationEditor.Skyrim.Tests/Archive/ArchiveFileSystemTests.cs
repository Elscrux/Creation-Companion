using System.IO.Abstractions;
using CreationEditor.Services.Archive.FileSystem;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Archives;
namespace CreationEditor.Skyrim.Tests.Archive;

public sealed class ArchiveFileSystemTests {
    private const string ArchivePath = @"C:\GameDirectory\Data\TestArchive.bsa";

    private static ArchiveFileSystem CreateFileSystem(IFileSystem fileSystem, params IArchiveFile[] files) => new(fileSystem, new FakeArchiveReader(files), ArchivePath);

    [Theory] [CreationEditorAutoData]
    public void FileExists_FindsFilesInArchive(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(fileSystem, new FakeArchiveFile(@"meshes\test.nif"));

        Assert.True(archiveFileSystem.File.Exists(Path.Combine(ArchivePath, "meshes", "test.nif")));
        Assert.False(archiveFileSystem.File.Exists(Path.Combine(ArchivePath, "meshes", "missing.nif")));
        Assert.False(archiveFileSystem.File.Exists(@"C:\Somewhere\Else\test.nif"));
    }

    [Theory] [CreationEditorAutoData]
    public void FileExists_IsCaseInsensitive(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(fileSystem, new FakeArchiveFile(@"meshes\test.nif"));

        Assert.True(archiveFileSystem.File.Exists(Path.Combine(ArchivePath, "MESHES", "TEST.NIF")));
    }

    [Theory] [CreationEditorAutoData]
    public void FileOpen_ReturnsArchiveContent(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(fileSystem, new FakeArchiveFile(@"meshes\test.nif", 1, 2, 3));

        using var stream = archiveFileSystem.File.Open(Path.Combine(ArchivePath, "meshes", "test.nif"), FileMode.Open, FileAccess.Read, FileShare.Read);
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);

        Assert.Equal([1, 2, 3], memoryStream.ToArray());
    }

    [Theory] [CreationEditorAutoData]
    public void FileOpen_MissingFile_ThrowsFileNotFound(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(fileSystem);

        Assert.Throws<FileNotFoundException>(() =>
            archiveFileSystem.File.Open(Path.Combine(ArchivePath, "meshes", "missing.nif"), FileMode.Open, FileAccess.Read, FileShare.Read));
    }

    [Theory] [CreationEditorAutoData]
    public void FileOpen_NonSeekableStream_ReturnsSeekableStream(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(fileSystem, new FakeCompressedArchiveFile(@"meshes\test.nif", 1, 2, 3));
        var path = Path.Combine(ArchivePath, "meshes", "test.nif");

        using var stream = archiveFileSystem.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        Assert.True(stream.CanSeek);
        stream.Position = 2;
        Assert.Equal(2, stream.Position);
        Assert.Equal(3, stream.ReadByte());

        stream.Position = 0;
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        Assert.Equal([1, 2, 3], memoryStream.ToArray());
    }

    [Theory] [CreationEditorAutoData]
    public void FileOpen_WriteAccess_ThrowsNotSupported(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(fileSystem, new FakeArchiveFile(@"meshes\test.nif"));

        Assert.Throws<NotSupportedException>(() =>
            archiveFileSystem.File.Open(Path.Combine(ArchivePath, "meshes", "test.nif"), FileMode.Create, FileAccess.Write, FileShare.None));
    }

    [Theory] [CreationEditorAutoData]
    public void ReadAllBytes_ReturnsArchiveContent(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(fileSystem, new FakeArchiveFile(@"meshes\test.nif", 1, 2, 3));

        Assert.Equal([1, 2, 3], archiveFileSystem.File.ReadAllBytes(Path.Combine(ArchivePath, "meshes", "test.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public void DirectoryExists_FindsDirectoriesInArchive(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(
            fileSystem,
            new FakeArchiveFile(@"meshes\test.nif", 1),
            new FakeArchiveFile(@"meshes\clutter\deep\test.nif", 2));

        Assert.True(archiveFileSystem.Directory.Exists(ArchivePath));
        Assert.True(archiveFileSystem.Directory.Exists(Path.Combine(ArchivePath, "meshes")));
        Assert.True(archiveFileSystem.Directory.Exists(Path.Combine(ArchivePath, "meshes", "clutter")));
        Assert.True(archiveFileSystem.Directory.Exists(Path.Combine(ArchivePath, "meshes", "clutter", "deep")));
        Assert.False(archiveFileSystem.Directory.Exists(Path.Combine(ArchivePath, "textures")));
    }

    [Theory] [CreationEditorAutoData]
    public void EnumerateFiles_TopDirectoryOnly_ReturnsDirectFiles(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(
            fileSystem,
            new FakeArchiveFile(@"meshes\test.nif", 1),
            new FakeArchiveFile(@"meshes\clutter\deep.nif", 2));

        var files = archiveFileSystem.Directory
            .EnumerateFiles(Path.Combine(ArchivePath, "meshes"), "*", SearchOption.TopDirectoryOnly)
            .ToList();

        var file = Assert.Single(files);
        Assert.Equal(Path.Combine(ArchivePath, "meshes", "test.nif"), file);
    }

    [Theory] [CreationEditorAutoData]
    public void EnumerateFiles_AllDirectories_ReturnsNestedFiles(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(
            fileSystem,
            new FakeArchiveFile(@"meshes\test.nif", 1),
            new FakeArchiveFile(@"meshes\clutter\deep.nif", 2),
            new FakeArchiveFile(@"textures\test.dds", 3));

        var files = archiveFileSystem.Directory
            .EnumerateFiles(ArchivePath, "*", SearchOption.AllDirectories)
            .ToList();

        Assert.Equal(3, files.Count);
        Assert.Contains(Path.Combine(ArchivePath, "meshes", "test.nif"), files);
        Assert.Contains(Path.Combine(ArchivePath, "meshes", "clutter", "deep.nif"), files);
        Assert.Contains(Path.Combine(ArchivePath, "textures", "test.dds"), files);
    }

    [Theory] [CreationEditorAutoData]
    public void EnumerateFiles_WithSearchPattern_FiltersFiles(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(
            fileSystem,
            new FakeArchiveFile(@"meshes\test.nif", 1),
            new FakeArchiveFile(@"meshes\test.dds", 2));

        var files = archiveFileSystem.Directory
            .EnumerateFiles(Path.Combine(ArchivePath, "meshes"), "*.nif", SearchOption.TopDirectoryOnly)
            .ToList();

        var file = Assert.Single(files);
        Assert.Equal(Path.Combine(ArchivePath, "meshes", "test.nif"), file);
    }

    [Theory] [CreationEditorAutoData]
    public void EnumerateDirectories_ReturnsSubDirectories(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(
            fileSystem,
            new FakeArchiveFile(@"meshes\test.nif", 1),
            new FakeArchiveFile(@"meshes\clutter\deep.nif", 2),
            new FakeArchiveFile(@"textures\test.dds", 3));

        var directories = archiveFileSystem.Directory
            .EnumerateDirectories(ArchivePath, "*", SearchOption.TopDirectoryOnly)
            .ToList();

        Assert.Equal(2, directories.Count);
        Assert.Contains(Path.Combine(ArchivePath, "meshes"), directories);
        Assert.Contains(Path.Combine(ArchivePath, "textures"), directories);
    }

    [Theory] [CreationEditorAutoData]
    public void EnumerateDirectories_AllDirectories_ReturnsNestedDirectories(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(
            fileSystem,
            new FakeArchiveFile(@"meshes\clutter\deep\test.nif"));

        var directories = archiveFileSystem.Directory
            .EnumerateDirectories(ArchivePath, "*", SearchOption.AllDirectories)
            .ToList();

        Assert.Equal(3, directories.Count);
        Assert.Contains(Path.Combine(ArchivePath, "meshes"), directories);
        Assert.Contains(Path.Combine(ArchivePath, "meshes", "clutter"), directories);
        Assert.Contains(Path.Combine(ArchivePath, "meshes", "clutter", "deep"), directories);
    }

    [Theory] [CreationEditorAutoData]
    public void DirectoryInfo_GetFiles_ReturnsFileInfosWithLength(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(fileSystem, new FakeArchiveFile(@"meshes\test.nif"));

        var directoryInfo = archiveFileSystem.DirectoryInfo.New(Path.Combine(ArchivePath, "meshes"));
        var fileInfos = directoryInfo.GetFiles();

        var fileInfo = Assert.Single(fileInfos);
        Assert.Equal(0, fileInfo.Length);
        Assert.Equal("test.nif", fileInfo.Name);
        Assert.True(fileInfo.Exists);
    }

    [Theory] [CreationEditorAutoData]
    public void WriteOperations_ThrowNotSupportedException(IFileSystem fileSystem) {
        var archiveFileSystem = CreateFileSystem(fileSystem, new FakeArchiveFile(@"meshes\test.nif"));
        var filePath = Path.Combine(ArchivePath, "meshes", "test.nif");

        Assert.Throws<NotSupportedException>(() => archiveFileSystem.File.Delete(filePath));
        Assert.Throws<NotSupportedException>(() => archiveFileSystem.File.WriteAllText(filePath, "content"));
        Assert.Throws<NotSupportedException>(() => archiveFileSystem.Directory.Delete(Path.Combine(ArchivePath, "meshes")));
        Assert.Throws<NotSupportedException>(() => archiveFileSystem.Directory.CreateDirectory(Path.Combine(ArchivePath, "new")));
    }
}
