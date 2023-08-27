using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public sealed class NifFileAssetParser : IFileAssetParser {
    private readonly IFileSystem _fileSystem;
    private readonly ModelAssetQuery _modelAssetQuery;
    private readonly IAssetTypeService _assetTypeService;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;

    public string Name => "Nif";
    public string FilterPattern => "*.nif";

    public NifFileAssetParser(
        IFileSystem fileSystem,
        ModelAssetQuery modelAssetQuery,
        IAssetTypeService assetTypeService,
        IDataDirectoryProvider dataDirectoryProvider) {
        _fileSystem = fileSystem;
        _modelAssetQuery = modelAssetQuery;
        _assetTypeService = assetTypeService;
        _dataDirectoryProvider = dataDirectoryProvider;
    }

    public IEnumerable<AssetQueryResult<string>> ParseFile(string filePath) {
        var dataRelativePath = _fileSystem.Path.GetRelativePath(_dataDirectoryProvider.Path, filePath);

        var type = _assetTypeService.GetAssetType(filePath);
        if (type == _assetTypeService.Provider.Model) {
            foreach (var result in _modelAssetQuery.ParseAssets(filePath)) {
                yield return result with { Reference = dataRelativePath };
            }
        }
    }
}
