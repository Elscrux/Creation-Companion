using Autofac;
using nifly;
namespace CreationEditor.Services.Mutagen.References.Asset.Query;

public sealed class ModelAssetQuery : AssetQuery<string, string> {
    protected override bool CacheAssets => false;
    protected override string QueryName => "Model";

    public ModelAssetQuery(ILifetimeScope lifetimeScope) : base(lifetimeScope) {}

    public override IEnumerable<AssetQueryResult<string>> ParseAssets(string directory) => ParseAssetsInternal(directory).Distinct();

    private IEnumerable<AssetQueryResult<string>> ParseAssetsInternal(string path) {
        if (!FileSystem.File.Exists(path)) yield break;

        using var nif = new NifFile();
        nif.Load(path);

        if (!nif.IsValid()) yield break;

        using var niHeader = nif.GetHeader();
        using var blockCache = new niflycpp.BlockCache(niflycpp.BlockCache.SafeClone<NiHeader>(niHeader));
        for (uint blockId = 0; blockId < blockCache.Header.GetNumBlocks(); ++blockId) {
            using var shaderTextureSet = blockCache.EditableBlockById<BSShaderTextureSet>(blockId);
            if (shaderTextureSet != null) {
                using var vectorNiString = shaderTextureSet.textures.items();
                foreach (var niString in vectorNiString) {
                    foreach (var result in GetAssets(niString)) yield return result;
                }
            } else {
                var effectShader = blockCache.EditableBlockById<BSEffectShaderProperty>(blockId);
                if (effectShader != null) {
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
                    if (shaderNoLighting == null) continue;

                    foreach (var result in GetAssets(shaderNoLighting.baseTexture)) yield return result;
                }
            }
        }

        IEnumerable<AssetQueryResult<string>> GetAssets(NiString? asset) {
            if (asset == null) yield break;

            var assetString = asset.get();
            if (!string.IsNullOrEmpty(assetString)) {
                var assetLink = AssetTypeService.GetAssetLink(assetString);
                if (assetLink != null) {
                    yield return new AssetQueryResult<string>(assetLink, path);
                }
            }
        }
    }

    protected override void WriteCacheCheck(BinaryWriter writer, string origin) => writer.Write(origin);

    protected override void WriteContext(BinaryWriter writer, string origin) => writer.Write(origin);

    protected override void WriteUsages(BinaryWriter writer, IEnumerable<string> usages) {
        foreach (var usage in usages) {
            writer.Write(usage);
        }
    }

    protected override bool IsCacheUpToDate(BinaryReader reader, string origin) => true;

    protected override string ReadContextString(BinaryReader reader) => reader.ReadString();

    protected override IEnumerable<string> ReadUsages(BinaryReader reader, string context, int count) {
        for (var i = 0; i < count; i++) {
            yield return reader.ReadString();
        }
    }
}
