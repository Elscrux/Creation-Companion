using System;
using System.Collections.Generic;
using CreationEditor.Services.Asset;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim.Assets;
namespace CreationEditor.Skyrim.Avalonia.Services.Asset;

public sealed class SkyrimAssetTypeProvider : IAssetTypeProvider {
    public IReadOnlyList<IAssetType> AllAssetTypes { get; } = new IAssetType[] {
        SkyrimTextureAssetType.Instance,
        SkyrimModelAssetType.Instance,
        SkyrimScriptSourceAssetType.Instance,
        SkyrimScriptCompiledAssetType.Instance,
        SkyrimSoundAssetType.Instance,
        SkyrimMusicAssetType.Instance,
        SkyrimDeformedModelAssetType.Instance,
        SkyrimSeqAssetType.Instance,
        SkyrimBodyTextureAssetType.Instance,
        SkyrimBehaviorAssetType.Instance,
        SkyrimTranslationAssetType.Instance,
    };

    public IDictionary<IAssetType, Func<string, IAssetLink>> AssetTypeConstructor { get; }
        = new Dictionary<IAssetType, Func<string, IAssetLink>> {
            { SkyrimTextureAssetType.Instance, path => new AssetLink<SkyrimTextureAssetType>(path) },
            { SkyrimModelAssetType.Instance, path => new AssetLink<SkyrimModelAssetType>(path) },
            { SkyrimScriptSourceAssetType.Instance, path => new AssetLink<SkyrimScriptSourceAssetType>(path) },
            { SkyrimScriptCompiledAssetType.Instance, path => new AssetLink<SkyrimScriptCompiledAssetType>(path) },
            { SkyrimSoundAssetType.Instance, path => new AssetLink<SkyrimSoundAssetType>(path) },
            { SkyrimMusicAssetType.Instance, path => new AssetLink<SkyrimMusicAssetType>(path) },
            { SkyrimDeformedModelAssetType.Instance, path => new AssetLink<SkyrimDeformedModelAssetType>(path) },
            { SkyrimSeqAssetType.Instance, path => new AssetLink<SkyrimSeqAssetType>(path) },
            { SkyrimBodyTextureAssetType.Instance, path => new AssetLink<SkyrimBodyTextureAssetType>(path) },
            { SkyrimBehaviorAssetType.Instance, path => new AssetLink<SkyrimBehaviorAssetType>(path) },
            { SkyrimTranslationAssetType.Instance, path => new AssetLink<SkyrimTranslationAssetType>(path) },
        };

    public IDictionary<IAssetType, string> AssetTypeIdentifiers { get; }
        = new Dictionary<IAssetType, string> {
            { SkyrimTextureAssetType.Instance, "TXR" },
            { SkyrimModelAssetType.Instance, "MDL" },
            { SkyrimScriptSourceAssetType.Instance, "SRC" },
            { SkyrimScriptCompiledAssetType.Instance, "SCR" },
            { SkyrimSoundAssetType.Instance, "SOU" },
            { SkyrimMusicAssetType.Instance, "MUS" },
            { SkyrimDeformedModelAssetType.Instance, "DEF" },
            { SkyrimSeqAssetType.Instance, "SEQ" },
            { SkyrimBodyTextureAssetType.Instance, "BDY" },
            { SkyrimBehaviorAssetType.Instance, "BEH" },
            { SkyrimTranslationAssetType.Instance, "TRL" },
        };

    public IAssetType Texture => SkyrimTextureAssetType.Instance;
    public IAssetType Model => SkyrimModelAssetType.Instance;
    public IAssetType ScriptSource => SkyrimScriptSourceAssetType.Instance;
    public IAssetType Script => SkyrimScriptCompiledAssetType.Instance;
    public IAssetType Sound => SkyrimSoundAssetType.Instance;
    public IAssetType Music => SkyrimMusicAssetType.Instance;

    public IAssetLink TextureLink(string path) => new AssetLink<SkyrimTextureAssetType>(path);
    public IAssetLink ModelLink(string path) => new AssetLink<SkyrimModelAssetType>(path);
    public IAssetLink ScriptSourceLink(string path) => new AssetLink<SkyrimScriptSourceAssetType>(path);
    public IAssetLink ScriptLink(string path) => new AssetLink<SkyrimScriptCompiledAssetType>(path);
    public IAssetLink SoundLink(string path) => new AssetLink<SkyrimSoundAssetType>(path);
    public IAssetLink MusicLink(string path) => new AssetLink<SkyrimMusicAssetType>(path);
}
