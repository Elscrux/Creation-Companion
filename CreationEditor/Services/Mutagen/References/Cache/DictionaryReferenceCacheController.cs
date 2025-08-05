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
        foreach (var cache in caches) {
            foreach (var (link, references) in cache.Cache) {
                bool contains;
                lock (references) {
                    contains = references.Contains(reference);
                }

                if (!contains) continue;

                yield return link;
            }
        }
    }

    public IEnumerable<TReference> GetReferences(IReadOnlyDictionary<TSource, DictionaryReferenceCache<TLink, TReference>> caches, TLink link) {
        foreach (var (_, cache) in caches) {
            if (!cache.Cache.TryGetValue(link, out var references)) continue;

            foreach (var reference in references.ToArray()) {
                yield return reference;
            }
        }
    }
}
