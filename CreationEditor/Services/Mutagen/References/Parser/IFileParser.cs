using System.IO.Abstractions;
namespace CreationEditor.Services.Mutagen.References.Parser;

public interface IFileParser<out TLink> {
    /// <summary>
    /// Name of the parser.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// File extension of the files to parse.
    /// </summary>
    IEnumerable<string> FileExtensions { get; }

    /// <summary>
    /// Parses the given file into a list of asset references.
    /// </summary>
    /// <param name="filePath">Path to the file to parse</param>
    /// <param name="fileSystem">File system to read the file from</param>
    /// <returns>Enumerable of asset references</returns>
    IEnumerable<TLink> ParseFile(string filePath, IFileSystem fileSystem);
}
