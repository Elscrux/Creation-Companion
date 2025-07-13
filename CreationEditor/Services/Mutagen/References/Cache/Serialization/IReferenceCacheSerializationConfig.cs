namespace CreationEditor.Services.Mutagen.References.Cache.Serialization;

public interface IReferenceCacheSerializationConfig<TSource, TReference>
    where TSource : notnull
    where TReference : notnull {

    /// <summary>
    /// Version of the cache.
    /// </summary>
    Version CacheVersion { get; }

    /// <summary>
    /// Check if the cache is up-to-date. Consumes the next step(s) of the BinaryReader.
    /// </summary>
    /// <param name="reader">BinaryReader to read the cache from as the next step</param>
    /// <param name="source">Source to check the cache for</param>
    /// <returns>True if the cache is up-to-date, false otherwise</returns>
    bool IsCacheUpToDate(BinaryReader reader, TSource source);

    /// <summary>
    /// Read the source context string from the BinaryReader. Consumes the next step(s) of the BinaryReader.
    /// </summary>
    /// <param name="reader">BinaryReader to read the source context string from as the next step</param>
    /// <returns>Source context string</returns>
    string ReadSource(BinaryReader reader);

    /// <summary>
    /// Read the references from the BinaryReader. Consumes the next step(s) of the BinaryReader.
    /// </summary>
    /// <param name="reader">BinaryReader to read the references from as the next step</param>
    /// <param name="sourceContextString">Source context string</param>
    /// <param name="referenceCount">Number of references to read</param>
    /// <returns>References read from the cache</returns>
    IEnumerable<TReference> ReadReferences(BinaryReader reader, string sourceContextString, int referenceCount);

    /// <summary>
    /// Write the cache validation to the BinaryWriter.
    /// </summary>
    /// <param name="writer">BinaryWriter to write the cache validation to</param>
    /// <param name="source">Source to write the cache validation for</param>
    void WriteCacheValidation(BinaryWriter writer, TSource source);

    /// <summary>
    /// Write source context string to the BinaryWriter.
    /// </summary>
    /// <param name="writer">BinaryWriter to write the source context string to</param>
    /// <param name="source">Source to write the source context string for</param>
    void WriteSource(BinaryWriter writer, TSource source);

    /// <summary>
    /// Write the references to the BinaryWriter.
    /// </summary>
    /// <param name="writer">BinaryWriter to write the references to</param>
    /// <param name="references">References to write</param>
    void WriteReferences(BinaryWriter writer, IEnumerable<TReference> references);
}
