using System.IO.Abstractions;
using CreationEditor.Services.DataSource;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Skyrim.Tests.DataSource;

public sealed class FileSystemDataSourceTests {
    private const string DataSourceDirectory = @"C:\TestDataSource";

    [Theory] [CreationEditorAutoData]
    public void FileExists_ChecksUnderlyingFileSystem(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);
        fileSystem.Directory.CreateDirectory(DataSourceDirectory);
        fileSystem.File.WriteAllText(Path.Combine(DataSourceDirectory, "test.nif"), "content");

        Assert.True(dataSource.FileExists(new DataRelativePath("test.nif")));
        Assert.False(dataSource.FileExists(new DataRelativePath("missing.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public void EnumerateFiles_ExcludesDeleteDirectory(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);
        fileSystem.Directory.CreateDirectory(DataSourceDirectory);
        fileSystem.Directory.CreateDirectory(Path.Combine(DataSourceDirectory, "_DELETE_"));
        fileSystem.File.WriteAllText(Path.Combine(DataSourceDirectory, "kept.nif"), "content");
        fileSystem.File.WriteAllText(Path.Combine(DataSourceDirectory, "_DELETE_", "deleted.nif"), "content");

        var files = dataSource.EnumerateFiles(new DataRelativePath(""), includeSubDirectories: true).ToList();

        var file = Assert.Single(files);
        Assert.Equal("kept.nif", file.DataRelativePath.Path);
    }

    [Theory] [CreationEditorAutoData]
    public void Create_CreatesMissingDirectories(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);

        using var stream = dataSource.Create(new DataRelativePath(@"meshes\new.nif"));

        Assert.True(fileSystem.File.Exists(Path.Combine(DataSourceDirectory, "meshes", "new.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public void Delete_RemovesFile(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);
        fileSystem.Directory.CreateDirectory(DataSourceDirectory);
        fileSystem.File.WriteAllText(Path.Combine(DataSourceDirectory, "test.nif"), "content");

        dataSource.Delete(new DataRelativePath("test.nif"));

        Assert.False(fileSystem.File.Exists(Path.Combine(DataSourceDirectory, "test.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public void Move_RenamesFile(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);
        fileSystem.Directory.CreateDirectory(DataSourceDirectory);
        fileSystem.File.WriteAllText(Path.Combine(DataSourceDirectory, "old.nif"), "content");

        dataSource.Move(new DataRelativePath("old.nif"), "new.nif");

        Assert.False(fileSystem.File.Exists(Path.Combine(DataSourceDirectory, "old.nif")));
        Assert.True(fileSystem.File.Exists(Path.Combine(DataSourceDirectory, "new.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public void Copy_DuplicatesFile(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);
        fileSystem.Directory.CreateDirectory(DataSourceDirectory);
        fileSystem.File.WriteAllText(Path.Combine(DataSourceDirectory, "original.nif"), "content");

        dataSource.Copy(new DataRelativePath("original.nif"), "copy.nif");

        Assert.True(fileSystem.File.Exists(Path.Combine(DataSourceDirectory, "original.nif")));
        Assert.True(fileSystem.File.Exists(Path.Combine(DataSourceDirectory, "copy.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public void CreateMemento_ContainsTypeAndPath(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory, true);

        var memento = dataSource.CreateMemento();

        Assert.Equal(DataSourceType.FileSystem, memento.Type);
        Assert.Equal(DataSourceDirectory, memento.Path);
        Assert.True(memento.IsReadyOnly);
    }

    [Theory] [CreationEditorAutoData]
    public void FileLink_Exists_FullPath_AndName(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);
        fileSystem.Directory.CreateDirectory(Path.Combine(DataSourceDirectory, "meshes"));
        fileSystem.File.WriteAllText(Path.Combine(DataSourceDirectory, "meshes", "test.nif"), "content");

        var link = new DataSourceFileLink(dataSource, new DataRelativePath(@"meshes\test.nif"));

        Assert.True(link.Exists());
        Assert.Equal(Path.Combine(DataSourceDirectory, "meshes", "test.nif"), link.FullPath);
        Assert.Equal("test.nif", link.Name);
        Assert.Equal("test", link.NameWithoutExtension);
        Assert.Equal(".nif", link.Extension);
    }

    [Theory] [CreationEditorAutoData]
    public void FileLink_WithExtension_ChangesExtension(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);

        var link = new DataSourceFileLink(dataSource, new DataRelativePath("test.nif"));
        var renamed = (DataSourceFileLink) link.WithExtension(".dds");

        Assert.Equal("test.dds", renamed.DataRelativePath.Path);
    }

    [Theory] [CreationEditorAutoData]
    public void FileLink_ReadFileStream_ReturnsContent(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);
        fileSystem.Directory.CreateDirectory(DataSourceDirectory);
        fileSystem.File.WriteAllText(Path.Combine(DataSourceDirectory, "test.nif"), "content");

        var link = new DataSourceFileLink(dataSource, new DataRelativePath("test.nif"));

        using var stream = link.ReadFileStream();
        Assert.NotNull(stream);
        using var reader = new StreamReader(stream);
        Assert.Equal("content", reader.ReadToEnd());
    }

    [Theory] [CreationEditorAutoData]
    public void DirectoryLink_Contains_MatchesSubPaths(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);

        var directoryLink = new DataSourceDirectoryLink(dataSource, @"meshes\clutter");

        Assert.True(directoryLink.Contains(new DataRelativePath(@"meshes\clutter\test.nif")));
        Assert.False(directoryLink.Contains(new DataRelativePath(@"meshes\other\test.nif")));
        Assert.False(directoryLink.Contains(new DataRelativePath(@"meshes\clutter")));
    }

    [Theory] [CreationEditorAutoData]
    public void DeleteDirectoryLink_HidesContainedFiles(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);

        Assert.True(dataSource.DeleteDirectoryLink.Contains(new DataRelativePath(@"_DELETE_\test.nif")));
        Assert.False(dataSource.DeleteDirectoryLink.Contains(new DataRelativePath(@"meshes\test.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public void FileLink_Equals_ComparesDataSourceAndPath(
        IFileSystem fileSystem) {
        var dataSource = new FileSystemDataSource(fileSystem, DataSourceDirectory);
        var dataSource2 = new FileSystemDataSource(fileSystem, DataSourceDirectory);

        var link1 = new DataSourceFileLink(dataSource, new DataRelativePath("test.nif"));
        var link2 = new DataSourceFileLink(dataSource2, new DataRelativePath("test.nif"));
        var link3 = new DataSourceFileLink(dataSource, new DataRelativePath("other.nif"));

        Assert.Equal(link1, link2);
        Assert.NotEqual(link1, link3);
    }
}
