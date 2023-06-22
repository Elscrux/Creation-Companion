using System.IO.Abstractions;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
namespace CreationEditor.Services.Asset;

public sealed class AssetProvider : IAssetProvider {
    private readonly IFileSystem _fileSystem;
    private readonly IAssetReferenceController _assetReferenceController;
    private readonly IDeleteDirectoryProvider _deleteDirectoryProvider;
    private readonly IAssetTypeService _assetTypeService;
    private readonly IArchiveService _archiveService;

    private readonly Dictionary<string, IAsset> _assetDirectories = new();

    public AssetProvider(
        IFileSystem fileSystem,
        IAssetReferenceController assetReferenceController,
        IDeleteDirectoryProvider deleteDirectoryProvider,
        IAssetTypeService assetTypeService,
        IArchiveService archiveService) {
        _fileSystem = fileSystem;
        _assetReferenceController = assetReferenceController;
        _deleteDirectoryProvider = deleteDirectoryProvider;
        _assetTypeService = assetTypeService;
        _archiveService = archiveService;
    }

    public IAsset GetAssetContainer(string directory) {
        foreach (var (path, asset) in _assetDirectories) {
            if (!directory.StartsWith(path, AssetCompare.PathComparison)) continue;

            // Retrieve child or self from the children 
            foreach (var child in asset.GetChildren(
                a => directory.StartsWith(a.Path, AssetCompare.PathComparison),
                a => a.Children,
                true)) {
                if (string.Equals(directory, child.Path, AssetCompare.PathComparison)) {
                    return child;
                }
            }
        }

        var assetDirectory = new AssetDirectory(_fileSystem.DirectoryInfo.New(directory), _fileSystem, _deleteDirectoryProvider, _assetReferenceController, _assetTypeService, _archiveService);
        _assetDirectories.Add(assetDirectory.Path, assetDirectory);
        return assetDirectory;
    }
}