using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Cache;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Query;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.References.Cache;

public sealed class AssetReferenceCacheSerializationTests {
    private sealed class TestQuery
        : IReferenceQuery<IModGetter, AssetReferenceCache<IFormLinkIdentifier>, IAssetLinkGetter, IFormLinkIdentifier> {
        public string Name => "Test Asset Query";
        public string GetSourceName(IModGetter source) => source.ModKey.FileName.String;
        public void FillCache(IModGetter source, AssetReferenceCache<IFormLinkIdentifier> cache) {}
        public void FillCache(IFormLinkIdentifier reference, AssetReferenceCache<IFormLinkIdentifier> cache) {}
    }

    private static AssetReferenceCacheSerialization<IModGetter, IFormLinkIdentifier> CreateSerialization(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IReferenceCacheSerializationConfig<IModGetter, IFormLinkIdentifier> serializationConfig,
        IAssetTypeService assetTypeService,
        IFileSystem fileSystem) => new(
        cacheLocationProviderFactory,
        serializationConfig,
        assetTypeService,
        Logger.None,
        fileSystem);

    // "Mod0.esp" exists in the test data directory, so the mod hash validation can run
    private static SkyrimMod CreateMod() => new(ModKey.FromName("Mod0", ModType.Plugin), SkyrimRelease.SkyrimSE);

    private static AssetReferenceCache<IFormLinkIdentifier> CreateCache() {
        var cache = AssetReferenceCache<IFormLinkIdentifier>.CreateNew();
        cache.Add(
            new AssetLink<SkyrimModelAssetType>(@"meshes\test.nif"),
            new FormLinkInformation(new FormKey(ModKey.FromName("Mod0", ModType.Plugin), 0x800), typeof(IStaticGetter)));
        cache.Add(
            new AssetLink<SkyrimTextureAssetType>(@"textures\test.dds"),
            new FormLinkInformation(new FormKey(ModKey.FromName("Mod0", ModType.Plugin), 0x801), typeof(ITextureSetGetter)));
        return cache;
    }

    [Theory] [CreationEditorAutoData]
    public void Serialize_CreatesCacheFile(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IReferenceCacheSerializationConfig<IModGetter, IFormLinkIdentifier> serializationConfig,
        IAssetTypeService assetTypeService,
        IFileSystem fileSystem) {
        var serialization = CreateSerialization(cacheLocationProviderFactory, serializationConfig, assetTypeService, fileSystem);
        var mod = CreateMod();
        var cacheFile = cacheLocationProviderFactory(["References"]).CacheFile("Test Asset Query", "Mod0.esp");

        serialization.Serialize(mod, new TestQuery(), CreateCache());

        Assert.True(fileSystem.File.Exists(cacheFile));
    }

    [Theory] [CreationEditorAutoData]
    public void SerializeThenValidate_ReturnsTrue(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IReferenceCacheSerializationConfig<IModGetter, IFormLinkIdentifier> serializationConfig,
        IAssetTypeService assetTypeService,
        IFileSystem fileSystem) {
        var serialization = CreateSerialization(cacheLocationProviderFactory, serializationConfig, assetTypeService, fileSystem);
        var mod = CreateMod();
        var query = new TestQuery();

        serialization.Serialize(mod, query, CreateCache());

        Assert.True(serialization.Validate(mod, query));
    }

    [Theory] [CreationEditorAutoData]
    public void SerializeThenDeserialize_RoundTripsLinksAndReferences(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IReferenceCacheSerializationConfig<IModGetter, IFormLinkIdentifier> serializationConfig,
        IAssetTypeService assetTypeService,
        IFileSystem fileSystem) {
        var serialization = CreateSerialization(cacheLocationProviderFactory, serializationConfig, assetTypeService, fileSystem);
        var mod = CreateMod();
        var query = new TestQuery();
        var modelLink = new AssetLink<SkyrimModelAssetType>(@"meshes\test.nif");
        var textureLink = new AssetLink<SkyrimTextureAssetType>(@"textures\test.dds");

        serialization.Serialize(mod, query, CreateCache());
        var deserialized = serialization.Deserialize(mod, query);

        Assert.True(deserialized.HasAsset(modelLink));
        Assert.True(deserialized.HasAsset(textureLink));
        var modelReferences = deserialized.GetReferences(modelLink).ToList();
        Assert.Contains(modelReferences, r => r.FormKey == new FormKey(ModKey.FromName("Mod0", ModType.Plugin), 0x800));
        Assert.Contains(modelReferences, r => r.Type == typeof(IStaticGetter));
        var textureReferences = deserialized.GetReferences(textureLink).ToList();
        Assert.Contains(textureReferences, r => r.FormKey == new FormKey(ModKey.FromName("Mod0", ModType.Plugin), 0x801));
    }

    [Theory] [CreationEditorAutoData]
    public void Validate_MissingCacheFile_ReturnsFalse(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IReferenceCacheSerializationConfig<IModGetter, IFormLinkIdentifier> serializationConfig,
        IAssetTypeService assetTypeService,
        IFileSystem fileSystem) {
        var serialization = CreateSerialization(cacheLocationProviderFactory, serializationConfig, assetTypeService, fileSystem);
        var mod = CreateMod();

        Assert.False(serialization.Validate(mod, new TestQuery()));
    }

    [Theory] [CreationEditorAutoData]
    public void Validate_CorruptedCacheFile_ReturnsFalse(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IReferenceCacheSerializationConfig<IModGetter, IFormLinkIdentifier> serializationConfig,
        IAssetTypeService assetTypeService,
        IFileSystem fileSystem) {
        var serialization = CreateSerialization(cacheLocationProviderFactory, serializationConfig, assetTypeService, fileSystem);
        var mod = CreateMod();
        var query = new TestQuery();
        serialization.Serialize(mod, query, CreateCache());
        var cacheFile = cacheLocationProviderFactory(["References"]).CacheFile("Test Asset Query", "Mod0.esp");

        fileSystem.File.WriteAllBytes(cacheFile, [1, 2, 3, 4]);

        Assert.False(serialization.Validate(mod, query));
    }

    [Theory] [CreationEditorAutoData]
    public void Validate_ChangedModFile_ReturnsFalse(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IReferenceCacheSerializationConfig<IModGetter, IFormLinkIdentifier> serializationConfig,
        IAssetTypeService assetTypeService,
        IFileSystem fileSystem) {
        var serialization = CreateSerialization(cacheLocationProviderFactory, serializationConfig, assetTypeService, fileSystem);
        var mod = CreateMod();
        var query = new TestQuery();
        serialization.Serialize(mod, query, CreateCache());

        // Modify the mod file on disk so the stored hash no longer matches
        fileSystem.File.WriteAllBytes(@"C:\GameDirectory\Data\Mod0.esp", [0xFF, 0xFF, 0xFF]);

        Assert.False(serialization.Validate(mod, query));
    }
}
