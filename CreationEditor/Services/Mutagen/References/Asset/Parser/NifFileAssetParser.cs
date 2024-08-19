using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public sealed class NifFileAssetParser(
    IFileSystem fileSystem,
    ModelAssetQuery modelAssetQuery,
    IAssetTypeService assetTypeService,
    IDataDirectoryProvider dataDirectoryProvider)
    : IFileAssetParser {

    public string Name => "Nif";
    public string FilterPattern => "*.nif";

    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseFile(string filePath) {
        var dataRelativePath = fileSystem.Path.GetRelativePath(dataDirectoryProvider.Path, filePath);

        var type = assetTypeService.GetAssetType(dataRelativePath);
        if (type != assetTypeService.Provider.Model) yield break;

        foreach (var result in modelAssetQuery.ParseAssets(filePath, dataRelativePath)) {
            yield return result;
        }
    }
}
