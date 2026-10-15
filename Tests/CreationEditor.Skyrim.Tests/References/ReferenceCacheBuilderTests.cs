using CreationEditor.Services.Cache.Validation;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Cache.Serialization;
using CreationEditor.Services.Mutagen.References.Query;
using Serilog.Core;
namespace CreationEditor.Skyrim.Tests.References;

public sealed class ReferenceCacheBuilderTests {
    private readonly ReferenceCacheBuilder _builder = new(Logger.None);

    private sealed class FakeQuery : IReferenceQuery<string, DictionaryReferenceCache<string, int>, string, int> {
        public string Name => "TestQuery";
        public List<DictionaryReferenceCache<string, int>> FilledSourceCaches { get; } = [];
        public List<int> FilledReferences { get; } = [];
        public Action<DictionaryReferenceCache<string, int>>? OnFillSource { get; set; }

        public string GetSourceName(string source) => source;

        public void FillCache(string source, DictionaryReferenceCache<string, int> cache) {
            FilledSourceCaches.Add(cache);
            OnFillSource?.Invoke(cache);
        }

        public void FillCache(int reference, DictionaryReferenceCache<string, int> cache) {
            FilledReferences.Add(reference);
            cache.Add("parsed", reference);
        }
    }

    private sealed class FakeSerialization
        : IReferenceCacheSerialization<string, DictionaryReferenceCache<string, int>, string, int> {
        public bool ValidateResult { get; set; }
        public bool ThrowOnDeserialize { get; set; }
        public DictionaryReferenceCache<string, int>? DeserializeResult { get; set; }

        public int ValidateCalls { get; private set; }
        public int DeserializeCalls { get; private set; }
        public List<DictionaryReferenceCache<string, int>> SerializedCaches { get; } = [];

        public bool Validate(
            string source,
            IReferenceQuery<string, DictionaryReferenceCache<string, int>, string, int> query) {
            ValidateCalls++;
            return ValidateResult;
        }

        public DictionaryReferenceCache<string, int> Deserialize(
            string source,
            IReferenceQuery<string, DictionaryReferenceCache<string, int>, string, int> query) {
            DeserializeCalls++;
            if (ThrowOnDeserialize) throw new IOException("deserialize failed");

            return DeserializeResult ?? DictionaryReferenceCache<string, int>.CreateNew();
        }

        public void Serialize(
            string source,
            IReferenceQuery<string, DictionaryReferenceCache<string, int>, string, int> query,
            DictionaryReferenceCache<string, int> cache) {
            SerializedCaches.Add(cache);
        }
    }

    private sealed class FakeValidation : IInternalCacheValidation<string, int> {
        public CacheValidationResult<int> Result { get; set; } = CacheValidationResult<int>.Valid();
        public int Calls { get; private set; }
        public bool UpdateCacheInvoked { get; private set; }

        public Task<CacheValidationResult<int>> GetInvalidatedContent(string source) {
            Calls++;
            return Task.FromResult(Result);
        }

        public CacheValidationResult<int> PartlyInvalid(IReadOnlyList<int> content) {
            return CacheValidationResult<int>.PartlyInvalid(content, () => UpdateCacheInvoked = true);
        }

        public CacheValidationResult<int> FullyInvalid() {
            return CacheValidationResult<int>.FullyInvalid(() => UpdateCacheInvoked = true);
        }
    }

    private static DictionaryReferenceCache<string, int> CreateCache(params (string Link, int Reference)[] entries) {
        var cache = DictionaryReferenceCache<string, int>.CreateNew();
        foreach (var (link, reference) in entries) {
            cache.Add(link, reference);
        }
        return cache;
    }

    [Fact]
    public async Task NoSerialization_ParsesSourceDirectly() {
        var query = new FakeQuery {
            OnFillSource = cache => cache.Add("link1", 1),
        };

        var cache = await _builder.BuildCache("source", query);

        Assert.Single(query.FilledSourceCaches);
        Assert.Equal([1], cache.Cache["link1"]);
    }

    [Fact]
    public async Task ValidCache_DeserializesWithoutParsing() {
        var query = new FakeQuery();
        var serialization = new FakeSerialization {
            ValidateResult = true,
            DeserializeResult = CreateCache(("link1", 1)),
        };

        var cache = await _builder.BuildCache("source", query, serialization);

        Assert.Equal(1, serialization.DeserializeCalls);
        Assert.Empty(query.FilledSourceCaches);
        Assert.Equal([1], cache.Cache["link1"]);
    }

    [Fact]
    public async Task InvalidCache_FullyParsesAndSerializes() {
        var query = new FakeQuery {
            OnFillSource = cache => cache.Add("link1", 1),
        };
        var serialization = new FakeSerialization {
            ValidateResult = false,
        };

        var cache = await _builder.BuildCache("source", query, serialization);

        Assert.Single(query.FilledSourceCaches);
        Assert.Equal(0, serialization.DeserializeCalls);
        var serialized = Assert.Single(serialization.SerializedCaches);
        Assert.Equal([1], serialized.Cache["link1"]);
        Assert.Equal([1], cache.Cache["link1"]);
    }

    [Fact]
    public async Task DeserializeFailure_FallsBackToFullParse() {
        var query = new FakeQuery {
            OnFillSource = cache => cache.Add("link1", 1),
        };
        var serialization = new FakeSerialization {
            ValidateResult = true,
            ThrowOnDeserialize = true,
        };

        await _builder.BuildCache("source", query, serialization);

        Assert.Single(query.FilledSourceCaches);
        var serialized = Assert.Single(serialization.SerializedCaches);
        Assert.Equal([1], serialized.Cache["link1"]);
    }

    [Fact]
    public async Task ValidCache_NoInvalidatedContent_Deserializes() {
        var query = new FakeQuery();
        var serialization = new FakeSerialization {
            ValidateResult = true,
            DeserializeResult = CreateCache(("link1", 1)),
        };
        var validation = new FakeValidation(); // Valid()

        var cache = await _builder.BuildCache("source", query, serialization, validation);

        Assert.Equal(1, validation.Calls);
        Assert.Equal(1, serialization.DeserializeCalls);
        Assert.Empty(query.FilledSourceCaches);
        Assert.Equal([1], cache.Cache["link1"]);
    }

    [Fact]
    public async Task ValidCache_FullyInvalidatedContent_FullyParses() {
        var query = new FakeQuery {
            OnFillSource = cache => cache.Add("link1", 1),
        };
        var serialization = new FakeSerialization {
            ValidateResult = true,
        };
        // Build a fully invalid result that flags UpdateCache on this instance
        var validation = new FakeValidation();
        validation.Result = validation.FullyInvalid();

        var cache = await _builder.BuildCache("source", query, serialization, validation);

        Assert.Single(query.FilledSourceCaches);
        Assert.Equal(0, serialization.DeserializeCalls);
        Assert.Single(serialization.SerializedCaches);
        Assert.True(validation.UpdateCacheInvoked);
        Assert.Equal([1], cache.Cache["link1"]);
    }

    [Fact]
    public async Task ValidCache_PartialInvalidation_MergesDeserializedAndParsed() {
        var query = new FakeQuery();
        var serialization = new FakeSerialization {
            ValidateResult = true,
            DeserializeResult = CreateCache(("a", 1), ("a", 2)),
        };
        var validation = new FakeValidation();
        validation.Result = validation.PartlyInvalid([1]);

        var cache = await _builder.BuildCache("source", query, serialization, validation);

        // Old reference 1 removed from deserialized cache, newly parsed reference added
        Assert.Equal([2], cache.Cache["a"]);
        Assert.Equal([1], cache.Cache["parsed"]);

        // Merged cache is re-serialized
        var serialized = Assert.Single(serialization.SerializedCaches);
        Assert.Equal([2], serialized.Cache["a"]);
        Assert.Equal([1], serialized.Cache["parsed"]);

        // Validation cache is updated after the merge
        Assert.True(validation.UpdateCacheInvoked);
    }

    [Fact]
    public async Task InvalidCache_WithValidation_UpdatesValidationCache() {
        var query = new FakeQuery {
            OnFillSource = cache => cache.Add("link1", 1),
        };
        var serialization = new FakeSerialization {
            ValidateResult = false,
        };
        var validation = new FakeValidation();
        validation.Result = validation.PartlyInvalid([1]);

        var cache = await _builder.BuildCache("source", query, serialization, validation);

        Assert.Single(query.FilledSourceCaches);
        Assert.Single(serialization.SerializedCaches);
        Assert.True(validation.UpdateCacheInvoked);
        Assert.Equal([1], cache.Cache["link1"]);
    }
}
