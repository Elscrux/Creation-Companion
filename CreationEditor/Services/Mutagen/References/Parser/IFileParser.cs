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
    /// <param name="stream">Stream of the file content to parse, opened by the caller on the file's data source. <see langword="null"/> if the file is not available.</param>
    /// <param name="fileLink">Data source relative file link of the asset that is parsed</param>
    /// <returns>Enumerable of asset references</returns>
    IEnumerable<TLink> ParseFile(Stream? stream, DataSourceFileLink fileLink);
}
