using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache;

public interface IAssetReferenceCacheFactory {
    Task<AssetReferenceCache<IModGetter, IFormLinkGetter>> GetModCache(IModGetter mod);
    Task<AssetReferenceCache<string, string>> GetNifCache(string directoryPath);
    Task<AssetReferenceCache<string, string>> GetNifArchiveCache(string archiveFilePath);
}
