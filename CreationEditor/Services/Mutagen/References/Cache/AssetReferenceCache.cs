using System.Collections.Concurrent;
using CreationEditor.Services.Asset;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Noggog;
namespace CreationEditor.Services.Mutagen.References.Cache;

public sealed class AssetReferenceCache<TReference> : IReferenceCache<AssetReferenceCache<TReference>, IAssetLinkGetter, TReference>
    where TReference : notnull {
    /// <summary>
    /// Asset for all asset types mapped to a list of their references
    /// </summary>
    public ConcurrentDictionary<IAssetType, ConcurrentDictionary<IAssetLinkGetter, HashSet<TReference>>> Cache { get; }

    internal AssetReferenceCache(
        ConcurrentDictionary<IAssetType, ConcurrentDictionary<IAssetLinkGetter, HashSet<TReference>>> cache) {
        Cache = cache;
    }

    public static AssetReferenceCache<TReference> CreateNew() {
        return new AssetReferenceCache<TReference>(
            new ConcurrentDictionary<IAssetType, ConcurrentDictionary<IAssetLinkGetter, HashSet<TReference>>>());
    }

    private static ConcurrentDictionary<IAssetLinkGetter, HashSet<TReference>> CreateNewEntry() => new(AssetLinkEqualityComparer.Instance);

    public void Add(AssetReferenceCache<TReference> otherCache) {
        foreach (var (assetType, assetReferences) in otherCache.Cache) {
            var typeCache = Cache.GetOrAdd(assetType, CreateNewEntry);
            foreach (var (assetLink, references) in assetReferences) {
                var typeReferences = typeCache.GetOrAdd(assetLink);
                typeReferences.UnionWith(references);
            }
        }
    }

    public void Add(IAssetLinkGetter link, TReference reference) {
        var typeCache = Cache.GetOrAdd(link.Type, CreateNewEntry);
        var references = typeCache.GetOrAdd(link);
        references.Add(reference);
    }

    public void Remove(IReadOnlyList<TReference> referencesToRemove) {
        foreach (var assetReferences in Cache.Values) {
            foreach (var (assetLink, references) in assetReferences) {
                references.ExceptWith(referencesToRemove);
                if (references.Count == 0) {
                    assetReferences.TryRemove(assetLink, out _);
                }
            }
        }
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

        if (!assetDictionary.TryGetValue(asset, out var references)) yield break;

        foreach (var usage in references.ToArray()) {
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
