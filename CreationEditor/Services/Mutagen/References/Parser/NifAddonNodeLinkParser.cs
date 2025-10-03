using System.IO.Abstractions;
using CreationEditor.Services.Asset;
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

    public IEnumerable<uint> ParseFile(string filePath, IFileSystem fileSystem) {
        var results = new HashSet<uint>();
        if (!fileSystem.File.Exists(filePath)) return results;

        try {
            using var fileStream = fileSystem.File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var nifFile = new NifFile(fileStream);
            foreach (var bsValueNode in nifFile.Blocks.OfType<BSValueNode>()) {
                results.Add(bsValueNode.Value);
            }
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to parse nif file {FilePath} for addon nodes", filePath);
        }

        return results;
    }
}
