using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Asset.Cache;

public interface IAssetReferenceCacheFactory {
    /// <summary>
    /// Get a cache for the given mod.
    /// </summary>
    /// <param name="mod">Mod to get the asset cache for</param>
    /// <returns>Asset cache for the given mod</returns>
    Task<AssetReferenceCache<IModGetter, IFormLinkGetter>> GetModCache(IModGetter mod);

    /// <summary>
    /// Get a cache for nif files in the given directory.
    /// </summary>
    /// <param name="directoryPath">Full path to the directory to get the asset cache for</param>
    /// <returns>Asset cache for the given directory</returns>
    Task<AssetReferenceCache<string, DataRelativePath>> GetNifCache(string directoryPath);

    /// <summary>
    /// Get a cache for nif files in the given archive.
    /// </summary>
    /// <param name="archiveFilePath">Full path to the archive file to get the asset cache for</param>
    /// <returns>Asset cache for the given archive</returns>
    Task<AssetReferenceCache<string, DataRelativePath>> GetNifArchiveCache(string archiveFilePath);
}
