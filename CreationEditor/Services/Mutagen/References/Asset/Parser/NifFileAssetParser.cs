using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Query;
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

    public IEnumerable<AssetQueryResult<string>> ParseFile(string filePath) {
        var dataRelativePath = fileSystem.Path.GetRelativePath(dataDirectoryProvider.Path, filePath);

        var type = assetTypeService.GetAssetType(filePath);
        if (type == assetTypeService.Provider.Model) {
            foreach (var result in modelAssetQuery.ParseAssets(filePath)) {
                yield return result with { Reference = dataRelativePath };
            }
        }
    }
}
