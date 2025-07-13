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
    string GetSourceName(TSource source) => source.ToString() ?? string.Empty;

    /// <summary>
    /// Attempts to convert a reference to a source.
    /// </summary>
    /// <param name="reference">Reference to convert</param>
    /// <returns>Source of the reference, or null if the reference couldn't be converted</returns>
    TSource? ReferenceToSource(TReference reference);

    /// <summary>
    /// Parses the given source into the given cache.
    /// </summary>
    /// <param name="source">Source to parse</param>
    /// <param name="cache">Cache to fill with references</param>
    void FillCache(TSource source, TCache cache);
}
