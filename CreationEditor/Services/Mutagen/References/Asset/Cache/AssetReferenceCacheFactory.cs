using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache;

public sealed class AssetReferenceCacheFactory : IAssetReferenceCacheFactory {
    private readonly AssetReferenceCacheBuilder _assetReferenceCacheBuilder;
    private readonly ModAssetQuery _modAssetQuery;
    private readonly NifAssetQuery _nifAssetQuery;
    private readonly NifArchiveAssetQuery _nifArchiveAssetQuery;

    public AssetReferenceCacheFactory(
        AssetReferenceCacheBuilder assetReferenceCacheBuilder,
        ModAssetQuery modAssetQuery,
        NifAssetQuery nifAssetQuery,
        NifArchiveAssetQuery nifArchiveAssetQuery) {
        _assetReferenceCacheBuilder = assetReferenceCacheBuilder;
        _modAssetQuery = modAssetQuery;
        _nifAssetQuery = nifAssetQuery;
        _nifArchiveAssetQuery = nifArchiveAssetQuery;
    }

    public async Task<AssetReferenceCache<IModGetter, IFormLinkGetter>> GetModCache(IModGetter mod) {
        return await _assetReferenceCacheBuilder.BuildCache(_modAssetQuery, mod);
    }

    public async Task<AssetReferenceCache<string, string>> GetNifCache(string directoryPath) {
        return await _assetReferenceCacheBuilder.BuildCache(_nifAssetQuery, directoryPath);
    }

    public async Task<AssetReferenceCache<string, string>> GetNifArchiveCache(string archiveFilePath) {
        return await _assetReferenceCacheBuilder.BuildCache(_nifArchiveAssetQuery, archiveFilePath);
    }
}
