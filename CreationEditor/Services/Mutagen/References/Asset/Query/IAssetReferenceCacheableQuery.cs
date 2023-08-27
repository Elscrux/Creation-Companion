using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public interface IAssetReferenceCacheableQuery<TSource, TReference> : IAssetReferenceQuery<TSource, TReference>
    where TSource : notnull
    where TReference : notnull {

    Version CacheVersion { get; }
    IAssetReferenceSerialization<TSource, TReference> Serialization { get; }

    bool IsCacheUpToDate(BinaryReader reader, TSource source);
    string ReadContextString(BinaryReader reader);
    IEnumerable<TReference> ReadUsages(BinaryReader reader, string contextString, int assetUsageCount);

    void WriteCacheCheck(BinaryWriter writer, TSource source);
    void WriteContext(BinaryWriter writer, TSource source);
    void WriteUsages(BinaryWriter writer, IEnumerable<TReference> usages);
}
