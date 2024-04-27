namespace CreationEditor.Services.Asset;

public interface IAssetProvider {
    /// <summary>
    /// Get an asset directory of a directory.
    /// </summary>
    /// <param name="directory">Full path to a directory to get an asset container for</param>
    /// <param name="token">A CancellationToken can be used to prematurely exit the calculation.
    /// Note that this will return an unexpected null.</param>
    /// <returns>Asset directory of the specified path</returns>
    AssetDirectory GetAssetContainer(string directory, CancellationToken token = default);

    /// <summary>
    /// Get an asset file for a file path.
    /// </summary>
    /// <param name="filePath">Full path to a file to get an asset file for</param>
    /// <param name="token">A CancellationToken can be used to prematurely exit the calculation.
    /// Note that this will return an unexpected null.</param>
    /// <returns>Asset file of the specified path</returns>
    AssetFile? GetAssetFile(string filePath, CancellationToken token = default);

    /// <summary>
    /// Get a file stream for an asset file, if it is a loose file or part of an archive.
    /// </summary>
    /// <param name="assetFile">Asset file to get a file stream for</param>
    /// <param name="token">A CancellationToken can be used to prematurely exit the calculation.</param>
    /// <returns>FileStream of the file in the archive</returns>
    Stream? GetAssetFileStream(AssetFile assetFile, CancellationToken token = default);
}
