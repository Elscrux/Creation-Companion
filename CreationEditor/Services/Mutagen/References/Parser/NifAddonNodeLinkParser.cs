using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using NiflySharp;
using NiflySharp.Blocks;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class NifAddonNodeLinkParser(
    IAssetTypeService assetTypeService,
    ILogger logger) : IFileParser<uint> {
    public string Name => "Nif Addon Nodes";
    public IAssetType AssetType => assetTypeService.Provider.Model;

    public IEnumerable<uint> ParseFile(Stream? stream, DataSourceFileLink fileLink) {
        var results = new HashSet<uint>();
        if (stream is null) return results;

        try {
            var nif = new NifFile(stream);

            foreach (var bsValueNode in nif.Blocks.OfType<BSValueNode>()) {
                results.Add(bsValueNode.Value);
            }
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to parse nif file {FilePath} for addon nodes", fileLink.FullPath);
        }

        return results;
    }
}
