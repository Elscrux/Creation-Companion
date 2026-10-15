using System.IO.Abstractions;
using CreationEditor.Services.Cache;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Query;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.References.Cache;

public sealed class RecordReferenceCacheSerializationTests {
    private sealed class TestQuery
        : IReferenceQuery<IModGetter, RecordReferenceCache, IFormLinkIdentifier, IFormLinkIdentifier> {
        public string Name => "Test Record Query";
        public string GetSourceName(IModGetter source) => source.ModKey.FileName.String;
        public void FillCache(IModGetter source, RecordReferenceCache cache) {}
        public void FillCache(IFormLinkIdentifier reference, RecordReferenceCache cache) {}
    }

    private static RecordReferenceCacheSerialization CreateSerialization(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IMutagenTypeProvider mutagenTypeProvider,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) => new(
        cacheLocationProviderFactory,
        mutagenTypeProvider,
        dataSourceService,
        fileSystem);

    // "Mod0.esp" exists in the test data directory, so the cache file name can be derived from its hash
    private static SkyrimMod CreateMod() => new(ModKey.FromName("Mod0", ModType.Plugin), SkyrimRelease.SkyrimSE);

    [Theory] [CreationEditorAutoData]
    public void SerializeThenValidate_ReturnsTrue(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IMutagenTypeProvider mutagenTypeProvider,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var serialization = CreateSerialization(cacheLocationProviderFactory, mutagenTypeProvider, dataSourceService, fileSystem);
        var mod = CreateMod();
        var query = new TestQuery();

        serialization.Serialize(mod, query, CreateCache());

        Assert.True(serialization.Validate(mod, query));
    }

    [Theory] [CreationEditorAutoData]
    public void SerializeThenDeserialize_RoundTripsFormKeysAndReferences(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IMutagenTypeProvider mutagenTypeProvider,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var serialization = CreateSerialization(cacheLocationProviderFactory, mutagenTypeProvider, dataSourceService, fileSystem);
        var mod = CreateMod();
        var query = new TestQuery();
        var armorAddonLink = new FormLinkInformation(new FormKey(ModKey.FromName("Mod0", ModType.Plugin), 0x800), typeof(IArmorAddonGetter));
        var armorReference = new FormLinkInformation(new FormKey(ModKey.FromName("Mod0", ModType.Plugin), 0x801), typeof(IArmorGetter));

        serialization.Serialize(mod, query, CreateCache());
        var deserialized = serialization.Deserialize(mod, query);

        Assert.Contains(armorAddonLink.FormKey, deserialized.FormKeys);
        Assert.True(deserialized.Cache.TryGetValue(armorAddonLink.FormKey, out var references));
        Assert.Contains(references, r => r.FormKey == armorReference.FormKey && r.Type == typeof(IArmorGetter));
    }

    [Theory] [CreationEditorAutoData]
    public void Validate_MissingCacheFile_ReturnsFalse(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IMutagenTypeProvider mutagenTypeProvider,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var serialization = CreateSerialization(cacheLocationProviderFactory, mutagenTypeProvider, dataSourceService, fileSystem);
        var mod = CreateMod();

        Assert.False(serialization.Validate(mod, new TestQuery()));
    }

    [Theory] [CreationEditorAutoData]
    public void Serialize_MissingModFile_DoesNotCreateCacheFile(
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory,
        IMutagenTypeProvider mutagenTypeProvider,
        IDataSourceService dataSourceService,
        IFileSystem fileSystem) {
        var serialization = CreateSerialization(cacheLocationProviderFactory, mutagenTypeProvider, dataSourceService, fileSystem);
        var mod = new SkyrimMod(ModKey.FromName("MissingMod", ModType.Plugin), SkyrimRelease.SkyrimSE);

        serialization.Serialize(mod, new TestQuery(), CreateCache());

        // No cache file was written anywhere for this query
        var cacheDirectory = cacheLocationProviderFactory(["References"]).CacheFile("Test Record Query", "x");
        var directory = fileSystem.Path.GetDirectoryName(cacheDirectory)!;
        Assert.False(fileSystem.Directory.Exists(directory));
    }

    private static RecordReferenceCache CreateCache() {
        var cache = RecordReferenceCache.CreateNew();
        cache.Add(
            new FormLinkInformation(new FormKey(ModKey.FromName("Mod0", ModType.Plugin), 0x800), typeof(IArmorAddonGetter)),
            new FormLinkInformation(new FormKey(ModKey.FromName("Mod0", ModType.Plugin), 0x801), typeof(IArmorGetter)));
        return cache;
    }
}
