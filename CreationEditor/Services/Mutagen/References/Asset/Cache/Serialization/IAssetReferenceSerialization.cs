using CreationEditor.Services.Mutagen.References.Asset.Query;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;

public interface IAssetReferenceSerialization<TSource, TReference>
    where TReference : notnull
    where TSource : notnull {

    /// <summary>
    /// Validate a cache for the given source.
    /// </summary>
    /// <param name="source">Source to validate the cache for</param>
    /// <param name="cacheableQuery">Query to validate cache for</param>
    /// <returns>True if the cache is valid, false otherwise</returns>
    bool Validate(TSource source, IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery);

    /// <summary>
    /// Deserialize a cache for the given source.
    /// </summary>
    /// <param name="source">Source to deserialize the cache for</param>
    /// <param name="cacheableQuery">Query to deserialize</param>
    /// <returns>Deserialized cache</returns>
    AssetReferenceCache<TSource, TReference> Deserialize(TSource source, IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery);

    /// <summary>
    /// Serialize a cache for the given source.
    /// </summary>
    /// <param name="source">Source to serialize the cache for</param>
    /// <param name="cacheableQuery">Query to serialize to cache</param>
    /// <param name="cache">Cache to write to</param>
    void Serialize(TSource source, IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery, AssetReferenceCache<TSource, TReference> cache);
}
