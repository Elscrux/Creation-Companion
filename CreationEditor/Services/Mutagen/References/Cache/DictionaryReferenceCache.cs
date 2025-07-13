using System.Collections.Concurrent;
namespace CreationEditor.Services.Mutagen.References.Cache;

public class DictionaryReferenceCache<TLink, TReference>(
    ConcurrentDictionary<TLink, HashSet<TReference>> cache,
    IEqualityComparer<TReference>? comparer)
    : IDictionaryReferenceCache<DictionaryReferenceCache<TLink, TReference>, TLink, TReference>
    where TLink : notnull {
    public ConcurrentDictionary<TLink, HashSet<TReference>> Cache { get; } = cache;

    public static DictionaryReferenceCache<TLink, TReference> CreateNew() {
        return new DictionaryReferenceCache<TLink, TReference>(new ConcurrentDictionary<TLink, HashSet<TReference>>(), null);
    }

    private HashSet<TReference> CreateNewEntry(TLink link) {
        return new HashSet<TReference>(comparer);
    }

    public void Add(DictionaryReferenceCache<TLink, TReference> otherCache) {
        foreach (var (link, references) in otherCache.Cache) {
            var typeReferences = Cache.GetOrAdd(link, CreateNewEntry);
            typeReferences.UnionWith(references);
        }
    }

    public void Add(TLink link, TReference reference) {
        var references = Cache.GetOrAdd(link, CreateNewEntry);
        references.Add(reference);
    }

    public void Remove(IReadOnlyList<TReference> referencesToRemove) {
        foreach (var references in Cache.Values) {
            references.ExceptWith(referencesToRemove);
        }
    }
}
