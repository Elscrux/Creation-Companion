using CreationEditor.Services.Mutagen.References.Asset.Query;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public interface IFileAssetParser {
    string Name { get; }
    string FilterPattern => "*";

    IEnumerable<AssetQueryResult<string>> ParseFile(string filePath);
}
