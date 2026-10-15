using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using NiflySharp;
using NiflySharp.Blocks;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class NifBehaviorParser(
    ILogger logger,
    IAssetTypeService assetTypeService)
    : IFileParser<IAssetLinkGetter> {

    public string Name => "Nif Behavior";
    public IAssetType AssetType => assetTypeService.Provider.Model;

    public IEnumerable<string> ParseFileTextureStrings(Stream? stream, DataSourceFileLink fileLink) {
        if (stream is null) return [];

        var results = new HashSet<string>();
        try {
            var nif = new NifFile(stream);

            foreach (var bsBehaviorGraphExtraData in nif.Blocks.OfType<BSBehaviorGraphExtraData>()) {
                if (bsBehaviorGraphExtraData.BehaviourGraphFile is null) continue;
                if (string.IsNullOrEmpty(bsBehaviorGraphExtraData.BehaviourGraphFile.String)) continue;

                results.Add(bsBehaviorGraphExtraData.BehaviourGraphFile.String);
            }
        } catch (Exception e) {
            logger.Here().Error(e, "Error occurred while parsing nif file: {FilePath}", fileLink.FullPath);
        }

        return results;
    }

    public IEnumerable<IAssetLinkGetter> ParseFile(Stream? stream, DataSourceFileLink fileLink) {
        foreach (var behaviorPath in ParseFileTextureStrings(stream, fileLink)) {
            var path = behaviorPath;
            if (!path.StartsWith(assetTypeService.Provider.Behavior.BaseFolder, DataRelativePath.PathComparison)) {
                path = fileLink.FileSystem.Path.Combine(assetTypeService.Provider.Behavior.BaseFolder, path);
            }

            DataRelativePath dataRelativePath;
            try {
                dataRelativePath = new DataRelativePath(path);
            } catch (AssetPathMisalignedException e) {
                logger.Here().Warning(e,
                    "Failed to parse asset path {AssetString} referenced in {Path}: {Exception}",
                    path,
                    fileLink.FullPath,
                    e);
                continue;
            }

            var assetLink = assetTypeService.GetAssetLink(dataRelativePath);
            if (assetLink is null) continue;

            yield return assetLink;
        }
    }
}
