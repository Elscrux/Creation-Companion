using System.IO.Abstractions.TestingHelpers;
using CreationEditor.Services.Cache;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Query;
using Mutagen.Bethesda.Assets;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.References.Cache;

public sealed class DictionaryReferenceCacheSerializationTests {
    private sealed class TestQuery
        : IReferenceQuery<IDataSource, DictionaryReferenceCache<string, DataRelativePath>, string, DataRelativePath> {
        public string Name => "Test Dictionary Query";
        public string GetSourceName(IDataSource source) => source.Name;
        public void FillCache(IDataSource source, DictionaryReferenceCache<string, DataRelativePath> cache) {}
        public void FillCache(DataRelativePath reference, DictionaryReferenceCache<string, DataRelativePath> cache) {}
    }

    private static DictionaryReferenceCacheSerialization<
        IDataSource,
        DictionaryReferenceCache<string, DataRelativePath>,
        string,
        DataRelativePath> CreateSerialization(MockFileSystem fileSystem) {
        Func<IReadOnlyList<string>, ICacheLocationProvider> cacheLocationProviderFactory =
            subDirectories => new CacheLocationProvider(fileSystem, subDirectories);
        var config = new AssetReferenceCacheSerializationConfigString<IDataSource>();
        return new DictionaryReferenceCacheSerialization<
            IDataSource,
            DictionaryReferenceCache<string, DataRelativePath>,
            string,
            DataRelativePath>(cacheLocationProviderFactory, config, Logger.None, fileSystem, "References");
    }

    [Fact]
    public void SerializeThenValidate_ReturnsTrue() {
        var fileSystem = new MockFileSystem();
        var serialization = CreateSerialization(fileSystem);
        var source = new FileSystemDataSource(fileSystem, @"C:\TestData");
        var query = new TestQuery();

        serialization.Serialize(source, query, CreateCache());

        Assert.True(serialization.Validate(source, query));
    }

    [Fact]
    public void SerializeThenDeserialize_RoundTripsLinksAndReferences() {
        var fileSystem = new MockFileSystem();
        var serialization = CreateSerialization(fileSystem);
        var source = new FileSystemDataSource(fileSystem, @"C:\TestData");
        var query = new TestQuery();

        serialization.Serialize(source, query, CreateCache());
        var deserialized = serialization.Deserialize(source, query);

        Assert.True(deserialized.Cache.TryGetValue("globalVariable", out var references));
        Assert.Contains(references, r => r.Path == @"scripts\test.psc");
    }

    [Fact]
    public void Validate_MissingCacheFile_ReturnsFalse() {
        var fileSystem = new MockFileSystem();
        var serialization = CreateSerialization(fileSystem);
        var source = new FileSystemDataSource(fileSystem, @"C:\TestData");

        Assert.False(serialization.Validate(source, new TestQuery()));
    }

    [Fact]
    public void Validate_CorruptedCacheFile_ReturnsFalse() {
        var fileSystem = new MockFileSystem();
        var serialization = CreateSerialization(fileSystem);
        var source = new FileSystemDataSource(fileSystem, @"C:\TestData");
        var query = new TestQuery();
        serialization.Serialize(source, query, CreateCache());
        var cacheFile = new CacheLocationProvider(fileSystem, "References").CacheFile("Test Dictionary Query", "TestData");

        fileSystem.File.WriteAllBytes(cacheFile, [1, 2, 3, 4]);

        Assert.False(serialization.Validate(source, query));
    }

    private static DictionaryReferenceCache<string, DataRelativePath> CreateCache() {
        var cache = DictionaryReferenceCache<string, DataRelativePath>.CreateNew();
        cache.Add("globalVariable", new DataRelativePath(@"scripts\test.psc"));
        return cache;
    }
}
