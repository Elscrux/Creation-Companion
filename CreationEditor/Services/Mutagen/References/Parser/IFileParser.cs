using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Parser;

public interface IFileParser<out TLink> {
    /// <summary>
    /// Name of the parser.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Asset type that this parser is responsible for.
    /// </summary>
    IAssetType AssetType { get; }

    /// <summary>
    /// Parses the given file into a list of asset references.
    /// </summary>
    /// <param name="actualFilePath">Actual the file path that should be parsed</param>
    /// <param name="fileLink">Data source relative file link of the asset that is parsed</param>
    /// <returns>Enumerable of asset references</returns>
    IEnumerable<TLink> ParseFile(string actualFilePath, DataSourceFileLink fileLink);
}
