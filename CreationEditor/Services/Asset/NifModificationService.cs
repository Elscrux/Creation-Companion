using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using nifly;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class NifModificationService(
    ILogger logger)
    : IModelModificationService {

    public void RemapLinks(DataSourceFileLink fileLink, Func<string, string> replace) {
        if (!fileLink.Exists()) return;

        using var nif = new NifFile();
        nif.Load(fileLink.FullPath);

        if (!nif.IsValid()) return;

        var modifiedNif = false;

        var niHeader = nif.GetHeader();
        var blockCache = new niflycpp.BlockCache(niflycpp.BlockCache.SafeClone<NiHeader>(niHeader));
        for (uint blockId = 0; blockId < blockCache.Header.GetNumBlocks(); ++blockId) {
            // Remap BS Shader Texture Set
            using var shaderTextureSet = blockCache.EditableBlockById<BSShaderTextureSet>(blockId);
            if (shaderTextureSet is not null) {
                // Find all indices where the old path is used
                var items = shaderTextureSet.textures.items();
                var occurenceIndices = new Dictionary<int, string?>();
                for (var index = 0; index < items.Count; index++) {
                    var niString = items[index];
                    if (niString is null) continue;

                    var current = niString.get();
                    var replacement = replace(current);
                    if (!string.Equals(current, replacement, DataRelativePath.PathComparison)) {
                        occurenceIndices.Add(index, replacement);
                    }
                }

                if (occurenceIndices.Count <= 0) continue;

                // Replace all occurrences with the new path
                foreach (var (occurenceIndex, replacement) in occurenceIndices) {
                    items[occurenceIndex] = new NiString(replacement);
                }

                using var textureWrapper = new NiStringVector();
                textureWrapper.SetItems(items);
                shaderTextureSet.textures = textureWrapper;
                niHeader.ReplaceBlock(blockId, shaderTextureSet);

                modifiedNif = true;
            } else {
                // Remap BS Effect Shader Property
                var effectShader = blockCache.EditableBlockById<BSEffectShaderProperty>(blockId);

                if (effectShader is not null) {
                    var modifiedEffectShader = false;

                    TryReplace(effectShader.lightingTexture, x => effectShader.lightingTexture = x);
                    TryReplace(effectShader.normalTexture, x => effectShader.normalTexture = x);
                    TryReplace(effectShader.reflectanceTexture, x => effectShader.reflectanceTexture = x);
                    TryReplace(effectShader.sourceTexture, x => effectShader.sourceTexture = x);
                    TryReplace(effectShader.emitGradientTexture, x => effectShader.emitGradientTexture = x);
                    TryReplace(effectShader.envMapTexture, x => effectShader.envMapTexture = x);
                    TryReplace(effectShader.envMaskTexture, x => effectShader.envMaskTexture = x);
                    
                    void TryReplace(NiString niString, Func<NiString, NiString> update) {
                        var str = niString.get();
                        if (replace(str) is {} replacement && !string.Equals(str, replacement, DataRelativePath.PathComparison)) {
                            update(new NiString(replacement));
                            modifiedEffectShader = true;
                        }
                    }

                    if (modifiedEffectShader) {
                        niHeader.ReplaceBlock(blockId, effectShader);
                        modifiedNif = true;
                    }
                } else {
                    // Remap BS Shader No  Lighting Property
                    var shaderNoLighting = blockCache.EditableBlockById<BSShaderNoLightingProperty>(blockId);
                    if (shaderNoLighting is not null && replace(shaderNoLighting.baseTexture.get()) is {} replacement) {
                        shaderNoLighting.baseTexture = new NiString(replacement);
                        niHeader.ReplaceBlock(blockId, shaderNoLighting);

                        modifiedNif = true;
                    }
                }
            }
        }

        if (modifiedNif) {
            try {
                nif.Save(fileLink.FullPath);
                logger.Here().Information("Remapped texture paths in nif {File}", fileLink.FullPath);
            } catch (Exception e) {
                logger.Here().Error(e, "Couldn't save modified nif {File}: {Exception}", fileLink, e);
            }
        }
    }
}
