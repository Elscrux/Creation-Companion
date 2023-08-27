using CreationEditor.Services.Mutagen.References.Asset.Query;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public interface IArchiveAssetParser {
    string Name { get; }
    IEnumerable<AssetQueryResult<string>> ParseAssets(string archivePath);
}
