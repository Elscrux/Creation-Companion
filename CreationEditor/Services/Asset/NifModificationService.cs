using System.IO.Abstractions;
using nifly;
using Serilog;
namespace CreationEditor.Services.Asset;

public sealed class NifModificationService(
    IFileSystem fileSystem,
    ILogger logger)
    : IModelModificationService {

    public void RemapLinks(string file, Func<string, bool> shouldReplace, string newLink) {
        if (!fileSystem.File.Exists(file)) return;

        using var nif = new NifFile();
        nif.Load(file);

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
                var occurenceIndices = new List<int>();
                for (var index = 0; index < items.Count; index++) {
                    var niString = items[index];
                    if (niString is null) continue;

                    if (shouldReplace(niString.get())) {
                        occurenceIndices.Add(index);
                    }
                }

                if (occurenceIndices.Count <= 0) continue;

                // Replace all occurrences with the new path
                var newPathNiString = new NiString(newLink);
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

                    if (shouldReplace(effectShader.greyscaleTexture.get())) {
                        effectShader.greyscaleTexture = new NiString(newLink);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.lightingTexture.get())) {
                        effectShader.lightingTexture = new NiString(newLink);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.normalTexture.get())) {
                        effectShader.normalTexture = new NiString(newLink);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.reflectanceTexture.get())) {
                        effectShader.reflectanceTexture = new NiString(newLink);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.sourceTexture.get())) {
                        effectShader.sourceTexture = new NiString(newLink);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.emitGradientTexture.get())) {
                        effectShader.emitGradientTexture = new NiString(newLink);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.envMapTexture.get())) {
                        effectShader.envMapTexture = new NiString(newLink);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.envMaskTexture.get())) {
                        effectShader.envMaskTexture = new NiString(newLink);
                        modifiedEffectShader = true;
                    }

                    if (modifiedEffectShader) {
                        niHeader.ReplaceBlock(blockId, effectShader);
                        modifiedNif = true;
                    }
                } else {
                    // Remap BS Shader No  Lighting Property
                    var shaderNoLighting = blockCache.EditableBlockById<BSShaderNoLightingProperty>(blockId);
                    if (shaderNoLighting is not null && shouldReplace(shaderNoLighting.baseTexture.get())) {
                        shaderNoLighting.baseTexture = new NiString(newLink);
                        niHeader.ReplaceBlock(blockId, shaderNoLighting);

                        modifiedNif = true;
                    }
                }
            }
        }

        if (modifiedNif) {
            try {
                nif.Save(file);
            } catch (Exception e) {
                logger.Here().Error("Couldn't save modified nif {File}: {Exception}", file, e.Message);
            }
        }
    }
}
