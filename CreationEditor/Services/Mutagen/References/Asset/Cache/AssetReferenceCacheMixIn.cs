using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache;

public static class AssetReferenceCacheMixIn {
    public static bool HasAssets<TSource, TReference>(this IEnumerable<AssetReferenceCache<TSource, TReference>> assetCaches, IAssetLinkGetter asset)
        where TSource : notnull
        where TReference : notnull {
        return assetCaches.Any(x => x.HasAsset(asset));
    }

    public static IEnumerable<IAssetLinkGetter> GetMissingAssets<TSource, TReference>(this IEnumerable<AssetReferenceCache<TSource, TReference>> assetCaches, IEnumerable<AssetReferenceCache<TSource, TReference>> otherAssetCaches, IEnumerable<IAssetType> assetTypes)
        where TSource : notnull
        where TReference : notnull {
        var otherAssetCachesList = otherAssetCaches.ToList();
        var assetTypeList = assetTypes.ToList();

        foreach (var assetCache in assetCaches.ToArray()) {
            foreach (var assetType in assetTypeList) {
                foreach (var requiredAsset in assetCache.GetAssets(assetType)) {
                    if (!otherAssetCachesList.HasAssets(requiredAsset)) {
                        yield return requiredAsset;
                    }
                }
            }
        }
    }

    public static IEnumerable<IAssetLinkGetter> GetAllAssets<TSource, TReference>(this IEnumerable<AssetReferenceCache<TSource, TReference>> assetCaches, IEnumerable<IAssetType> assetTypes)
        where TSource : notnull
        where TReference : notnull {
        var assetTypeList = assetTypes.ToList();
        foreach (var assetCache in assetCaches.ToArray()) {
            foreach (var assetType in assetTypeList) {
                foreach (var requiredAsset in assetCache.GetAssets(assetType)) {
                    yield return requiredAsset;
                }
            }
        }
    }

    public static IEnumerable<TReference> GetReferences<TSource, TReference>(this IEnumerable<AssetReferenceCache<TSource, TReference>> assetCaches, IAssetLinkGetter assetLink)
        where TSource : notnull
        where TReference : notnull {
        foreach (var assetCache in assetCaches.ToArray()) {
            foreach (var usage in assetCache.GetReferences(assetLink)) {
                yield return usage;
            }
        }
    }
}
