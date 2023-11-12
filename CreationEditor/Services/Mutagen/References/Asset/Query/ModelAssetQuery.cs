using System.Collections.Concurrent;
using System.IO.Abstractions;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Mutagen.References.Asset.Cache;
using nifly;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class ModelAssetQuery : IAssetReferenceQuery<string, string> {
    private readonly IFileSystem _fileSystem;
    private readonly IAssetTypeService _assetTypeService;

    public string QueryName => "Model";
    public IDictionary<string, AssetReferenceCache<string, string>> AssetCaches { get; }
        = new ConcurrentDictionary<string, AssetReferenceCache<string, string>>();

    public ModelAssetQuery(
        IFileSystem fileSystem,
        IAssetTypeService assetTypeService) {
        _fileSystem = fileSystem;
        _assetTypeService = assetTypeService;
    }

    public IEnumerable<AssetQueryResult<string>> ParseAssets(string source) => ParseAssetsInternal(source).Distinct();

    private IEnumerable<AssetQueryResult<string>> ParseAssetsInternal(string path) {
        if (!_fileSystem.File.Exists(path)) yield break;

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

        IEnumerable<AssetQueryResult<string>> GetAssets(NiString? asset) {
            if (asset is null) yield break;

            var assetString = asset.get();
            if (!string.IsNullOrEmpty(assetString)) {
                var assetLink = _assetTypeService.GetAssetLink(assetString);
                if (assetLink is not null) {
                    yield return new AssetQueryResult<string>(assetLink, path);
                }
            }
        }
    }
}
