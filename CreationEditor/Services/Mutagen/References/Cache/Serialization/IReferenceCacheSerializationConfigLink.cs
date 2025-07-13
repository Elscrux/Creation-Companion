namespace CreationEditor.Services.Mutagen.References.Cache.Serialization;

public interface IReferenceCacheSerializationConfigLink<TSource, TLink, TReference>
    : IReferenceCacheSerializationConfig<TSource, TReference>
    where TSource : notnull
    where TLink : notnull
    where TReference : notnull {
    /// <summary>
    /// Read the link from the BinaryReader. Consumes the next step(s) of the BinaryReader.
    /// </summary>
    /// <param name="reader">BinaryReader to read the link from as the next step</param>
    /// <returns>Link read from the cache</returns>
    TLink? ReadLink(BinaryReader reader);

    /// <summary>
    /// Write the link to the BinaryWriter.
    /// </summary>
    /// <param name="writer">BinaryWriter to write the link to</param>
    /// <param name="link">Link to write</param>
    void WriteLink(BinaryWriter writer, TLink link);
}
