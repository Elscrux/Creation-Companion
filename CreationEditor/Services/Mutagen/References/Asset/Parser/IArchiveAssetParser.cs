using CreationEditor.Services.Mutagen.References.Asset.Query;
using Mutagen.Bethesda.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset.Parser;

public interface IArchiveAssetParser {
    /// <summary>
    /// Name of the parser.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Parses the given archive file into a list of asset references.
    /// </summary>
    /// <param name="archivePath">Full path to the archive file to parse</param>
    /// <returns>Enumerable of asset references</returns>
    IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(string archivePath);
}
