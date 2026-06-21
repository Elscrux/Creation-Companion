using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Cache;

public sealed class AssetDictionaryReferenceCacheController<TLink>
    : IReferenceCacheController<IDataSource, AssetDictionaryReferenceCache<TLink>, TLink, DataRelativePath>
    where TLink : notnull {
    public void AddLink(
        AssetDictionaryReferenceCache<TLink> cache,
        DataRelativePath reference,
        IEnumerable<TLink> linksToAdd) {
        foreach (var link in linksToAdd) {
            var references = cache.Cache.GetOrAdd(link, _ => []);
            lock (references) {
                references.Add(reference);
            }
        }
    }

    public void RemoveLink(
        AssetDictionaryReferenceCache<TLink> cache,
        DataRelativePath reference,
        IEnumerable<TLink> linksToRemove) {
        foreach (var link in linksToRemove) {
            if (!cache.Cache.TryGetValue(link, out var references)) continue;

            lock (references) {
                references.Remove(reference);
            }
        }
    }

    public IEnumerable<TLink> GetLinks(
        IEnumerable<AssetDictionaryReferenceCache<TLink>> caches,
        DataRelativePath reference) {
        return caches.SelectMany(cache => cache.ReverseCache.TryGetValue(reference, out var links) ? links : []).ToArray();
    }

    public IEnumerable<DataRelativePath> GetReferences(
        IReadOnlyDictionary<IDataSource, AssetDictionaryReferenceCache<TLink>> caches,
        TLink link) {
        return caches.Values.SelectMany(cache => cache.Cache.TryGetValue(link, out var references) ? references : []).ToArray();
    }
}
