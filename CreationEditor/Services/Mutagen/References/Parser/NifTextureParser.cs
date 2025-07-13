using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using nifly;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class NifTextureParser(
    ILogger logger,
    IAssetTypeService assetTypeService)
    : IFileParser<IAssetLinkGetter> {

    public string Name => "Nif Textures";
    public IAssetType AssetType => assetTypeService.Provider.Model;

    public IEnumerable<IAssetLinkGetter> ParseFile(string filePath, IFileSystem fileSystem) {
        var results = new HashSet<IAssetLinkGetter>();
        if (!fileSystem.File.Exists(filePath)) return results;

        using var nif = new NifFile();
        nif.Load(filePath);

        if (!nif.IsValid()) return results;

        using var niHeader = nif.GetHeader();
        using var blockCache = new niflycpp.BlockCache(niflycpp.BlockCache.SafeClone<NiHeader>(niHeader));
        for (uint blockId = 0; blockId < blockCache.Header.GetNumBlocks(); ++blockId) {
            using var shaderTextureSet = blockCache.EditableBlockById<BSShaderTextureSet>(blockId);
            if (shaderTextureSet is not null) {
                using var vectorNiString = shaderTextureSet.textures.items();
                foreach (var niString in vectorNiString) {
                    TryAddAsset(niString);
                }
            } else {
                var effectShader = blockCache.EditableBlockById<BSEffectShaderProperty>(blockId);
                if (effectShader is not null) {
                    TryAddAsset(effectShader.greyscaleTexture);
                    TryAddAsset(effectShader.lightingTexture);
                    TryAddAsset(effectShader.normalTexture);
                    TryAddAsset(effectShader.reflectanceTexture);
                    TryAddAsset(effectShader.sourceTexture);
                    TryAddAsset(effectShader.emitGradientTexture);
                    TryAddAsset(effectShader.envMapTexture);
                    TryAddAsset(effectShader.envMaskTexture);
                } else {
                    var shaderNoLighting = blockCache.EditableBlockById<BSShaderNoLightingProperty>(blockId);
                    if (shaderNoLighting is null) continue;

                    TryAddAsset(shaderNoLighting.baseTexture);
                }
            }
        }

        return results;

        void TryAddAsset(NiString? asset) {
            if (asset is null) return;

            var assetString = asset.get();
            if (string.IsNullOrEmpty(assetString)) return;

            DataRelativePath dataRelativePath;
            try {
                dataRelativePath = new DataRelativePath(assetString);
            } catch (AssetPathMisalignedException e) {
                logger.Here().Warning(e,
                    "Failed to parse asset path {AssetString} referenced in {Path}: {Exception}",
                    assetString,
                    filePath,
                    e.Message);
                return;
            }

            var assetLink = assetTypeService.GetAssetLink(dataRelativePath);
            if (assetLink is null) return;

            results.Add(assetLink);
        }
    }
}
