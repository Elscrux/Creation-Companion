using System.IO.Abstractions;
using System.Reactive.Linq;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog.Testing.IO;
namespace CreationEditor.Skyrim.Tests.Asset;

public sealed class AssetControllerTests {
    private static readonly string DataDirectory = Path.Combine(PathingUtil.DrivePrefix, "GameDirectory", "Data");

    private static DataSourceFileLink CreateFile(IDataSourceService dataSourceService, IFileSystem fileSystem, string relativePath, byte[] content) {
        var fullPath = fileSystem.Path.Combine(DataDirectory, relativePath);
        var directory = fileSystem.Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory)) fileSystem.Directory.CreateDirectory(directory);
        fileSystem.File.WriteAllBytes(fullPath, content);

        var link = dataSourceService.GetFileLink(relativePath);
        Assert.NotNull(link);
        return link;
    }

    private static DataSourceFileLink CreateFileLink(IDataSourceService dataSourceService, string relativePath) {
        return new DataSourceFileLink(dataSourceService.DataDirectoryDataSource, relativePath);
    }

    /// <summary>
    /// Waits until the reference service finished loading all record references,
    /// so that record mutations are reliably picked up by the reference tracking.
    /// </summary>
    private static async Task WaitForReferenceLoading(IReferenceService referenceService) {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var subscription = referenceService.IsLoadingRecordReferences
            .Where(loading => !loading)
            .Subscribe(_ => completion.TrySetResult());

        if (!referenceService.IsLoadingRecordReferencesCurrently) {
            completion.TrySetResult();
        }

        await completion.Task.WaitAsync(TimeSpan.FromSeconds(10));
    }

    [Theory] [CreationEditorAutoData]
    public void Delete_File_MovesFileIntoDeleteDirectory(
        IAssetController assetController,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var link = CreateFile(dataSourceService, fileSystem, "meshes/test.nif", [1, 2, 3]);

        assetController.Delete(link);

        Assert.False(fileSystem.File.Exists(link.FullPath));
        Assert.True(fileSystem.File.Exists(fileSystem.Path.Combine(DataDirectory, "_DELETE_", "meshes", "test.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public void Delete_Directory_MovesDirectoryIntoDeleteDirectory(
        IAssetController assetController,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        CreateFile(dataSourceService, fileSystem, "meshes/testdir/a.nif", [1]);
        CreateFile(dataSourceService, fileSystem, "meshes/testdir/b.nif", [2]);
        var directoryLink = new DataSourceDirectoryLink(dataSourceService.DataDirectoryDataSource, "meshes/testdir");

        assetController.Delete(directoryLink);

        Assert.False(fileSystem.Directory.Exists(fileSystem.Path.Combine(DataDirectory, "meshes", "testdir")));
        Assert.True(fileSystem.File.Exists(fileSystem.Path.Combine(DataDirectory, "_DELETE_", "meshes", "testdir", "a.nif")));
        Assert.True(fileSystem.File.Exists(fileSystem.Path.Combine(DataDirectory, "_DELETE_", "meshes", "testdir", "b.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public void Copy_File_CreatesCopyAtDestination(
        IAssetController assetController,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var origin = CreateFile(dataSourceService, fileSystem, "meshes/test.nif", [1, 2, 3]);
        var destination = CreateFileLink(dataSourceService, "textures/test.nif");

        assetController.Copy(origin, destination, overwrite: false);

        Assert.True(fileSystem.File.Exists(origin.FullPath));
        Assert.True(fileSystem.File.Exists(destination.FullPath));
        Assert.Equal([1, 2, 3], fileSystem.File.ReadAllBytes(destination.FullPath));
    }

    [Theory] [CreationEditorAutoData]
    public void Copy_File_ExistingDestinationWithoutOverwrite_KeepsOriginal(
        IAssetController assetController,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var origin = CreateFile(dataSourceService, fileSystem, "meshes/test.nif", [1, 2, 3]);
        var destination = CreateFile(dataSourceService, fileSystem, "textures/test.nif", [9, 9, 9]);

        assetController.Copy(origin, destination, overwrite: false);

        Assert.Equal([9, 9, 9], fileSystem.File.ReadAllBytes(destination.FullPath));
    }

    [Theory] [CreationEditorAutoData]
    public void Move_File_RemovesOriginAndCreatesDestination(
        IAssetController assetController,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var origin = CreateFile(dataSourceService, fileSystem, "meshes/test.nif", [1, 2, 3]);
        var destination = CreateFileLink(dataSourceService, "textures/test.nif");

        assetController.Move(origin, destination, overwrite: false);

        Assert.False(fileSystem.File.Exists(origin.FullPath));
        Assert.True(fileSystem.File.Exists(destination.FullPath));
        Assert.Equal([1, 2, 3], fileSystem.File.ReadAllBytes(destination.FullPath));
    }

    [Theory] [CreationEditorAutoData]
    public void Rename_File_RenamesFile(
        IAssetController assetController,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var origin = CreateFile(dataSourceService, fileSystem, "meshes/test.nif", [1, 2, 3]);

        assetController.Rename(origin, "renamed.nif", overwrite: false);

        Assert.False(fileSystem.File.Exists(origin.FullPath));
        Assert.True(fileSystem.File.Exists(fileSystem.Path.Combine(DataDirectory, "meshes", "renamed.nif")));
        Assert.Equal([1, 2, 3], fileSystem.File.ReadAllBytes(fileSystem.Path.Combine(DataDirectory, "meshes", "renamed.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public async Task Rename_File_RemapsRecordAssetReferences(
        IAssetController assetController,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem,
        IRecordController recordController,
        IReferenceService referenceService) {
        await WaitForReferenceLoading(referenceService);

        // Create a record referencing the asset
        var staticRecord = recordController.CreateRecord<Static, IStaticGetter>();
        recordController.RegisterUpdate(staticRecord, () => staticRecord.Model = new Model {
            File = new AssetLink<SkyrimModelAssetType>("Meshes/test.nif"),
        });

        var oldLink = new AssetLink<SkyrimModelAssetType>("Meshes/test.nif");
        Assert.Contains(staticRecord.FormKey, referenceService.GetRecordReferences(oldLink).Select(x => x.FormKey));

        // Create the referenced file and rename it
        var fileLink = CreateFile(dataSourceService, fileSystem, "Meshes/test.nif", [1, 2, 3]);

        assetController.Rename(fileLink, "renamed.nif", overwrite: false);

        // File was moved
        Assert.False(fileSystem.File.Exists(fileLink.FullPath));
        Assert.True(fileSystem.File.Exists(fileSystem.Path.Combine(DataDirectory, "Meshes", "renamed.nif")));

        // Record asset reference was remapped to the new path
        Assert.Equal(Path.Combine("Meshes", "renamed.nif"), staticRecord.Model!.File.DataRelativePath.Path, ignoreCase: true);

        // Reference tracking reflects the remap
        var newLink = new AssetLink<SkyrimModelAssetType>("Meshes/renamed.nif");
        Assert.DoesNotContain(staticRecord.FormKey, referenceService.GetRecordReferences(oldLink).Select(x => x.FormKey));
        Assert.Contains(staticRecord.FormKey, referenceService.GetRecordReferences(newLink).Select(x => x.FormKey));
    }
}
