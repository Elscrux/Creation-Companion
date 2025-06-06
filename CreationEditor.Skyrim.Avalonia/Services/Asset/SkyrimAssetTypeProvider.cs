﻿using CreationEditor.Services.Asset;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Skyrim.Assets;
namespace CreationEditor.Skyrim.Avalonia.Services.Asset;

public sealed class SkyrimAssetTypeProvider : IAssetTypeProvider {
    public IReadOnlyList<IAssetType> AllAssetTypes { get; } = [
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
        SkyrimInterfaceAssetType.Instance,
    ];

    public IDictionary<IAssetType, Func<DataRelativePath, IAssetLink>> AssetTypeConstructor { get; }
        = new Dictionary<IAssetType, Func<DataRelativePath, IAssetLink>> {
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
            { SkyrimInterfaceAssetType.Instance, path => new AssetLink<SkyrimInterfaceAssetType>(path) },
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
            { SkyrimInterfaceAssetType.Instance, "INT" },
        };

    public IAssetType Texture => SkyrimTextureAssetType.Instance;
    public IAssetType Model => SkyrimModelAssetType.Instance;
    public IAssetType ScriptSource => SkyrimScriptSourceAssetType.Instance;
    public IAssetType Script => SkyrimScriptCompiledAssetType.Instance;
    public IAssetType Sound => SkyrimSoundAssetType.Instance;
    public IAssetType Music => SkyrimMusicAssetType.Instance;
    public IAssetType DeformedModel => SkyrimDeformedModelAssetType.Instance;
    public IAssetType Seq => SkyrimSeqAssetType.Instance;
    public IAssetType BodyTexture => SkyrimBodyTextureAssetType.Instance;
    public IAssetType Behavior => SkyrimBehaviorAssetType.Instance;
    public IAssetType Translation => SkyrimTranslationAssetType.Instance;
    public IAssetType Interface => SkyrimInterfaceAssetType.Instance;
}
