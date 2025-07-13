using System.IO.Abstractions;
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
    /// <param name="filePath">Path to the file to parse</param>
    /// <param name="fileSystem">File system to read the file from</param>
    /// <returns>Enumerable of asset references</returns>
    IEnumerable<TLink> ParseFile(string filePath, IFileSystem fileSystem);
}
