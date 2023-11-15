using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Query;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public sealed class FileAssetParser(IAssetTypeService assetTypeService) : IFileAssetParser {
    public string Name => "File";

    public IEnumerable<AssetQueryResult<string>> ParseFile(string filePath) {
        var type = assetTypeService.GetAssetType(filePath);

        if (type is not null) {
            yield return new AssetQueryResult<string>(assetTypeService.GetAssetLink(filePath, type), filePath);
        }
    }
}
