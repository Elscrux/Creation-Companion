using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Asset;

public interface IAssetTypeService {
    IReadOnlyCollection<string> FileExtensions { get; }

    IAssetTypeProvider Provider { get; }

    /// <summary>
    /// Determines the asset type of a file path
    /// </summary>
    /// <param name="filePath">File path to determine asset type of</param>
    /// <returns>The singleton instance of the asset type, or null if no asset type could be found</returns>
    IAssetType? GetAssetType(string filePath);

    /// <summary>
    /// Builds an asset link for a file path
    /// </summary>
    /// <param name="filePath">File path to build the asset link for</param>
    /// <returns>Asset link of file path, or null if the asset type of the file path couldn't be determined</returns>
    IAssetLink? GetAssetLink(DataRelativePath filePath);

    /// <summary>
    /// Builds an asset link for a file path using a specific asset type
    /// </summary>
    /// <param name="filePath">File path to build the asset link for</param>
    /// <param name="assetType">Asset type of the file at the file path</param>
    /// <returns>Asset link of file path</returns>
    IAssetLink GetAssetLink(DataRelativePath filePath, IAssetType assetType);


    /// <summary>
    /// Gets an identifier for an asset type
    /// </summary>
    /// <param name="assetType">Asset type to get an identifier for</param>
    /// <returns>Three character string identifier of the asset type</returns>
    string GetAssetTypeIdentifier(IAssetType assetType);

    /// <summary>
    /// Gets an asset type from an identifier
    /// </summary>
    /// <param name="identifier">Three character string identifier</param>
    /// <returns>Asset type matching the identifier</returns>
    IAssetType GetAssetTypeFromIdentifier(string identifier);
}
