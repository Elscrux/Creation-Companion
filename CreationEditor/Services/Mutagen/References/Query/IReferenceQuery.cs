using CreationEditor.Services.Mutagen.References.Cache;
namespace CreationEditor.Services.Mutagen.References.Query;

public interface IReferenceQuery<TSource, TCache, TLink, TReference>
    where TSource : notnull
    where TCache : IReferenceCache<TCache, TLink, TReference>
    where TLink : notnull
    where TReference : notnull {

    /// <summary>
    /// Name of the query.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns the name of a given source.
    /// </summary>
    /// <param name="source">Source to get the name of</param>
    /// <returns>Name of the given source</returns>
    string GetSourceName(TSource source);

    /// <summary>
    /// Parses the given source into the given cache.
    /// </summary>
    /// <param name="source">Source to parse</param>
    /// <param name="cache">Cache to fill with references</param>
    void FillCache(TSource source, TCache cache);

    /// <summary>
    /// Parses the given reference into the given cache.
    /// </summary>
    /// <param name="reference">Reference to parse</param>
    /// <param name="cache">Cache to fill with references</param>
    void FillCache(TReference reference, TCache cache);
}
