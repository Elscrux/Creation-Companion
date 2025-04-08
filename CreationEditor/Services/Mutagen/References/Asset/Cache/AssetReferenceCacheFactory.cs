using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache;

public sealed class AssetReferenceCacheFactory(
    AssetReferenceCacheBuilder assetReferenceCacheBuilder,
    ModAssetQuery modAssetQuery,
    NifAssetQuery nifAssetQuery,
    NifArchiveAssetQuery nifArchiveAssetQuery)
    : IAssetReferenceCacheFactory {

    public Task<AssetReferenceCache<IModGetter, IFormLinkIdentifier>> GetModCache(IModGetter mod) {
        return assetReferenceCacheBuilder.BuildCache(modAssetQuery, mod);
    }

    public Task<AssetReferenceCache<string, DataRelativePath>> GetNifCache(string directoryPath) {
        return assetReferenceCacheBuilder.BuildCache(nifAssetQuery, directoryPath);
    }

    public Task<AssetReferenceCache<string, DataRelativePath>> GetNifArchiveCache(string archiveFilePath) {
        return assetReferenceCacheBuilder.BuildCache(nifArchiveAssetQuery, archiveFilePath);
    }
}
