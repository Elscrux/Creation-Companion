using CreationEditor.Services.Mutagen.References.Asset.Query;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;

public interface IAssetReferenceSerialization<TSource, TReference>
    where TReference : notnull
    where TSource : notnull {

    bool Validate(TSource source, IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery);

    AssetReferenceCache<TSource, TReference> Deserialize(TSource source, IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery);
    void Serialize(TSource source, IAssetReferenceCacheableQuery<TSource, TReference> cacheableQuery, AssetReferenceCache<TSource, TReference> cache);
}
