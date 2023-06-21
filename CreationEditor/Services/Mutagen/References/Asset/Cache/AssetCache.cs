using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache;

public sealed class AssetCache<TOrigin, TReference>
    where TOrigin : notnull
    where TReference : notnull {
    public TOrigin Origin { get; }

    /// <summary>
    /// Asset for all asset types mapped to a list of their usages
    /// </summary>
    private readonly IReadOnlyDictionary<TOrigin, Query.AssetQuery<TOrigin, TReference>.AssetReferenceCache> _referenceCaches;

    public AssetCache(
        Query.AssetQuery<TOrigin, TReference> assetQuery,
        TOrigin origin) {
        Origin = origin;

        var assetReferenceCache = assetQuery.LoadAssets(origin);
        var assetReferenceCaches = new Dictionary<TOrigin, Query.AssetQuery<TOrigin, TReference>.AssetReferenceCache>();
        if (assetReferenceCache is not null) assetReferenceCaches.Add(origin, assetReferenceCache);
        _referenceCaches = assetReferenceCaches;
    }

    public IEnumerable<IAssetLinkGetter> GetAssets(IAssetType assetType) {
        foreach (var (_, cache) in _referenceCaches) {
            if (cache.Cache.TryGetValue(assetType, out var assetDictionary)) {
                foreach (var asset in assetDictionary.Keys) {
                    yield return asset;
                }
            }
        }
        
    }

    public bool HasAsset(IAssetLinkGetter asset) {
        // Check if any cache contains the assets
        foreach (var (_, cache) in _referenceCaches) {
            if (cache.Cache.TryGetValue(asset.Type, out var assetDictionary)
             && assetDictionary.ContainsKey(asset)) return true;
        }

        return false;
    }

    /// <summary>
    /// Retrieve references of this asset in this asset manager.
    /// If you have the asset type available, use this to reduce runtime.
    /// </summary>
    /// <param name="asset">Asset to retrieve references for</param>
    /// <returns>String representation of the entity referencing asset</returns>
    public IEnumerable<TReference> GetReferences(IAssetLinkGetter asset) {
        foreach (var (_, cache) in _referenceCaches) {
            // todo replace with TryGetValue when mutagen update is done
            var assetDictionary = cache.Cache.FirstOrDefault(t => t.Key.GetType() == asset.Type.GetType()).Value;
            if (assetDictionary is null) continue;

            // if (!cache.Cache.TryGetValue(asset.Type, out var assetDictionary)) continue;
            if (!assetDictionary.TryGetValue(asset, out var usages)) continue;

            foreach (var usage in usages) {
                yield return usage;
            }
        }
    }

    public bool RemoveReference(TOrigin origin, IAssetLinkGetter asset, TReference oldReference) {
        if (!TryGetReferences(origin, asset, out var references)) return false;

        var removed = references.Remove(oldReference);
        if (references.Count == 0) {
            _referenceCaches[origin].Cache[asset.Type].Remove(asset);
        }

        return removed;
    }

    public bool AddReference(TOrigin origin, IAssetLinkGetter asset, TReference newReference) {
        if (!TryGetReferences(origin, asset, out var references)) return false;

        return references.Add(newReference);
    }

    private bool TryGetReferences(TOrigin origin, IAssetLinkGetter asset, [MaybeNullWhen(false)] out HashSet<TReference> references) {
        if (!_referenceCaches.TryGetValue(origin, out var cache)) {
            references = null;
            return false;
        }

        // todo replace with TryGetValue when mutagen update is done
        var assetDictionary = cache.Cache.FirstOrDefault(t => t.Key.GetType() == asset.Type.GetType()).Value;
        references = assetDictionary.GetOrAdd(asset);

        return true;
    }

    public IEnumerable<IAssetLinkGetter> FindLinksToReference(TReference reference) {
        foreach (var cache in _referenceCaches.Values) {
            foreach (var assetDictionary in cache.Cache.Values) {
                foreach (var (assetLink, references) in assetDictionary) {
                    if (references.Contains(reference)) {
                        yield return assetLink;
                    }
                }
            }
        }
    }
}
