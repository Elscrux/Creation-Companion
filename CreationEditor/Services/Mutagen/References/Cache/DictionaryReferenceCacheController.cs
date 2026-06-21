namespace CreationEditor.Services.Mutagen.References.Cache;

public sealed class DictionaryReferenceCacheController<TSource, TLink, TReference>
    : IReferenceCacheController<TSource, DictionaryReferenceCache<TLink, TReference>, TLink, TReference>
    where TSource : notnull 
    where TLink : notnull
    where TReference : notnull {
    public void AddLink(DictionaryReferenceCache<TLink, TReference> cache, TReference reference, IEnumerable<TLink> linksToAdd) {
        foreach (var link in linksToAdd) {
            cache.Add(link, reference);
        }
    }

    public void RemoveLink(DictionaryReferenceCache<TLink, TReference> cache, TReference reference, IEnumerable<TLink> linksToRemove) {
        foreach (var link in linksToRemove) {
            cache.Cache.TryRemove(link, out _);
        }
    }

    public IEnumerable<TLink> GetLinks(IEnumerable<DictionaryReferenceCache<TLink, TReference>> caches, TReference reference) {
        return caches.SelectMany(cache => cache.ReverseCache.TryGetValue(reference, out var links) ? links : []).ToArray();
    }

    public IEnumerable<TReference> GetReferences(IReadOnlyDictionary<TSource, DictionaryReferenceCache<TLink, TReference>> caches, TLink link) {
        return caches.Values.SelectMany(cache => cache.Cache.TryGetValue(link, out var references) ? references : []).ToArray();
    }
}
