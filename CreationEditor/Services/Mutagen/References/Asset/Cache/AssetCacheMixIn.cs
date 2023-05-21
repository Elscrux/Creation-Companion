using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache;

public static class AssetCacheMixIn {
    public static bool HasAssets<TOrigin, TReference>(this IEnumerable<AssetCache<TOrigin, TReference>> assetCaches, IAssetLinkGetter asset)
        where TOrigin : notnull
        where TReference : notnull {
        return assetCaches.Any(x => x.HasAsset(asset));
    }

    public static IEnumerable<IAssetLinkGetter> GetMissingAssets<TOrigin, TReference>(this IEnumerable<AssetCache<TOrigin, TReference>> assetCaches, IEnumerable<AssetCache<TOrigin, TReference>> otherAssetCaches, IEnumerable<IAssetType> assetTypes)
        where TOrigin : notnull
        where TReference : notnull {
        var otherAssetCachesList = otherAssetCaches.ToList();
        var assetTypeList = assetTypes.ToList();

        foreach (var assetCache in assetCaches) {
            foreach (var assetType in assetTypeList) {
                foreach (var requiredAsset in assetCache.GetAssets(assetType)) {
                    if (!otherAssetCachesList.HasAssets(requiredAsset)) {
                        yield return requiredAsset;
                    }
                }
            }
        }
    }

    public static IEnumerable<IAssetLinkGetter> GetAllAssets<TOrigin, TReference>(this IEnumerable<AssetCache<TOrigin, TReference>> assetCaches, IEnumerable<IAssetType> assetTypes)
        where TOrigin : notnull
        where TReference : notnull {
        var assetTypeList = assetTypes.ToList();
        foreach (var assetCache in assetCaches) {
            foreach (var assetType in assetTypeList) {
                foreach (var requiredAsset in assetCache.GetAssets(assetType)) {
                    yield return requiredAsset;
                }
            }
        }
    }

    public static IEnumerable<TReference> GetReferences<TOrigin, TReference>(this IEnumerable<AssetCache<TOrigin, TReference>> assetCaches, IAssetLinkGetter assetLink)
        where TOrigin : notnull
        where TReference : notnull {
        foreach (var assetCache in assetCaches) {
            foreach (var usage in assetCache.GetReferences(assetLink)) {
                yield return usage;
            }
        }
    }
}
