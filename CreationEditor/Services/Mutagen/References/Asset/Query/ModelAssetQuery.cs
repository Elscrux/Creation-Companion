using System.Collections.Concurrent;
using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using nifly;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class ModelAssetQuery(
    ILogger logger,
    IFileSystem fileSystem,
    IAssetTypeService assetTypeService)
    : IAssetReferenceQuery<string, DataRelativePath> {

    public string QueryName => "Model";
    public IDictionary<string, AssetReferenceCache<string, DataRelativePath>> AssetCaches { get; }
        = new ConcurrentDictionary<string, AssetReferenceCache<string, DataRelativePath>>();

    public string ReferenceToSource(DataRelativePath reference) => reference.Path;
    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(string source) => ParseAssetsInternal(source, source).Distinct();
    public IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssets(string source, DataRelativePath actualReference) =>
        ParseAssetsInternal(source, actualReference).Distinct();

    private IEnumerable<AssetQueryResult<DataRelativePath>> ParseAssetsInternal(string path, DataRelativePath actualReference) {
        if (!fileSystem.File.Exists(path)) yield break;

        using var nif = new NifFile();
        nif.Load(path);

        if (!nif.IsValid()) yield break;

        using var niHeader = nif.GetHeader();
        using var blockCache = new niflycpp.BlockCache(niflycpp.BlockCache.SafeClone<NiHeader>(niHeader));
        for (uint blockId = 0; blockId < blockCache.Header.GetNumBlocks(); ++blockId) {
            using var shaderTextureSet = blockCache.EditableBlockById<BSShaderTextureSet>(blockId);
            if (shaderTextureSet is not null) {
                using var vectorNiString = shaderTextureSet.textures.items();
                foreach (var niString in vectorNiString) {
                    foreach (var result in GetAssets(niString)) yield return result;
                }
            } else {
                var effectShader = blockCache.EditableBlockById<BSEffectShaderProperty>(blockId);
                if (effectShader is not null) {
                    foreach (var result in GetAssets(effectShader.greyscaleTexture)) yield return result;
                    foreach (var result in GetAssets(effectShader.lightingTexture)) yield return result;
                    foreach (var result in GetAssets(effectShader.normalTexture)) yield return result;
                    foreach (var result in GetAssets(effectShader.reflectanceTexture)) yield return result;
                    foreach (var result in GetAssets(effectShader.sourceTexture)) yield return result;
                    foreach (var result in GetAssets(effectShader.emitGradientTexture)) yield return result;
                    foreach (var result in GetAssets(effectShader.envMapTexture)) yield return result;
                    foreach (var result in GetAssets(effectShader.envMaskTexture)) yield return result;
                } else {
                    var shaderNoLighting = blockCache.EditableBlockById<BSShaderNoLightingProperty>(blockId);
                    if (shaderNoLighting is null) continue;

                    foreach (var result in GetAssets(shaderNoLighting.baseTexture)) yield return result;
                }
            }
        }

        IEnumerable<AssetQueryResult<DataRelativePath>> GetAssets(NiString? asset) {
            if (asset is null) yield break;

            var assetString = asset.get();
            if (!string.IsNullOrEmpty(assetString)) {
                DataRelativePath dataRelativePath;
                try {
                    dataRelativePath = assetString;
                } catch (AssetPathMisalignedException e) {
                    logger.Here().Warning(e,
                        "Failed to parse asset path {AssetString} referenced in {Path}: {Exception}",
                        assetString,
                        path,
                        e.Message);
                    yield break;
                }

                var assetLink = assetTypeService.GetAssetLink(dataRelativePath);
                if (assetLink is not null) {
                    yield return new AssetQueryResult<DataRelativePath>(assetLink, actualReference);
                }
            }
        }
    }
}
