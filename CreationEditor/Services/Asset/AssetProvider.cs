using System.IO.Abstractions;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Mutagen.References.Asset.Controller;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class AssetProvider(
    ILogger logger,
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
            var dir = asset.GetChildren<IAsset, AssetDirectory>(
                    a => directory.StartsWith(a.Path, AssetCompare.PathComparison),
                    a => a.Children,
                    true)
                .FirstOrDefault(child => string.Equals(directory, child.Path, AssetCompare.PathComparison));

            if (dir is not null) return dir;
        }

        var assetDirectory = new AssetDirectory(fileSystem.DirectoryInfo.New(directory), logger, fileSystem, dataDirectoryService, assetReferenceController, assetTypeService, archiveService);
        _assetDirectories.Add(assetDirectory.Path, assetDirectory);
        return assetDirectory;
    }

    public AssetFile? GetAssetFile(string filePath, CancellationToken token = default) {
        var directoryName = fileSystem.Path.GetDirectoryName(filePath);
        if (directoryName is null) return null;

        var assetContainer = GetAssetContainer(directoryName, token);
        return assetContainer.GetAssetFile(filePath);
    }

    public Stream? GetAssetFileStream(AssetFile assetFile, CancellationToken token = default) {
        return assetFile.IsVirtual
            ? archiveService.TryGetFileStream(assetFile.Path)
            : fileSystem.File.OpenRead(assetFile.Path);
    }
}
