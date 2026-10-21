using System.IO.Abstractions;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.State;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments.DI;
using Noggog.Testing.IO;
namespace CreationEditor.Skyrim.Tests.DataSource;

public sealed class DataSourceServiceTests {
    private static readonly string DataDirectory = Path.Combine(PathingUtil.DrivePrefix, "GameDirectory", "Data");
    private static readonly string ExtraSource = Path.Combine(Path.GetTempPath(), "ExtraSource");

    [Theory] [CreationEditorAutoData]
    public void DataDirectoryDataSource_PointsAtInjectedDataDirectory(
        IDataSourceService dataSourceService) {
        Assert.Equal(DataDirectory, dataSourceService.DataDirectoryDataSource.Path);
        Assert.Contains(dataSourceService.DataDirectoryDataSource, dataSourceService.ListedOrder);
        Assert.Equal(dataSourceService.DataDirectoryDataSource, dataSourceService.ActiveDataSource);
    }

    [Theory] [CreationEditorAutoData]
    public void GetFileLink_DataRelativePath_ResolvesFileInDataDirectory(
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        fileSystem.Directory.CreateDirectory(Path.Combine(DataDirectory, "meshes"));
        fileSystem.File.WriteAllText(Path.Combine(DataDirectory, "meshes", "test.nif"), "content");

        var link = dataSourceService.GetFileLink(new DataRelativePath("meshes/test.nif"));

        Assert.NotNull(link);
        Assert.Equal(Path.Combine(DataDirectory, "meshes", "test.nif"), link.FullPath);
        Assert.True(link.Exists());
    }

    [Theory] [CreationEditorAutoData]
    public void GetFileLink_MissingFile_ReturnsNull(
        IDataSourceService dataSourceService) {
        var link = dataSourceService.GetFileLink(new DataRelativePath("meshes/missing.nif"));

        Assert.Null(link);
    }

    [Theory] [CreationEditorAutoData]
    public void GetFileLink_AbsolutePathInsideDataDirectory_Resolves(
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        fileSystem.Directory.CreateDirectory(DataDirectory);
        fileSystem.File.WriteAllText(Path.Combine(DataDirectory, "test.nif"), "content");

        var link = dataSourceService.GetFileLink(Path.Combine(DataDirectory, "test.nif"));

        Assert.NotNull(link);
        Assert.Equal("test.nif", link.DataRelativePath.Path);
    }

    [Theory] [CreationEditorAutoData]
    public void GetFileLink_AbsolutePathOutsideDataDirectory_ReturnsNull(
        IDataSourceService dataSourceService) {
        var link = dataSourceService.GetFileLink(Path.Combine(Path.GetTempPath(), "Somewhere", "Else", "test.nif"));

        Assert.Null(link);
    }

    [Theory] [CreationEditorAutoData]
    public void FileExists_ReflectsDataSources(
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        fileSystem.Directory.CreateDirectory(DataDirectory);
        fileSystem.File.WriteAllText(Path.Combine(DataDirectory, "existing.nif"), "content");

        Assert.True(dataSourceService.FileExists(new DataRelativePath("existing.nif")));
        Assert.False(dataSourceService.FileExists(new DataRelativePath("missing.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public void UpdateDataSources_FiresDataSourcesChanged(
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        fileSystem.Directory.CreateDirectory(ExtraSource);
        var extraSource = new FileSystemDataSource(fileSystem, ExtraSource);
        var received = new List<IReadOnlyList<IDataSource>>();
        using var subscription = dataSourceService.DataSourcesChanged.Subscribe(received.Add);

        dataSourceService.UpdateDataSources(
            [dataSourceService.DataDirectoryDataSource, extraSource],
            dataSourceService.DataDirectoryDataSource);

        var last = received[^1];
        Assert.Equal(2, last.Count);
        Assert.Contains(extraSource, last);
    }

    [Theory] [CreationEditorAutoData]
    public void GetFileLink_HigherPrioritySourceWins(
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        fileSystem.Directory.CreateDirectory(DataDirectory);
        fileSystem.Directory.CreateDirectory(ExtraSource);
        fileSystem.File.WriteAllText(Path.Combine(DataDirectory, "duplicate.nif"), "data directory");
        fileSystem.File.WriteAllText(Path.Combine(ExtraSource, "duplicate.nif"), "extra source");

        var extraSource = new FileSystemDataSource(fileSystem, ExtraSource);
        dataSourceService.UpdateDataSources(
            [dataSourceService.DataDirectoryDataSource, extraSource],
            dataSourceService.DataDirectoryDataSource);

        var link = dataSourceService.GetFileLink(new DataRelativePath("duplicate.nif"));

        Assert.NotNull(link);
        Assert.Equal(ExtraSource, link.DataSource.Path);
    }

    [Theory] [CreationEditorAutoData]
    public void MementoPersistence_NewServiceRestoresDataSources(
        IDataSourceService dataSourceService,
        IFileSystem fileSystem,
        IIgnoredDirectoriesProvider ignoredDirectoriesProvider,
        IDataDirectoryProvider dataDirectoryProvider,
        IArchiveService archiveService,
        IStateRepositoryFactory<DataSourceMemento, DataSourceMemento, NamedGuid> stateRepositoryFactory) {
        fileSystem.Directory.CreateDirectory(ExtraSource);
        var extraSource = new FileSystemDataSource(fileSystem, ExtraSource);
        dataSourceService.UpdateDataSources(
            [dataSourceService.DataDirectoryDataSource, extraSource],
            dataSourceService.DataDirectoryDataSource);

        // A new service with the same state repository restores the registered data sources
        var restoredService = new DataSourceService(fileSystem, ignoredDirectoriesProvider, dataDirectoryProvider, archiveService, stateRepositoryFactory);

        Assert.Contains(restoredService.DataDirectoryDataSource, restoredService.ListedOrder);
        var restored = restoredService.ListedOrder.OfType<FileSystemDataSource>().FirstOrDefault(x => x.Path == ExtraSource);
        Assert.NotNull(restored);
    }

    [Theory] [CreationEditorAutoData]
    public void HasDataSource_And_TryGetDataSource_MatchByPath(
        IDataSourceService dataSourceService) {
        Assert.True(dataSourceService.HasDataSource(DataDirectory));
        Assert.False(dataSourceService.HasDataSource(Path.Combine(Path.GetTempPath(), "NotASource")));

        Assert.True(dataSourceService.TryGetDataSource(DataDirectory, out var dataSource));
        Assert.Equal(dataSourceService.DataDirectoryDataSource, dataSource);
        Assert.False(dataSourceService.TryGetDataSource(Path.Combine(Path.GetTempPath(), "NotASource"), out var missing));
        Assert.Null(missing);
    }

    [Theory] [CreationEditorAutoData]
    public void EnumerateFileLinksInAllDataSources_DeduplicatesAcrossSources(
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        fileSystem.Directory.CreateDirectory(Path.Combine(DataDirectory, "TestAssets"));
        fileSystem.Directory.CreateDirectory(Path.Combine(ExtraSource, "TestAssets"));
        fileSystem.File.WriteAllText(Path.Combine(DataDirectory, "TestAssets", "duplicate.nif"), "data directory");
        fileSystem.File.WriteAllText(Path.Combine(DataDirectory, "TestAssets", "onlyData.nif"), "data directory");
        fileSystem.File.WriteAllText(Path.Combine(ExtraSource, "TestAssets", "duplicate.nif"), "extra source");

        var extraSource = new FileSystemDataSource(fileSystem, ExtraSource);
        dataSourceService.UpdateDataSources(
            [dataSourceService.DataDirectoryDataSource, extraSource],
            dataSourceService.DataDirectoryDataSource);

        var links = dataSourceService
            .EnumerateFileLinksInAllDataSources(new DataRelativePath("TestAssets"), false)
            .ToList();

        Assert.Equal(2, links.Count);
        var duplicate = links.Single(x => x.DataRelativePath.Path == Path.Combine("TestAssets", "duplicate.nif"));
        Assert.Equal(ExtraSource, duplicate.DataSource.Path);
    }
}
