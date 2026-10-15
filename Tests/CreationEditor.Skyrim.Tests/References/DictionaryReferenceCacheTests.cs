using System.Collections.Concurrent;
using CreationEditor.Services.Mutagen.References.Cache;
namespace CreationEditor.Skyrim.Tests.References;

public sealed class DictionaryReferenceCacheTests {
    [Fact]
    public void Add_PopulatesForwardAndReverseCache() {
        var cache = DictionaryReferenceCache<string, int>.CreateNew();

        cache.Add("link1", 1);

        Assert.Equal([1], cache.Cache["link1"]);
        Assert.Equal(["link1"], cache.ReverseCache[1]);
    }

    [Fact]
    public void Add_SameReferenceUnderMultipleLinks_TracksBothLinks() {
        var cache = DictionaryReferenceCache<string, int>.CreateNew();

        cache.Add("link1", 1);
        cache.Add("link2", 1);

        Assert.Equal([1], cache.Cache["link1"]);
        Assert.Equal([1], cache.Cache["link2"]);
        Assert.Equal(["link1", "link2"], cache.ReverseCache[1]);
    }

    [Fact]
    public void Add_DuplicateReference_IsNotDuplicated() {
        var cache = DictionaryReferenceCache<string, int>.CreateNew();

        cache.Add("link1", 1);
        cache.Add("link1", 1);

        Assert.Single(cache.Cache["link1"]);
        Assert.Single(cache.ReverseCache[1]);
    }

    [Fact]
    public void Add_OtherCache_MergesForwardAndReverse() {
        var cache = DictionaryReferenceCache<string, int>.CreateNew();
        cache.Add("link1", 1);

        var other = DictionaryReferenceCache<string, int>.CreateNew();
        other.Add("link2", 2);

        cache.Add(other);

        Assert.Equal([1], cache.Cache["link1"]);
        Assert.Equal([2], cache.Cache["link2"]);
        Assert.Equal(["link1"], cache.ReverseCache[1]);
        Assert.Equal(["link2"], cache.ReverseCache[2]);
    }

    [Fact]
    public void Add_OtherCache_UnionizesOverlappingLinks() {
        var cache = DictionaryReferenceCache<string, int>.CreateNew();
        cache.Add("link1", 1);

        var other = DictionaryReferenceCache<string, int>.CreateNew();
        other.Add("link1", 2);

        cache.Add(other);

        Assert.Equal([1, 2], cache.Cache["link1"]);
        Assert.Equal(["link1"], cache.ReverseCache[1]);
        Assert.Equal(["link1"], cache.ReverseCache[2]);
    }

    [Fact]
    public void Remove_RemovesReferencesFromForwardCache() {
        var cache = DictionaryReferenceCache<string, int>.CreateNew();
        cache.Add("link1", 1);
        cache.Add("link1", 2);

        cache.Remove([1]);

        Assert.Equal([2], cache.Cache["link1"]);
    }

    [Fact]
    public void Remove_RemovesReverseCacheEntries() {
        var cache = DictionaryReferenceCache<string, int>.CreateNew();
        cache.Add("link1", 1);
        cache.Add("link2", 1);

        cache.Remove([1]);

        Assert.False(cache.ReverseCache.ContainsKey(1));
    }

    [Fact]
    public void Remove_NonExistingReference_IsNoOp() {
        var cache = DictionaryReferenceCache<string, int>.CreateNew();
        cache.Add("link1", 1);

        cache.Remove([42]);

        Assert.Equal([1], cache.Cache["link1"]);
        Assert.Equal(["link1"], cache.ReverseCache[1]);
    }

    [Fact]
    public void CustomComparer_IsRespected() {
        var cache = new DictionaryReferenceCache<string, string>(
            new ConcurrentDictionary<string, HashSet<string>>(),
            StringComparer.OrdinalIgnoreCase);

        cache.Add("link1", "Value");
        cache.Add("link1", "value");

        Assert.Single(cache.Cache["link1"]);
        Assert.Single(cache.ReverseCache["Value"]);
    }
}
