using CreationEditor.Services.Mutagen.References.Asset.Cache;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public interface IAssetReferenceQuery<TSource, TReference>
    where TSource : notnull
    where TReference : notnull {

    string QueryName { get; }
    IDictionary<TSource, AssetReferenceCache<TSource, TReference>> AssetCaches { get; }

    IEnumerable<AssetQueryResult<TReference>> ParseAssets(TSource source);
    string GetName(TSource source) => source.ToString() ?? string.Empty;
}
