using System.IO.Abstractions;
using CreationEditor.Services.DataSource;
using CreationEditor.Skyrim.Tests.Archive;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Skyrim.Tests.DataSource;

public sealed class ArchiveDataSourceTests {
    private const string ArchivePath = @"C:\GameDirectory\Data\TestArchive.bsa";

    private static ArchiveDataSource CreateDataSource(IFileSystem fileSystem, params IArchiveFile[] files) {
        var parentDataSource = new FileSystemDataSource(fileSystem, @"C:\GameDirectory\Data");
        var archiveLink = new DataSourceFileLink(parentDataSource, new DataRelativePath("TestArchive.bsa"));
        return new ArchiveDataSource(fileSystem, ArchivePath, new FakeArchiveReader(files), archiveLink);
    }

    [Theory] [CreationEditorAutoData]
    public void FileExists_And_DirectoryExists_CheckArchiveContents(IFileSystem fileSystem) {
        var dataSource = CreateDataSource(
            fileSystem,
            new FakeArchiveFile(@"meshes\test.nif", 1),
            new FakeArchiveFile(@"meshes\clutter\deep.nif", 2));

        Assert.True(dataSource.FileExists(new DataRelativePath(@"meshes\test.nif")));
        Assert.False(dataSource.FileExists(new DataRelativePath(@"meshes\missing.nif")));
        Assert.True(dataSource.DirectoryExists(new DataRelativePath(@"meshes\clutter")));
        Assert.False(dataSource.DirectoryExists(new DataRelativePath("textures")));
    }

    [Theory] [CreationEditorAutoData]
    public void EnumerateFiles_ReturnsRelativePaths(IFileSystem fileSystem) {
        var dataSource = CreateDataSource(
            fileSystem,
            new FakeArchiveFile(@"meshes\test.nif", 1),
            new FakeArchiveFile(@"meshes\clutter\deep.nif", 2));

        var files = dataSource.EnumerateFiles(new DataRelativePath("meshes")).ToList();

        var file = Assert.Single(files);
        Assert.Equal(@"meshes\test.nif", file.DataRelativePath.Path);
        Assert.Equal(dataSource, file.DataSource);
    }

    [Theory] [CreationEditorAutoData]
    public void EnumerateFiles_Recursive_ReturnsAllFiles(IFileSystem fileSystem) {
        var dataSource = CreateDataSource(
            fileSystem,
            new FakeArchiveFile(@"meshes\test.nif", 1),
            new FakeArchiveFile(@"meshes\clutter\deep.nif", 2),
            new FakeArchiveFile(@"textures\test.dds", 3));

        var files = dataSource.EnumerateFiles(new DataRelativePath(""), includeSubDirectories: true).ToList();

        Assert.Equal(3, files.Count);
        Assert.Contains(files, x => x.DataRelativePath.Path == @"meshes\test.nif");
        Assert.Contains(files, x => x.DataRelativePath.Path == @"meshes\clutter\deep.nif");
        Assert.Contains(files, x => x.DataRelativePath.Path == @"textures\test.dds");
    }

    [Theory] [CreationEditorAutoData]
    public void EnumerateDirectories_ReturnsRelativePaths(IFileSystem fileSystem) {
        var dataSource = CreateDataSource(
            fileSystem,
            new FakeArchiveFile(@"meshes\test.nif", 1),
            new FakeArchiveFile(@"meshes\clutter\deep.nif", 2),
            new FakeArchiveFile(@"textures\test.dds", 3));

        var directories = dataSource.EnumerateDirectories(new DataRelativePath("")).ToList();

        Assert.Equal(2, directories.Count);
        Assert.Contains(directories, x => x.DataRelativePath.Path == "meshes");
        Assert.Contains(directories, x => x.DataRelativePath.Path == "textures");
    }

    [Theory] [CreationEditorAutoData]
    public void FileLink_Exists_And_ReadFileStream_ReturnsArchiveContent(IFileSystem fileSystem) {
        var dataSource = CreateDataSource(fileSystem, new FakeArchiveFile(@"meshes\test.nif", 1, 2, 3));

        var link = new DataSourceFileLink(dataSource, new DataRelativePath(@"meshes\test.nif"));

        Assert.True(link.Exists());

        using var stream = link.ReadFileStream();
        Assert.NotNull(stream);
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        Assert.Equal([1, 2, 3], memoryStream.ToArray());
    }

    [Theory] [CreationEditorAutoData]
    public void FileLink_ReadFileStream_MissingFile_ReturnsNull(IFileSystem fileSystem) {
        var dataSource = CreateDataSource(fileSystem);

        var link = new DataSourceFileLink(dataSource, new DataRelativePath(@"meshes\missing.nif"));

        Assert.False(link.Exists());
        Assert.Null(link.ReadFileStream());
    }

    [Theory] [CreationEditorAutoData]
    public void DirectoryLink_EnumerateFileLinks_WorksForArchives(IFileSystem fileSystem) {
        var dataSource = CreateDataSource(
            fileSystem,
            new FakeArchiveFile(@"meshes\test.nif", 1),
            new FakeArchiveFile(@"meshes\clutter\deep.nif", 2));

        var directoryLink = new DataSourceDirectoryLink(dataSource, new DataRelativePath("meshes"));

        var links = directoryLink.EnumerateFileLinks(false).ToList();

        var link = Assert.Single(links);
        Assert.Equal(@"meshes\test.nif", link.DataRelativePath.Path);
    }

    [Theory] [CreationEditorAutoData]
    public void DeleteDirectoryLink_DoesNotContainArchiveFiles(IFileSystem fileSystem) {
        var dataSource = CreateDataSource(fileSystem, new FakeArchiveFile(@"meshes\test.nif", 1));

        var fileLink = new DataSourceFileLink(dataSource, new DataRelativePath(@"meshes\test.nif"));

        Assert.False(dataSource.DeleteDirectoryLink.Contains(fileLink));
    }

    [Theory] [CreationEditorAutoData]
    public void CreateMemento_ContainsArchiveTypeAndPath(IFileSystem fileSystem) {
        var dataSource = CreateDataSource(fileSystem, new FakeArchiveFile(@"meshes\test.nif", 1));

        var memento = dataSource.CreateMemento();

        Assert.Equal(DataSourceType.Archive, memento.Type);
        Assert.Equal(ArchivePath, memento.Path);
        Assert.True(memento.IsReadyOnly);
    }
}
