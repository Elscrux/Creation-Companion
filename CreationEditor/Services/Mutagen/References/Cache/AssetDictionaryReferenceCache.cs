using System.Collections.Concurrent;
using CreationEditor.Resources.Comparer;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Cache;

public class AssetDictionaryReferenceCache<TLink> : IDictionaryReferenceCache<AssetDictionaryReferenceCache<TLink>, TLink, DataRelativePath>
    where TLink : notnull {
    private readonly DictionaryReferenceCache<TLink, DataRelativePath> _cache =
        new(new ConcurrentDictionary<TLink, HashSet<DataRelativePath>>(), DataRelativePathComparer.Instance);

    public ConcurrentDictionary<TLink, HashSet<DataRelativePath>> Cache => _cache.Cache;

    public static AssetDictionaryReferenceCache<TLink> CreateNew() => new();

    public void Add(AssetDictionaryReferenceCache<TLink> otherCache) {
        _cache.Add(otherCache._cache);
    }

    public void Add(TLink link, DataRelativePath reference) {
        _cache.Add(link, reference);
    }
   
    public void Remove(IReadOnlyList<DataRelativePath> referencesToRemove) {
        _cache.Remove(referencesToRemove);
    }
}
