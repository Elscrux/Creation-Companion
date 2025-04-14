using System.IO.Abstractions;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class AssetTypeService : IAssetTypeService {
    private readonly ILogger _logger;
    private readonly IFileSystem _fileSystem;

    public IReadOnlyCollection<string> FileExtensions { get; }
    public IAssetTypeProvider Provider { get; }

    private readonly Dictionary<string, IAssetType> _assetTypesExtensions = new(DataRelativePath.PathComparer);

    private readonly Dictionary<string, IAssetType> _identifierAssetTypes;

    public AssetTypeService(
        ILogger logger,
        IFileSystem fileSystem,
        IAssetTypeProvider assetTypeProvider) {
        _logger = logger;
        _fileSystem = fileSystem;
        Provider = assetTypeProvider;

        _identifierAssetTypes = Provider.AssetTypeIdentifiers.ToDictionary(x => x.Value, x => x.Key);

        FileExtensions = Provider.AllAssetTypes
            .SelectMany(a => a.FileExtensions)
            .ToHashSet(DataRelativePath.PathComparer);

        foreach (var assetType in Provider.AllAssetTypes) {
            foreach (var extension in assetType.FileExtensions) {
                _assetTypesExtensions.TryAdd(extension, assetType);
            }
        }
    }

    public IAssetType? GetAssetType(string filePath) {
        var extension = _fileSystem.Path.GetExtension(filePath);
        if (extension.Length == 0) return null;

        // Temporary adjustments
        if (extension == ".xwm" && filePath.Contains("music", DataRelativePath.PathComparison)) return Provider.Music;
        if (filePath.Contains("interface", DataRelativePath.PathComparison)) return null;
        if (filePath.Contains("source\\scripts", DataRelativePath.PathComparison)) return null;

        _assetTypesExtensions.TryGetValue(extension, out var assetType);

        return assetType;
    }

    public IAssetLink? GetAssetLink(DataRelativePath filePath) {
        var assetType = GetAssetType(filePath.Path);
        if (assetType is null) return null;

        return GetAssetLink(filePath, assetType);
    }

    public IAssetLink? GetAssetLink(DataRelativePath filePath, IAssetType assetType) {
        var constructor = Provider.AssetTypeConstructor[assetType];
        try {
            return constructor(filePath);
        } catch (Exception e) {
            _logger.Here().Warning(e, "Failed to create asset link for {FilePath} with asset type {AssetType}", filePath, assetType);
            return null;
        }
    }

    public string GetAssetTypeIdentifier(IAssetType assetType) => Provider.AssetTypeIdentifiers[assetType];
    public IAssetType GetAssetTypeFromIdentifier(string identifier) => _identifierAssetTypes[identifier];
}
