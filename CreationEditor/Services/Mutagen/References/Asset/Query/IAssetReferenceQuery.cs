using CreationEditor.Services.Mutagen.References.Asset.Cache;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public interface IAssetReferenceQuery<TSource, TReference>
    where TSource : notnull
    where TReference : notnull {

    /// <summary>
    /// Name of the query.
    /// </summary>
    string QueryName { get; }

    /// <summary>
    /// Returns the name of a given source.
    /// </summary>
    /// <param name="source">Source to get the name of</param>
    /// <returns>Name of the given source</returns>
    string GetName(TSource source) => source.ToString() ?? string.Empty;

    /// <summary>
    /// Asset caches for the given query.
    /// </summary>
    IDictionary<TSource, AssetReferenceCache<TSource, TReference>> AssetCaches { get; }

    /// <summary>
    /// Attempts to convert a reference to a source.
    /// </summary>
    /// <param name="reference">Reference to convert</param>
    /// <returns>Source of the reference, or null if the reference couldn't be converted</returns>
    TSource? ReferenceToSource(TReference reference);

    /// <summary>
    /// Parses the given source into a list of asset references.
    /// </summary>
    /// <param name="source">Source to parse</param>
    /// <returns>Enumerable of asset references</returns>
    IEnumerable<AssetQueryResult<TReference>> ParseAssets(TSource source);
}
