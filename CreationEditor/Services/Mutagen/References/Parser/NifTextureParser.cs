using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Exceptions;
using NiflySharp;
using NiflySharp.Blocks;
using Serilog;
namespace CreationEditor.Services.Mutagen.References.Parser;

public sealed class NifTextureParser(
    ILogger logger,
    IAssetTypeService assetTypeService)
    : IFileParser<IAssetLinkGetter> {

    public string Name => "Nif Textures";
    public IAssetType AssetType => assetTypeService.Provider.Model;

    public IEnumerable<string> ParseFileTextureStrings(Stream? stream, DataSourceFileLink fileLink) {
        var results = new HashSet<string>();
        if (stream is null) return results;

        try {
            var nif = new NifFile(stream);

            foreach (var block in nif.Blocks) {
                switch (block) {
                    case BSShaderTextureSet bsShaderTextureSet:
                        foreach (var texture in bsShaderTextureSet.Textures) {
                            TryAddAsset(texture);
                        }
                        break;
                    case BSEffectShaderProperty effectShader:
                        TryAddAsset(effectShader.GreyscaleTexture);
                        TryAddAsset(effectShader.LightingTexture);
                        TryAddAsset(effectShader.NormalTexture);
                        TryAddAsset(effectShader.ReflectanceTexture);
                        TryAddAsset(effectShader.SourceTexture);
                        TryAddAsset(effectShader.EmitGradientTexture);
                        TryAddAsset(effectShader.EnvMapTexture);
                        TryAddAsset(effectShader.EnvMaskTexture);
                        break;
                }

            }
        } catch (Exception e) {
            logger.Here().Error(e, "Error occurred while parsing nif file: {FilePath}", fileLink.FullPath);
        }

        return results;

        void TryAddAsset(NiString? asset) {
            if (asset is null) return;

            var assetString = asset.Content;
            if (string.IsNullOrEmpty(assetString)) return;

            results.Add(assetString);
        }
    }

    public IEnumerable<IAssetLinkGetter> ParseFile(Stream? stream, DataSourceFileLink fileLink) {
        foreach (var fileTextureString in ParseFileTextureStrings(stream, fileLink)) {
            DataRelativePath dataRelativePath;
            try {
                dataRelativePath = new DataRelativePath(fileTextureString);
            } catch (AssetPathMisalignedException e) {
                if (fileLink.FileSystem.Path.IsPathRooted(fileTextureString)
                 && fileTextureString.IndexOf($@"\{assetTypeService.Provider.Texture.BaseFolder}\", DataRelativePath.PathComparison) is var pathIndex and > 0) {
                    var relativePath = fileTextureString[(pathIndex + 1)..];
                    try {
                        dataRelativePath = new DataRelativePath(relativePath);
                    } catch (AssetPathMisalignedException innerException) {
                        logger.Here().Warning(innerException,
                            "Failed to parse asset path {AssetString} referenced in {Path}: {Exception}",
                            relativePath,
                            fileLink.FullPath,
                            innerException);
                        continue;
                    }
                } else {
                    logger.Here().Warning(e,
                        "Failed to parse asset path {AssetString} referenced in {Path}: {Exception}",
                        fileTextureString,
                        fileLink.FullPath,
                        e);
                    continue;
                }
            }

            // Normalize the path to start with the base folder of the texture asset type if it doesn't already
            if (!dataRelativePath.Path.StartsWith(assetTypeService.Provider.Texture.BaseFolder, DataRelativePath.PathComparison)) {
                dataRelativePath = fileLink.FileSystem.Path.Combine(assetTypeService.Provider.Texture.BaseFolder, dataRelativePath.Path);
            }

            var assetLink = assetTypeService.GetAssetLink(dataRelativePath);
            if (assetLink is null) continue;

            yield return assetLink;
        }
    }
}
