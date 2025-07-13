using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Cache;

public sealed class AssetReferenceCacheController<TSource, TReference> : IReferenceCacheController<TSource, AssetReferenceCache<TReference>, IAssetLinkGetter, TReference>
    where TSource : notnull
    where TReference : notnull {
    public void AddLink(AssetReferenceCache<TReference> cache, TReference reference, IEnumerable<IAssetLinkGetter> linksToAdd) {
        foreach (var link in linksToAdd) {
            cache.AddReference(link, reference);
        }
    }

    public void RemoveLink(AssetReferenceCache<TReference> cache, TReference reference, IEnumerable<IAssetLinkGetter> linksToRemove) {
        foreach (var link in linksToRemove) {
            cache.RemoveReference(link, reference);
        }
    }

    public IEnumerable<IAssetLinkGetter> GetLinks(IEnumerable<AssetReferenceCache<TReference>> caches, TReference reference) {
        foreach (var cache in caches) {
            foreach (var assetDictionary in cache.Cache.Values) {
                foreach (var (assetLink, references) in assetDictionary) {
                    bool contains;
                    lock (references) {
                        contains = references.Contains(reference);
                    }
                    if (contains) {
                        yield return assetLink;
                    }
                }
            }
        }
    }

    public IEnumerable<TReference> GetReferences(IReadOnlyDictionary<TSource, AssetReferenceCache<TReference>> caches, IAssetLinkGetter link) {
        return caches.Values.SelectMany(cache => cache.GetReferences(link));
    }
}
