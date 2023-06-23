namespace CreationEditor.Services.Asset;

public interface IAssetProvider {
    /// <summary>
    /// Get an asset directory of a directory.
    /// </summary>
    /// <param name="directory">Full path to a directory to get an asset container for</param>
    /// <returns>Asset directory of the specified path</returns>
    AssetDirectory GetAssetContainer(string directory);
}