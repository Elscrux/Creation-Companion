using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Query;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.References.Query;

public sealed class ModAssetSerializableQueryTests {
    [Theory] [CreationEditorAutoData]
    public void FillCache_MapsAssetLinksToRecords(
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        ILinkCacheProvider linkCacheProvider,
        IAssetTypeService assetTypeService,
        IRecordController recordController) {
        using var query = new ModAssetSerializableQuery(Logger.None, assetTypeService, linkCacheProvider);

        var staticRecord = recordController.CreateRecord<Static, IStaticGetter>();
        recordController.RegisterUpdate(staticRecord,
            () => staticRecord.Model = new Model {
                File = new AssetLink<SkyrimModelAssetType>(@"meshes\test.nif"),
            });

        var cache = AssetReferenceCache<IFormLinkIdentifier>.CreateNew();
        query.FillCache(editorEnvironment.ActiveMod, cache);

        var references = cache.GetReferences(new AssetLink<SkyrimModelAssetType>(@"meshes\test.nif")).ToList();
        Assert.Contains(references, r => r.FormKey == staticRecord.FormKey);
    }

    [Theory] [CreationEditorAutoData]
    public void FillCache_RecordWithoutAssetLinks_AddsNoEntries(
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        ILinkCacheProvider linkCacheProvider,
        IAssetTypeService assetTypeService,
        IRecordController recordController) {
        using var query = new ModAssetSerializableQuery(Logger.None, assetTypeService, linkCacheProvider);

        recordController.CreateRecord<Static, IStaticGetter>();

        var cache = AssetReferenceCache<IFormLinkIdentifier>.CreateNew();
        query.FillCache(editorEnvironment.ActiveMod, cache);

        Assert.Empty(cache.Cache);
    }

    [Theory] [CreationEditorAutoData]
    public void ParseRecord_ReturnsAssetLinksOfRecord(
        ILinkCacheProvider linkCacheProvider,
        IAssetTypeService assetTypeService,
        IRecordController recordController) {
        using var query = new ModAssetSerializableQuery(Logger.None, assetTypeService, linkCacheProvider);

        var staticRecord = recordController.CreateRecord<Static, IStaticGetter>();
        recordController.RegisterUpdate(staticRecord,
            () => staticRecord.Model = new Model {
                File = new AssetLink<SkyrimModelAssetType>(@"meshes\test.nif"),
            });

        var links = query.ParseRecord(staticRecord).ToList();

        Assert.Contains(links, l => l.DataRelativePath.Path == @"meshes\test.nif");
    }
}
