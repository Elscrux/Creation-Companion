using CreationEditor.Services.DataSource;
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

    public Task<AssetReferenceCache<FileSystemLink, DataRelativePath>> GetNifCache(FileSystemLink directoryLink) {
        return assetReferenceCacheBuilder.BuildCache(nifAssetQuery, directoryLink);
    }

    public Task<AssetReferenceCache<FileSystemLink, DataRelativePath>> GetNifArchiveCache(FileSystemLink archiveLink) {
        return assetReferenceCacheBuilder.BuildCache(nifArchiveAssetQuery, archiveLink);
    }
}
