using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public sealed class FileAssetParser(IAssetTypeService assetTypeService) : IFileAssetParser {
    public string Name => "File";

    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseFile(FileSystemLink fileSystemLink) {
        var type = assetTypeService.GetAssetType(fileSystemLink.DataRelativePath.Path);
        if (type is null) yield break;

        var assetLink = assetTypeService.GetAssetLink(fileSystemLink.DataRelativePath, type);
        if (assetLink is null) yield break;

        yield return new AssetQueryResult<DataRelativePath>(assetLink, fileSystemLink.DataRelativePath);
    }
}
