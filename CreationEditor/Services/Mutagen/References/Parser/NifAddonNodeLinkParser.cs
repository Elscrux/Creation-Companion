using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using nifly;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class NifAddonNodeLinkParser(
    IAssetTypeService assetTypeService,
    ILogger logger) : IFileParser<uint> {
    public string Name => "Nif Addon Nodes";
    public IAssetType AssetType => assetTypeService.Provider.Model;

    public IEnumerable<uint> ParseFile(string actualFilePath, DataSourceFileLink fileLink) {
        var results = new HashSet<uint>();
        if (!fileLink.FileSystem.File.Exists(actualFilePath)) return results;

        try {
            using var fileStream = fileLink.FileSystem.File.Open(actualFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            using var nif = new NifFile();
            nif.Load(actualFilePath);

            if (!nif.IsValid()) return results;

            var niHeader = nif.GetHeader();
            using var blockCache = new niflycpp.BlockCache(niflycpp.BlockCache.SafeClone<NiHeader>(niHeader));
            for (uint blockId = 0; blockId < blockCache.Header.GetNumBlocks(); blockId++) {
                using var bsValueNode = blockCache.EditableBlockById<BSValueNode>(blockId);
                if (bsValueNode is null) continue;

                results.Add((uint) bsValueNode.value);
            }
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to parse nif file {FilePath} for addon nodes", actualFilePath);
        }

        return results;
    }
}
