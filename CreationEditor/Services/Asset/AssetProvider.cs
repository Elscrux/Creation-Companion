using System.IO.Abstractions;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
namespace CreationEditor.Services.Asset;

public sealed class AssetProvider(
    IFileSystem fileSystem,
    IAssetReferenceController assetReferenceController,
    IAssetTypeService assetTypeService,
    IArchiveService archiveService,
    IDataDirectoryService dataDirectoryService)
    : IAssetProvider {

    private readonly Dictionary<string, IAsset> _assetDirectories = new();

    public AssetDirectory GetAssetContainer(string directory, CancellationToken token = default) {
        foreach (var (path, asset) in _assetDirectories) {
            if (token.IsCancellationRequested) return null!;

            if (!directory.StartsWith(path, AssetCompare.PathComparison)) continue;

            // Retrieve child or self from the children
            foreach (var child in asset.GetChildren<IAsset, AssetDirectory>(
                a => directory.StartsWith(a.Path, AssetCompare.PathComparison),
                a => a.Children,
                true)) {
                if (string.Equals(directory, child.Path, AssetCompare.PathComparison)) {
                    return child;
                }
            }
        }

        var assetDirectory = new AssetDirectory(fileSystem.DirectoryInfo.New(directory), fileSystem, dataDirectoryService, assetReferenceController, assetTypeService, archiveService);
        _assetDirectories.Add(assetDirectory.Path, assetDirectory);
        return assetDirectory;
    }
}
