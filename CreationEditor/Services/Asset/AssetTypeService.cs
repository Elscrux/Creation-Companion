using System.IO.Abstractions;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Asset;

public sealed class AssetTypeService : IAssetTypeService {
    private readonly IFileSystem _fileSystem;

    public IReadOnlyList<string> FileExtensions { get; }
    public IAssetTypeProvider Provider { get; }

    private readonly Dictionary<string, IAssetType> _assetTypesExtensions = new(AssetCompare.PathComparer);

    private readonly Dictionary<string, IAssetType> _identifierAssetTypes;

    public AssetTypeService(
        IFileSystem fileSystem,
        IAssetTypeProvider assetTypeProvider) {
        _fileSystem = fileSystem;
        Provider = assetTypeProvider;

        _identifierAssetTypes = Provider.AssetTypeIdentifiers.ToDictionary(x => x.Value, x => x.Key);

        FileExtensions = Provider.AllAssetTypes
            .SelectMany(a => a.FileExtensions)
            .ToArray();

        foreach (var assetType in Provider.AllAssetTypes) {
            foreach (var extension in assetType.FileExtensions) {
                _assetTypesExtensions.TryAdd(extension, assetType);
            }
        }
    }

    public IAssetType? GetAssetType(string filePath) {
        var extension = _fileSystem.Path.GetExtension(filePath);
        if (extension.Length == 0) return null;

        _assetTypesExtensions.TryGetValue(extension, out var assetType);

        return assetType;
    }

    public IAssetLink? GetAssetLink(string filePath) {
        var assetType = GetAssetType(filePath);
        if (assetType is null) return null;

        return GetAssetLink(filePath, assetType);
    }

    public IAssetLink GetAssetLink(string filePath, IAssetType assetType) {
        var constructor = Provider.AssetTypeConstructor[assetType];
        return constructor(filePath);
    }

    // todo replace with direct directory access when everything uses the same instance
    public string GetAssetTypeIdentifier(IAssetType assetType) => Provider.AssetTypeIdentifiers.FirstOrDefault(pair => pair.Key.GetType() == assetType.GetType()).Value;
    public IAssetType GetAssetTypeFromIdentifier(string identifier) => _identifierAssetTypes[identifier];
}
