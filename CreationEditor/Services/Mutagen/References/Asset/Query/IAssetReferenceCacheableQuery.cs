using CreationEditor.Services.Mutagen.References.Asset.Cache.Serialization;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public interface IAssetReferenceCacheableQuery<TSource, TReference>
    : IAssetReferenceQuery<TSource, TReference>
    where TSource : notnull
    where TReference : notnull {

    /// <summary>
    /// Version of the cache.
    /// </summary>
    Version CacheVersion { get; }

    /// <summary>
    /// Serialization for the cache.
    /// </summary>
    IAssetReferenceSerialization<TSource, TReference> Serialization { get; }

    /// <summary>
    /// Check if the cache is up-to-date. Consumes the next step(s) of the BinaryReader.
    /// </summary>
    /// <param name="reader">BinaryReader to read the cache from as the next step</param>
    /// <param name="source">Source to check the cache for</param>
    /// <returns>True if the cache is up-to-date, false otherwise</returns>
    bool IsCacheUpToDate(BinaryReader reader, TSource source);

    /// <summary>
    /// Read the context string from the BinaryReader. Consumes the next step(s) of the BinaryReader.
    /// </summary>
    /// <param name="reader">BinaryReader to read the context string from as the next step</param>
    /// <returns>Context string</returns>
    string ReadContextString(BinaryReader reader);

    /// <summary>
    /// Read the usages from the BinaryReader. Consumes the next step(s) of the BinaryReader.
    /// </summary>
    /// <param name="reader">BinaryReader to read the usages from as the next step</param>
    /// <param name="contextString">Context string</param>
    /// <param name="assetReferenceCount">Number of references to read</param>
    /// <returns>References read from the cache</returns>
    IEnumerable<TReference> ReadReferences(BinaryReader reader, string contextString, int assetReferenceCount);

    /// <summary>
    /// Write the cache validation to the BinaryWriter.
    /// </summary>
    /// <param name="writer">BinaryWriter to write the cache validation to</param>
    /// <param name="source">Source to write the cache validation for</param>
    void WriteCacheValidation(BinaryWriter writer, TSource source);

    /// <summary>
    /// Write the context string to the BinaryWriter.
    /// </summary>
    /// <param name="writer">BinaryWriter to write the context string to</param>
    /// <param name="source">Source to write the context string for</param>
    void WriteContext(BinaryWriter writer, TSource source);

    /// <summary>
    /// Write the references to the BinaryWriter.
    /// </summary>
    /// <param name="writer">BinaryWriter to write the references to</param>
    /// <param name="references">References to write</param>
    void WriteReferences(BinaryWriter writer, IEnumerable<TReference> references);
}
