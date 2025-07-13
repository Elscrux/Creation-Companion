using CreationEditor.Services.Mutagen.References.Query;
namespace CreationEditor.Services.Mutagen.References.Cache.Serialization;

public interface IReferenceCacheSerialization<TSource, TCache, TLink, TReference>
    where TSource : notnull
    where TCache : IReferenceCache<TCache, TLink, TReference>
    where TLink : notnull
    where TReference : notnull {

    /// <summary>
    /// Validate a cache for the given source.
    /// </summary>
    /// <param name="source">Source to validate the cache for</param>
    /// <param name="query">Query to validate cache for</param>
    /// <returns>True if the cache is valid, false otherwise</returns>
    bool Validate(TSource source, IReferenceQuery<TSource, TCache, TLink, TReference> query);

    /// <summary>
    /// Deserialize a cache for the given source.
    /// </summary>
    /// <param name="source">Source to deserialize the cache for</param>
    /// <param name="query">Query to deserialize</param>
    /// <returns>Deserialized cache</returns>
    TCache Deserialize(TSource source, IReferenceQuery<TSource, TCache, TLink, TReference> query);

    /// <summary>
    /// Serialize a cache for the given source.
    /// </summary>
    /// <param name="source">Source to serialize the cache for</param>
    /// <param name="query">Query to serialize to cache</param>
    /// <param name="cache">Cache to write to</param>
    void Serialize(TSource source, IReferenceQuery<TSource, TCache, TLink, TReference> query, TCache cache);
}
