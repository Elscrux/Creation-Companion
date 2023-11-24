using CreationEditor.Services.Mutagen.References.Asset.Query;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public interface IFileAssetParser {
    /// <summary>
    /// Name of the parser.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// File extension of the files to parse.
    /// </summary>
    string FilterPattern => "*";

    /// <summary>
    /// Parses the given file into a list of asset references.
    /// </summary>
    /// <param name="filePath">Full path to the file to parse</param>
    /// <returns>Enumerable of asset references</returns>
    IEnumerable<AssetQueryResult<string>> ParseFile(string filePath);
}
