using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Assets;
using nifly;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class NifModificationService(
    ILogger logger)
    : IModelModificationService {

    public void RemapLinks(DataSourceFileLink fileLink, Func<string, bool> shouldReplaceLink, DataRelativePath newLink) {
        if (!fileLink.Exists()) return;

        using var nif = new NifFile();
        nif.Load(fileLink.FullPath);

        if (!nif.IsValid()) return;

        var modifiedNif = false;

        var newLinkPath = newLink.Path;

        var niHeader = nif.GetHeader();
        var blockCache = new niflycpp.BlockCache(niflycpp.BlockCache.SafeClone<NiHeader>(niHeader));
        for (uint blockId = 0; blockId < blockCache.Header.GetNumBlocks(); ++blockId) {
            // Remap BS Shader Texture Set
            using var shaderTextureSet = blockCache.EditableBlockById<BSShaderTextureSet>(blockId);
            if (shaderTextureSet is not null) {
                // Find all indices where the old path is used
                var items = shaderTextureSet.textures.items();
                var occurenceIndices = new List<int>();
                for (var index = 0; index < items.Count; index++) {
                    var niString = items[index];
                    if (niString is null) continue;

                    if (shouldReplaceLink(niString.get())) {
                        occurenceIndices.Add(index);
                    }
                }

                if (occurenceIndices.Count <= 0) continue;

                // Replace all occurrences with the new path
                var newPathNiString = new NiString(newLinkPath);
                foreach (var occurenceIndex in occurenceIndices) {
                    items[occurenceIndex] = newPathNiString;
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

                    if (shouldReplaceLink(effectShader.greyscaleTexture.get())) {
                        effectShader.greyscaleTexture = new NiString(newLinkPath);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplaceLink(effectShader.lightingTexture.get())) {
                        effectShader.lightingTexture = new NiString(newLinkPath);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplaceLink(effectShader.normalTexture.get())) {
                        effectShader.normalTexture = new NiString(newLinkPath);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplaceLink(effectShader.reflectanceTexture.get())) {
                        effectShader.reflectanceTexture = new NiString(newLinkPath);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplaceLink(effectShader.sourceTexture.get())) {
                        effectShader.sourceTexture = new NiString(newLinkPath);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplaceLink(effectShader.emitGradientTexture.get())) {
                        effectShader.emitGradientTexture = new NiString(newLinkPath);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplaceLink(effectShader.envMapTexture.get())) {
                        effectShader.envMapTexture = new NiString(newLinkPath);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplaceLink(effectShader.envMaskTexture.get())) {
                        effectShader.envMaskTexture = new NiString(newLinkPath);
                        modifiedEffectShader = true;
                    }

                    if (modifiedEffectShader) {
                        niHeader.ReplaceBlock(blockId, effectShader);
                        modifiedNif = true;
                    }
                } else {
                    // Remap BS Shader No  Lighting Property
                    var shaderNoLighting = blockCache.EditableBlockById<BSShaderNoLightingProperty>(blockId);
                    if (shaderNoLighting is not null && shouldReplaceLink(shaderNoLighting.baseTexture.get())) {
                        shaderNoLighting.baseTexture = new NiString(newLinkPath);
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
                logger.Here().Error(e, "Couldn't save modified nif {File}: {Exception}", fileLink, e.Message);
            }
        }
    }
}
