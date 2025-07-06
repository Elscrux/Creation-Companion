using System.Collections.Concurrent;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache;

public sealed class AssetReferenceCache<TSource, TReference>
    where TSource : notnull
    where TReference : notnull {
    public TSource Source { get; }

    /// <summary>
    /// Asset for all asset types mapped to a list of their usages
    /// </summary>
    public ConcurrentDictionary<IAssetType, ConcurrentDictionary<IAssetLinkGetter, HashSet<TReference>>> Cache { get; }

    internal AssetReferenceCache(
        TSource source,
        ConcurrentDictionary<IAssetType, ConcurrentDictionary<IAssetLinkGetter, HashSet<TReference>>> cache) {
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

        if (!assetDictionary.TryGetValue(asset, out var usages)) yield break;

        foreach (var usage in usages.ToArray()) {
            yield return usage;
        }
    }

    public bool RemoveReference(IAssetLinkGetter asset, TReference oldReference) {
        if (!Cache.TryGetValue(asset.Type, out var assetDictionary)) return false;
        if (!assetDictionary.TryGetValue(asset, out var references)) return false;

        bool removed;
        lock (references) {
            removed = references.Remove(oldReference);
        }
        if (references.Count == 0) {
            Cache[asset.Type].TryRemove(asset, out _);
        }

        return removed;
    }

    public bool AddReference(IAssetLinkGetter asset, TReference newReference) {
        var assetDictionary = Cache.GetOrAdd(asset.Type, _ => new ConcurrentDictionary<IAssetLinkGetter, HashSet<TReference>>());
        var references = assetDictionary.GetOrAdd(asset, _ => []);
        lock (references) {
            return references.Add(newReference);
        }
    }

    public IEnumerable<IAssetLinkGetter> FindLinksToReference(TReference reference) {
        foreach (var assetDictionary in Cache.Values) {
            foreach (var (assetLink, references) in assetDictionary) {
                bool contains;
                lock (references) {
                    contains = references.Contains(reference);
                }
                if (contains) {
                    yield return assetLink;
                }
            }
        }
    }
}
