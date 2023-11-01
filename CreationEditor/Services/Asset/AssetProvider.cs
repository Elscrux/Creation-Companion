using System.IO.Abstractions;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
namespace CreationEditor.Services.Asset;

public sealed class AssetProvider : IAssetProvider, ILifecycleTask {
    private readonly IFileSystem _fileSystem;
    private readonly IAssetReferenceController _assetReferenceController;
    private readonly IAssetTypeService _assetTypeService;
    private readonly IArchiveService _archiveService;
    private readonly IDataDirectoryService _dataDirectoryService;

    private readonly Dictionary<string, IAsset> _assetDirectories = new();

    public AssetProvider(
        IFileSystem fileSystem,
        IAssetReferenceController assetReferenceController,
        IAssetTypeService assetTypeService,
        IArchiveService archiveService,
        IDataDirectoryService dataDirectoryService) {
        _fileSystem = fileSystem;
        _assetReferenceController = assetReferenceController;
        _assetTypeService = assetTypeService;
        _archiveService = archiveService;
        _dataDirectoryService = dataDirectoryService;
    }

    public void PreStartup() {}

    public void PostStartupAsync(CancellationToken token) {
        // Force load all directories up front
        var dataDirectory = GetAssetContainer(_dataDirectoryService.Path, token);
        foreach (var _ in dataDirectory.GetAllChildren<IAsset, AssetDirectory>(directory => directory.Children)) {}
    }

    public void OnExit() {}

    public AssetDirectory GetAssetContainer(string directory, CancellationToken token) {
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

        var assetDirectory = new AssetDirectory(_fileSystem.DirectoryInfo.New(directory), _fileSystem, _dataDirectoryService, _assetReferenceController, _assetTypeService, _archiveService);
        _assetDirectories.Add(assetDirectory.Path, assetDirectory);
        return assetDirectory;
    }
}
