using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using nifly;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class NifAddonNodeLinkParser(IAssetTypeService assetTypeService) : IFileParser<int> {
    public string Name => "Nif Addon Nodes";
    public IEnumerable<string> FileExtensions => assetTypeService.Provider.Model.FileExtensions;

    public IEnumerable<int> ParseFile(string filePath, IFileSystem fileSystem) {
        if (assetTypeService.GetAssetType(filePath) != assetTypeService.Provider.Model) return [];

        var results = new HashSet<int>();

        if (!fileSystem.File.Exists(filePath)) return results;

        using var nif = new NifFile();
        nif.Load(filePath);

        if (!nif.IsValid()) return results;

        using var niHeader = nif.GetHeader();
        using var blockCache = new niflycpp.BlockCache(niflycpp.BlockCache.SafeClone<NiHeader>(niHeader));
        for (uint blockId = 0; blockId < blockCache.Header.GetNumBlocks(); ++blockId) {
            using var valueNode = blockCache.EditableBlockById<BSValueNode>(blockId);
            if (valueNode is not null) {
                results.Add(valueNode.value);
            }
        }

        return results;
    }
}
