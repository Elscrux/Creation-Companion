using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public sealed class NifFileAssetParser(
    ModelAssetQuery modelAssetQuery,
    IAssetTypeService assetTypeService)
    : IFileAssetParser {

    public string Name => "Nif";
    public string FilterPattern => "*.nif";

    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseFile(FileSystemLink fileSystemLink) {
        var type = assetTypeService.GetAssetType(fileSystemLink.DataRelativePath.Path);
        if (type != assetTypeService.Provider.Model) yield break;

        foreach (var result in modelAssetQuery.ParseAssets(fileSystemLink, fileSystemLink.DataRelativePath)) {
            yield return result;
        }
    }
}
