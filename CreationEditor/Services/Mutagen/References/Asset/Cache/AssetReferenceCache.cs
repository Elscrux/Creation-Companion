using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache;

public sealed class AssetReferenceCache<TSource, TReference>
    where TSource : notnull
    where TReference : notnull {
    public TSource Source { get; }

    /// <summary>
    /// Asset for all asset types mapped to a list of their usages
    /// </summary>
    public Dictionary<IAssetType, Dictionary<IAssetLinkGetter, HashSet<TReference>>> Cache { get; }

    internal AssetReferenceCache(
        TSource source,
        Dictionary<IAssetType, Dictionary<IAssetLinkGetter, HashSet<TReference>>> cache) {
        Source = source;
        Cache = cache;
    }

    public IEnumerable<IAssetLinkGetter> GetAssets(IAssetType assetType) {
        if (!Cache.TryGetValue(assetType, out var assetDictionary)) yield break;

        foreach (var asset in assetDictionary.Keys) {
            yield return asset;
        }
    }

    public bool HasAsset(IAssetLinkGetter asset) {
        // Check if the cache contains the assets
        return Cache.TryGetValue(asset.Type, out var assetDictionary)
         && assetDictionary.ContainsKey(asset);
    }

    /// <summary>
    /// Retrieve references of this asset in this asset manager.
    /// If you have the asset type available, use this to reduce runtime.
    /// </summary>
    /// <param name="asset">Asset to retrieve references for</param>
    /// <returns>String representation of the entity referencing asset</returns>
    public IEnumerable<TReference> GetReferences(IAssetLinkGetter asset) {
        if (!Cache.TryGetValue(asset.Type, out var assetDictionary)) yield break;

        // if (!cache.Cache.TryGetValue(asset.Type, out var assetDictionary)) continue;
        if (!assetDictionary.TryGetValue(asset, out var usages)) yield break;

        foreach (var usage in usages) {
            yield return usage;
        }
    }

    public bool RemoveReference(IAssetLinkGetter asset, TReference oldReference) {
        if (!TryGetReferences(asset, out var references)) return false;

        var removed = references.Remove(oldReference);
        if (references.Count == 0) {
            Cache[asset.Type].Remove(asset);
        }

        return removed;
    }

    public bool AddReference(IAssetLinkGetter asset, TReference newReference) {
        if (!TryGetReferences(asset, out var references)) return false;

        return references.Add(newReference);
    }

    private bool TryGetReferences(IAssetLinkGetter asset, [MaybeNullWhen(false)] out HashSet<TReference> references) {
        if (!Cache.TryGetValue(asset.Type, out var assetDictionary)) {
            references = null;
            return false;
        }

        references = assetDictionary.GetOrAdd(asset);

        return true;
    }

    public IEnumerable<IAssetLinkGetter> FindLinksToReference(TReference reference) {
        foreach (var assetDictionary in Cache.Values) {
            foreach (var (assetLink, references) in assetDictionary) {
                if (references.Contains(reference)) {
                    yield return assetLink;
                }
            }
        }
    }
}
