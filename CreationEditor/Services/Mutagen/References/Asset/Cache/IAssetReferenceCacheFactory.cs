using CreationEditor.Services.DataSource;
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
    Task<AssetReferenceCache<IModGetter, IFormLinkIdentifier>> GetModCache(IModGetter mod);

    /// <summary>
    /// Get a cache for nif files in the given directory.
    /// </summary>
    /// <param name="directoryLink">File system link to the directory to get the asset cache for</param>
    /// <returns>Asset cache for the given directory</returns>
    Task<AssetReferenceCache<FileSystemLink, DataRelativePath>> GetNifCache(FileSystemLink directoryLink);

    /// <summary>
    /// Get a cache for nif files in the given archive.
    /// </summary>
    /// <param name="archiveLink">File system link to the archive to get the asset cache for</param>
    /// <returns>Asset cache for the given archive</returns>
    Task<AssetReferenceCache<FileSystemLink, DataRelativePath>> GetNifArchiveCache(FileSystemLink archiveLink);
}
