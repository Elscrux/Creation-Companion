using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using NiflySharp;
using NiflySharp.Blocks;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class NifModificationService(
    ILogger logger)
    : IModelModificationService {

    public void RemapLinks(DataSourceFileLink fileLink, Func<string, string> replace) {
        if (!fileLink.Exists()) return;

        var fileStream = fileLink.ReadFileStream();
        if (fileStream == null) return;

        var nif = new NifFile(fileStream);

        var modifiedNif = false;

        foreach (var block in nif.Blocks) {
            switch (block) {
                case BSShaderTextureSet bsShaderTextureSet:
                    foreach (var texture in bsShaderTextureSet.Textures) {
                        var current = texture.Content;
                        var replacement = replace(current);
                        if (!string.Equals(current, replacement, DataRelativePath.PathComparison)) {
                            texture.Content = replacement;
                            modifiedNif = true;
                        }
                    }
                    break;
                case BSEffectShaderProperty bsEffectShaderProperty:
                    TryReplace(bsEffectShaderProperty.LightingTexture, x => bsEffectShaderProperty.LightingTexture = x);
                    TryReplace(bsEffectShaderProperty.NormalTexture, x => bsEffectShaderProperty.NormalTexture = x);
                    TryReplace(bsEffectShaderProperty.ReflectanceTexture, x => bsEffectShaderProperty.ReflectanceTexture = x);
                    TryReplace(bsEffectShaderProperty.SourceTexture, x => bsEffectShaderProperty.SourceTexture = x);
                    TryReplace(bsEffectShaderProperty.EmitGradientTexture, x => bsEffectShaderProperty.EmitGradientTexture = x);
                    TryReplace(bsEffectShaderProperty.EnvMapTexture, x => bsEffectShaderProperty.EnvMapTexture = x);
                    TryReplace(bsEffectShaderProperty.EnvMaskTexture, x => bsEffectShaderProperty.EnvMaskTexture = x);

                    void TryReplace(NiString4 niString, Func<NiString4, NiString4> update) {
                        var str = niString.Content;
                        if (replace(str) is {} replacement && !string.Equals(str, replacement, DataRelativePath.PathComparison)) {
                            update(new NiString4(replacement));
                            modifiedNif = true;
                        }
                    }

                    break;
            }
        }

        if (modifiedNif) {
            try {
                fileStream.Dispose();
                nif.Save(fileLink.FullPath);
                logger.Here().Information("Remapped texture paths in nif {File}", fileLink.FullPath);
            } catch (Exception e) {
                logger.Here().Error(e, "Couldn't save modified nif {File}: {Exception}", fileLink, e);
            }
        } else {
            fileStream.Dispose();
        }
    }
}
