using CreationEditor.Services.Mutagen.References.Cache;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim.Assets;
namespace CreationEditor.Skyrim.Tests.References.Cache;

public sealed class AssetReferenceCacheControllerTests {
    private static AssetReferenceCacheController<string, int> CreateController() {
        return new AssetReferenceCacheController<string, int>();
    }

    private static AssetLink<SkyrimModelAssetType> CreateAssetLink(string path) => new(path);

    [Fact]
    public void AddLink_AddsReferenceForEveryLink() {
        var controller = CreateController();
        var cache = AssetReferenceCache<int>.CreateNew();
        var link1 = CreateAssetLink("meshes/a.nif");
        var link2 = CreateAssetLink("meshes/b.nif");

        controller.AddLink(cache, 1, [link1, link2]);

        Assert.Contains(1, cache.GetReferences(link1));
        Assert.Contains(1, cache.GetReferences(link2));
    }

    [Fact]
    public void RemoveLink_RemovesReferenceFromLinks() {
        var controller = CreateController();
        var cache = AssetReferenceCache<int>.CreateNew();
        var link = CreateAssetLink("meshes/a.nif");
        controller.AddLink(cache, 1, [link]);

        controller.RemoveLink(cache, 1, [link]);

        Assert.DoesNotContain(1, cache.GetReferences(link));
    }

    [Fact]
    public void GetLinks_ReturnsLinksFromAllCaches() {
        var controller = CreateController();
        var cache1 = AssetReferenceCache<int>.CreateNew();
        cache1.AddReference(CreateAssetLink("meshes/a.nif"), 1);
        var cache2 = AssetReferenceCache<int>.CreateNew();
        cache2.AddReference(CreateAssetLink("meshes/b.nif"), 1);

        var links = controller.GetLinks([cache1, cache2], 1).ToList();

        Assert.Contains(links, l => l.DataRelativePath.Path == Path.Combine("meshes", "a.nif"));
        Assert.Contains(links, l => l.DataRelativePath.Path == Path.Combine("meshes", "b.nif"));
    }

    [Fact]
    public void GetLinks_UnknownReference_ReturnsEmpty() {
        var controller = CreateController();
        var cache = AssetReferenceCache<int>.CreateNew();

        Assert.Empty(controller.GetLinks([cache], 42));
    }

    [Fact]
    public void GetReferences_ReturnsReferencesFromAllCaches() {
        var controller = CreateController();
        var cache1 = AssetReferenceCache<int>.CreateNew();
        cache1.AddReference(CreateAssetLink("meshes/a.nif"), 1);
        var cache2 = AssetReferenceCache<int>.CreateNew();
        cache2.AddReference(CreateAssetLink("meshes/a.nif"), 2);

        var caches = new Dictionary<string, AssetReferenceCache<int>> {
            ["source1"] = cache1,
            ["source2"] = cache2,
        };

        Assert.Equal([1, 2], controller.GetReferences(caches, CreateAssetLink("meshes/a.nif")));
    }

    [Fact]
    public void GetReferences_UnknownLink_ReturnsEmpty() {
        var controller = CreateController();
        var cache = AssetReferenceCache<int>.CreateNew();
        var caches = new Dictionary<string, AssetReferenceCache<int>> { ["source"] = cache };

        Assert.Empty(controller.GetReferences(caches, CreateAssetLink("meshes/unknown.nif")));
    }
}
