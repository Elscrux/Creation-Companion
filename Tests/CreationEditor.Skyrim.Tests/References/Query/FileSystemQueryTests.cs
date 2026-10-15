using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Parser;
using CreationEditor.Services.Mutagen.References.Query;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim.Assets;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.References.Query;

public sealed class FileSystemQueryTests {
    private const string DataDirectory = @"C:\GameDirectory\Data";

    private static FileSystemQuery<AssetReferenceCache<DataRelativePath>, IAssetLinkGetter> CreateQuery(
        IAssetTypeService assetTypeService,
        IDataSourceService dataSourceService) {
        var parser = new ScriptFileParser(assetTypeService, Logger.None);
        return new FileSystemQuery<AssetReferenceCache<DataRelativePath>, IAssetLinkGetter>(parser, assetTypeService, dataSourceService);
    }

    private static void WriteFile(IFileSystem fileSystem, string relativePath, string content) {
        var fullPath = fileSystem.Path.Combine(DataDirectory, relativePath);
        var directory = fileSystem.Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory)) fileSystem.Directory.CreateDirectory(directory);
        fileSystem.File.WriteAllText(fullPath, content);
    }

    [Theory] [CreationEditorAutoData]
    public void FillCache_ParsesMatchingFilesInDataSource(
        IAssetTypeService assetTypeService,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var query = CreateQuery(assetTypeService, dataSourceService);
        WriteFile(fileSystem, @"scripts\source\child.psc", "ScriptName Child extends Parent\n");

        var cache = AssetReferenceCache<DataRelativePath>.CreateNew();
        query.FillCache(dataSourceService.DataDirectoryDataSource, cache);

        var sourceReferences = cache.GetReferences(new AssetLink<SkyrimScriptSourceAssetType>(@"Scripts\Source\Parent.psc")).ToList();
        Assert.Contains(sourceReferences, r => r.Path == @"scripts\source\child.psc");
        var compiledReferences = cache.GetReferences(new AssetLink<SkyrimScriptCompiledAssetType>(@"Scripts\Parent.pex")).ToList();
        Assert.Contains(compiledReferences, r => r.Path == @"scripts\source\child.psc");
    }

    [Theory] [CreationEditorAutoData]
    public void FillCache_IgnoresFilesOfOtherAssetTypes(
        IAssetTypeService assetTypeService,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var query = CreateQuery(assetTypeService, dataSourceService);
        WriteFile(fileSystem, @"meshes\test.nif", "not a script");

        var cache = AssetReferenceCache<DataRelativePath>.CreateNew();
        query.FillCache(dataSourceService.DataDirectoryDataSource, cache);

        Assert.Empty(cache.Cache);
    }

    [Theory] [CreationEditorAutoData]
    public void FillCache_Reference_ParsesSingleFile(
        IAssetTypeService assetTypeService,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var query = CreateQuery(assetTypeService, dataSourceService);
        WriteFile(fileSystem, @"scripts\source\child.psc", "ScriptName Child extends Parent\n");

        var cache = AssetReferenceCache<DataRelativePath>.CreateNew();
        query.FillCache(new DataRelativePath(@"scripts\source\child.psc"), cache);

        var references = cache.GetReferences(new AssetLink<SkyrimScriptSourceAssetType>(@"Scripts\Source\Parent.psc")).ToList();
        Assert.Contains(references, r => r.Path == @"scripts\source\child.psc");
    }

    [Theory] [CreationEditorAutoData]
    public void FillCache_Reference_NonScriptFile_AddsNoEntries(
        IAssetTypeService assetTypeService,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var query = CreateQuery(assetTypeService, dataSourceService);
        WriteFile(fileSystem, @"meshes\test.nif", "not a script");

        var cache = AssetReferenceCache<DataRelativePath>.CreateNew();
        query.FillCache(new DataRelativePath(@"meshes\test.nif"), cache);

        Assert.Empty(cache.Cache);
    }

    [Theory] [CreationEditorAutoData]
    public void ReferenceToSource_ReturnsFileLinkOfExistingFile(
        IAssetTypeService assetTypeService,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var query = CreateQuery(assetTypeService, dataSourceService);
        WriteFile(fileSystem, @"scripts\source\child.psc", "ScriptName Child\n");

        var link = query.ReferenceToSource(new DataRelativePath(@"scripts\source\child.psc"));

        Assert.NotNull(link);
        Assert.Equal(@"scripts\source\child.psc", link.DataRelativePath.Path);
    }

    [Theory] [CreationEditorAutoData]
    public void ReferenceToSource_MissingFile_ReturnsNull(
        IAssetTypeService assetTypeService,
        IDataSourceService dataSourceService) {
        var query = CreateQuery(assetTypeService, dataSourceService);

        var link = query.ReferenceToSource(new DataRelativePath(@"scripts\source\missing.psc"));

        Assert.Null(link);
    }
}
