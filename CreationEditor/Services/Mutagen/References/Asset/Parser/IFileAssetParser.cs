using CreationEditor.Services.DataSource;
using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Assets;
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
    /// <param name="fileSystemLink">File system link to the file to parse</param>
    /// <returns>Enumerable of asset references</returns>
    IEnumerable<AssetQueryResult<DataRelativePath>> ParseFile(FileSystemLink fileSystemLink);
}
