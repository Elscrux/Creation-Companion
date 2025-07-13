namespace CreationEditor.Services.Mutagen.References.Cache;

public interface IReferenceCacheController<TSource, TCache, TLink, TReference> 
    where TSource : notnull
    where TCache : IReferenceCache<TCache, TLink, TReference>
    where TLink : notnull
    where TReference : notnull {
    void AddLink(TCache cache, TReference reference, IEnumerable<TLink> linksToAdd);
    void RemoveLink(TCache cache, TReference reference, IEnumerable<TLink> linksToRemove);
    IEnumerable<TLink> GetLinks(IEnumerable<TCache> caches, TReference reference);
    IEnumerable<TReference> GetReferences(IReadOnlyDictionary<TSource, TCache> caches, TLink link);
}
