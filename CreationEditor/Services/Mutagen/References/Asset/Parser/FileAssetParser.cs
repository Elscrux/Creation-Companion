using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public sealed class FileAssetParser(
    IAssetTypeService assetTypeService,
    ILogger logger) : IFileAssetParser {
    public string Name => "File";

    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseFile(string filePath) {
        var type = assetTypeService.GetAssetType(filePath);
        if (type is null) yield break;

        DataRelativePath dataRelativePath;
        try {
            dataRelativePath = filePath;
        } catch (AssetPathMisalignedException e) {
            logger.Here().Warning(e,
                "Invalid asset path {Path} referenced in {Source}: {Exception}",
                filePath,
                filePath,
                e.Message);
            yield break;
        }

        yield return new AssetQueryResult<DataRelativePath>(assetTypeService.GetAssetLink(dataRelativePath, type),
            dataRelativePath);
    }
}
