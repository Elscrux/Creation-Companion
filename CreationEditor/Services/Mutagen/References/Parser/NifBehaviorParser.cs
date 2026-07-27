using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using nifly;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class NifBehaviorParser(
    ILogger logger,
    IAssetTypeService assetTypeService)
    : IFileParser<IAssetLinkGetter> {

    public string Name => "Nif Behavior";
    public IAssetType AssetType => assetTypeService.Provider.Model;

    public IEnumerable<string> ParseFileTextureStrings(string filePath, DataSourceFileLink fileLink) {
        if (!fileLink.DataSource.FileSystem.File.Exists(filePath)) return [];

        var results = new HashSet<string>();
        try {
            using var nif = new NifFile();
            nif.Load(filePath);

            if (!nif.IsValid()) return results;

            using var niHeader = nif.GetHeader();
            using var blockCache = new niflycpp.BlockCache(niflycpp.BlockCache.SafeClone<NiHeader>(niHeader));
            for (uint blockId = 0; blockId < blockCache.Header.GetNumBlocks(); ++blockId) {
                using var bsBehaviorGraphExtraData = blockCache.EditableBlockById<BSBehaviorGraphExtraData>(blockId);
                if (bsBehaviorGraphExtraData is not null) {
                    if (bsBehaviorGraphExtraData.behaviorGraphFile is null) continue;

                    var assetString = bsBehaviorGraphExtraData.behaviorGraphFile.get();
                    if (string.IsNullOrEmpty(assetString)) continue;

                    results.Add(assetString);
                }
            }
        } catch (Exception e) {
            logger.Here().Error(e, "Error occurred while parsing nif file: {FilePath}", filePath);
        }

        return results;
    }

    public IEnumerable<IAssetLinkGetter> ParseFile(string actualFilePath, DataSourceFileLink fileLink) {
        foreach (var behaviorPath in ParseFileTextureStrings(actualFilePath, fileLink)) {
            var path = behaviorPath;
            if (!path.StartsWith(assetTypeService.Provider.Behavior.BaseFolder)) {
                path = fileLink.FileSystem.Path.Combine(assetTypeService.Provider.Behavior.BaseFolder, path);
            }

            DataRelativePath dataRelativePath;
            try {
                dataRelativePath = new DataRelativePath(path);
            } catch (AssetPathMisalignedException e) {
                logger.Here().Warning(e,
                    "Failed to parse asset path {AssetString} referenced in {Path}: {Exception}",
                    path,
                    actualFilePath,
                    e);
                continue;
            }

            var assetLink = assetTypeService.GetAssetLink(dataRelativePath);
            if (assetLink is null) continue;

            yield return assetLink;
        }
    }
}