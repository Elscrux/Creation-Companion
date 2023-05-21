using System.IO.Abstractions;
using nifly;
namespace CreationEditor.Services.Asset;

public interface IModelModificationService {
    void RemapReferences(string file, Func<string, bool> shouldReplace, string newReference);
}

public sealed class NifModificationService : IModelModificationService {
    private readonly IFileSystem _fileSystem;

    public NifModificationService(IFileSystem fileSystem) {
        _fileSystem = fileSystem;
    }

    public void RemapReferences(string file, Func<string, bool> shouldReplace, string newReference) {
        if (!_fileSystem.File.Exists(file)) return;

        using var nif = new NifFile();
        nif.Load(file);

        if (!nif.IsValid()) return;

        var modifiedNif = false;

        var niHeader = nif.GetHeader();
        var blockCache = new niflycpp.BlockCache(niflycpp.BlockCache.SafeClone<NiHeader>(niHeader));
        for (uint blockId = 0; blockId < blockCache.Header.GetNumBlocks(); ++blockId) {
            // Remap BS Shader Texture Set
            using var shaderTextureSet = blockCache.EditableBlockById<BSShaderTextureSet>(blockId);
            if (shaderTextureSet != null) {
                // Find all indices where the old path is used
                var items = shaderTextureSet.textures.items();
                var occurenceIndices = new List<int>();
                for (var index = 0; index < items.Count; index++) {
                    var niString = items[index];
                    if (niString == null) continue;

                    if (shouldReplace(niString.get())) {
                        occurenceIndices.Add(index);
                    }
                }

                if (occurenceIndices.Count <= 0) continue;

                // Replace all occurrences with the new path
                var newPathNiString = new NiString(newReference);
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

                if (effectShader != null) {
                    var modifiedEffectShader = false;

                    if (shouldReplace(effectShader.greyscaleTexture.get())) {
                        effectShader.greyscaleTexture = new NiString(newReference);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.lightingTexture.get())) {
                        effectShader.lightingTexture = new NiString(newReference);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.normalTexture.get())) {
                        effectShader.normalTexture = new NiString(newReference);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.reflectanceTexture.get())) {
                        effectShader.reflectanceTexture = new NiString(newReference);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.sourceTexture.get())) {
                        effectShader.sourceTexture = new NiString(newReference);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.emitGradientTexture.get())) {
                        effectShader.emitGradientTexture = new NiString(newReference);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.envMapTexture.get())) {
                        effectShader.envMapTexture = new NiString(newReference);
                        modifiedEffectShader = true;
                    }
                    if (shouldReplace(effectShader.envMaskTexture.get())) {
                        effectShader.envMaskTexture = new NiString(newReference);
                        modifiedEffectShader = true;
                    }

                    if (modifiedEffectShader) {
                        niHeader.ReplaceBlock(blockId, effectShader);
                        modifiedNif = true;
                    }
                } else {
                    // Remap BS Shader No  Lighting Property
                    var shaderNoLighting = blockCache.EditableBlockById<BSShaderNoLightingProperty>(blockId);
                    if (shaderNoLighting != null && shouldReplace(shaderNoLighting.baseTexture.get())) {
                        shaderNoLighting.baseTexture = new NiString(newReference);
                        niHeader.ReplaceBlock(blockId, shaderNoLighting);

                        modifiedNif = true;
                    }
                }
            }
        }

        if (modifiedNif) nif.Save(file);
    }
}
