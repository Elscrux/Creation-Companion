using CreationEditor.Services.DataSource;
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
                var occurenceIndices = new Dictionary<int, string>();
                for (var index = 0; index < items.Count; index++) {
                    var niString = items[index];
                    if (niString is null) continue;

                    var current = niString.get();
                    var replacement = replace(current);
                    if (current != replacement) {
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

                    if (replace(effectShader.greyscaleTexture.get()) is {} replacement1) {
                        effectShader.greyscaleTexture = new NiString(replacement1);
                        modifiedEffectShader = true;
                    }
                    if (replace(effectShader.lightingTexture.get()) is {} replacement2) {
                        effectShader.lightingTexture = new NiString(replacement2);
                        modifiedEffectShader = true;
                    }
                    if (replace(effectShader.normalTexture.get()) is {} replacement3) {
                        effectShader.normalTexture = new NiString(replacement3);
                        modifiedEffectShader = true;
                    }
                    if (replace(effectShader.reflectanceTexture.get()) is {} replacement) {
                        effectShader.reflectanceTexture = new NiString(replacement);
                        modifiedEffectShader = true;
                    }
                    if (replace(effectShader.sourceTexture.get()) is {} replacement4) {
                        effectShader.sourceTexture = new NiString(replacement4);
                        modifiedEffectShader = true;
                    }
                    if (replace(effectShader.emitGradientTexture.get()) is {} replacement5) {
                        effectShader.emitGradientTexture = new NiString(replacement5);
                        modifiedEffectShader = true;
                    }
                    if (replace(effectShader.envMapTexture.get()) is {} replacement6) {
                        effectShader.envMapTexture = new NiString(replacement6);
                        modifiedEffectShader = true;
                    }
                    if (replace(effectShader.envMaskTexture.get()) is {} replacement7) {
                        effectShader.envMaskTexture = new NiString(replacement7);
                        modifiedEffectShader = true;
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
            } catch (Exception e) {
                logger.Here().Error(e, "Couldn't save modified nif {File}: {Exception}", fileLink, e);
            }
        }
    }
}
