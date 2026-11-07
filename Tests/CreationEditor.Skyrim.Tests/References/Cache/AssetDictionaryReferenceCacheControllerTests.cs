using System.IO.Abstractions;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Skyrim.Tests.References.Cache;

public sealed class AssetDictionaryReferenceCacheControllerTests {
    private static AssetDictionaryReferenceCacheController<string> CreateController() {
        return new AssetDictionaryReferenceCacheController<string>();
    }

    private static DataRelativePath CreatePath(string path) => new(path);

    private static IDataSource CreateDataSource(IFileSystem fileSystem, string path) {
        return new FileSystemDataSource(fileSystem, path);
    }

    [Fact]
    public void AddLink_AddsReferenceForEveryLink() {
        var controller = CreateController();
        var cache = AssetDictionaryReferenceCache<string>.CreateNew();
        var reference = CreatePath("meshes/test.nif");

        controller.AddLink(cache, reference, ["link1", "link2"]);

        Assert.Contains(reference, cache.Cache["link1"]);
        Assert.Contains(reference, cache.Cache["link2"]);
    }

    [Fact]
    public void RemoveLink_RemovesReferenceFromLinks() {
        var controller = CreateController();
        var cache = AssetDictionaryReferenceCache<string>.CreateNew();
        var reference = CreatePath("meshes/test.nif");
        controller.AddLink(cache, reference, ["link1"]);

        controller.RemoveLink(cache, reference, ["link1"]);

        Assert.DoesNotContain(reference, cache.Cache["link1"]);
    }

    [Fact]
    public void GetLinks_ReturnsLinksFromAllCaches() {
        var controller = CreateController();
        var cache1 = AssetDictionaryReferenceCache<string>.CreateNew();
        cache1.Add("link1", CreatePath("meshes/test.nif"));
        var cache2 = AssetDictionaryReferenceCache<string>.CreateNew();
        cache2.Add("link2", CreatePath("meshes/test.nif"));

        var links = controller.GetLinks([cache1, cache2], CreatePath("meshes/test.nif"));

        Assert.Equal(["link1", "link2"], links);
    }

    [Fact]
    public void GetLinks_UnknownReference_ReturnsEmpty() {
        var controller = CreateController();
        var cache = AssetDictionaryReferenceCache<string>.CreateNew();

        Assert.Empty(controller.GetLinks([cache], CreatePath("meshes/unknown.nif")));
    }

    [Theory] [CreationEditorAutoData]
    public void GetReferences_ReturnsReferencesFromAllCaches(IFileSystem fileSystem) {
        var controller = CreateController();
        var cache1 = AssetDictionaryReferenceCache<string>.CreateNew();
        cache1.Add("link1", CreatePath("meshes/a.nif"));
        var cache2 = AssetDictionaryReferenceCache<string>.CreateNew();
        cache2.Add("link1", CreatePath("meshes/b.nif"));

        var caches = new Dictionary<IDataSource, AssetDictionaryReferenceCache<string>> {
            [CreateDataSource(fileSystem, "C:\\Source1")] = cache1,
            [CreateDataSource(fileSystem, "C:\\Source2")] = cache2,
        };

        var references = controller.GetReferences(caches, "link1").ToList();

        Assert.Contains(references, r => r.Path == Path.Combine("meshes", "a.nif"));
        Assert.Contains(references, r => r.Path == Path.Combine("meshes", "b.nif"));
    }

    [Theory] [CreationEditorAutoData]
    public void GetReferences_UnknownLink_ReturnsEmpty(IFileSystem fileSystem) {
        var controller = CreateController();
        var cache = AssetDictionaryReferenceCache<string>.CreateNew();
        var caches = new Dictionary<IDataSource, AssetDictionaryReferenceCache<string>> {
            [CreateDataSource(fileSystem, "C:\\Source1")] = cache,
        };

        Assert.Empty(controller.GetReferences(caches, "unknown"));
    }
}
