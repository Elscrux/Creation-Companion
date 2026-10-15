using CreationEditor.Services.Mutagen.References.Cache;
namespace CreationEditor.Skyrim.Tests.References;

public sealed class DictionaryReferenceCacheControllerTests {
    private static DictionaryReferenceCacheController<string, string, int> CreateController() {
        return new DictionaryReferenceCacheController<string, string, int>();
    }

    [Fact]
    public void AddLink_AddsReferenceForEveryLink() {
        var controller = CreateController();
        var cache = DictionaryReferenceCache<string, int>.CreateNew();

        controller.AddLink(cache, 1, ["link1", "link2"]);

        Assert.Equal([1], cache.Cache["link1"]);
        Assert.Equal([1], cache.Cache["link2"]);
    }

    [Fact]
    public void RemoveLink_RemovesWholeLinkEntry() {
        var controller = CreateController();
        var cache = DictionaryReferenceCache<string, int>.CreateNew();
        cache.Add("link1", 1);
        cache.Add("link1", 2);

        controller.RemoveLink(cache, 1, ["link1"]);

        Assert.False(cache.Cache.ContainsKey("link1"));
    }

    [Fact]
    public void GetLinks_ReturnsLinksFromAllCaches() {
        var controller = CreateController();
        var cache1 = DictionaryReferenceCache<string, int>.CreateNew();
        cache1.Add("link1", 1);
        var cache2 = DictionaryReferenceCache<string, int>.CreateNew();
        cache2.Add("link2", 1);

        var links = controller.GetLinks([cache1, cache2], 1);

        Assert.Equal(["link1", "link2"], links);
    }

    [Fact]
    public void GetLinks_UnknownReference_ReturnsEmpty() {
        var controller = CreateController();
        var cache = DictionaryReferenceCache<string, int>.CreateNew();

        Assert.Empty(controller.GetLinks([cache], 42));
    }

    [Fact]
    public void GetReferences_ReturnsReferencesFromAllCaches() {
        var controller = CreateController();
        var cache1 = DictionaryReferenceCache<string, int>.CreateNew();
        cache1.Add("link1", 1);
        var cache2 = DictionaryReferenceCache<string, int>.CreateNew();
        cache2.Add("link1", 2);

        var caches = new Dictionary<string, DictionaryReferenceCache<string, int>> {
            ["source1"] = cache1,
            ["source2"] = cache2,
        };

        Assert.Equal([1, 2], controller.GetReferences(caches, "link1"));
    }

    [Fact]
    public void GetReferences_UnknownLink_ReturnsEmpty() {
        var controller = CreateController();
        var cache = DictionaryReferenceCache<string, int>.CreateNew();
        var caches = new Dictionary<string, DictionaryReferenceCache<string, int>> { ["source"] = cache };

        Assert.Empty(controller.GetReferences(caches, "unknown"));
    }
}