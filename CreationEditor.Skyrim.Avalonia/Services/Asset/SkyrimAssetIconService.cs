using Avalonia.Controls;
using Avalonia.Layout;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim.Assets;
namespace CreationEditor.Skyrim.Avalonia.Services.Asset;

public sealed class SkyrimAssetIconService : IAssetIconService {
    private readonly Dictionary<string, Func<FAIconElement>> _fileExtensionIconsFactories;

    public SkyrimAssetIconService(
        IAssetTypeProvider assetTypeProvider,
        IArchiveService archiveService) {
        _fileExtensionIconsFactories = Enum
            .GetValues<ModType>()
            .ToDictionary<ModType, string, Func<FAIconElement>>(modType => modType.ToFileExtension(),
                modType => () => GetIcon(FASymbol.Save, modType.ToString()));

        _fileExtensionIconsFactories.Add(archiveService.GetExtension(), () => GetIcon(FASymbol.Library));

        foreach (var assetType in assetTypeProvider.AllAssetTypes) {
            foreach (var extension in assetType.FileExtensions) {
                _fileExtensionIconsFactories.TryAdd(extension, () => GetIcon(assetType));
            }
        }
    }

    public static FAIconElement GetGlyphIcon(string glyph, string? tooltip = null) {
        return new FAFontIcon {
            Glyph = glyph,
            VerticalAlignment = VerticalAlignment.Center,
            [ToolTip.TipProperty] = tooltip,
        };
    }

    public FAIconElement GetIcon(FASymbol symbol, string? tooltip = null) {
        return new FASymbolIcon {
            Symbol = symbol,
            VerticalAlignment = VerticalAlignment.Center,
            [ToolTip.TipProperty] = tooltip,
        };
    }

    public FAIconElement GetIcon(string extension, string? tooltip = null) {
        return _fileExtensionIconsFactories.TryGetValue(extension, out var factory)
            ? factory()
            : GetIcon(FASymbol.Document, tooltip);
    }

    public FAIconElement GetIcon(IAssetType? assetType) {
        var tooltip = GetTooltip(assetType);
        return assetType switch {
            SkyrimBehaviorAssetType => GetGlyphIcon("", tooltip),
            SkyrimBodyTextureAssetType => GetGlyphIcon("", tooltip),
            SkyrimDeformedModelAssetType => GetIcon(FASymbol.HomeFilled, tooltip),
            SkyrimInterfaceAssetType => GetIcon(FASymbol.Keyboard, tooltip),
            SkyrimModelAssetType => GetIcon(FASymbol.Home, tooltip),
            SkyrimMusicAssetType => GetGlyphIcon("🎜", tooltip),
            SkyrimScriptCompiledAssetType => GetIcon(FASymbol.Next, tooltip),
            SkyrimScriptSourceAssetType => GetIcon(FASymbol.Code, tooltip),
            SkyrimSeqAssetType => GetGlyphIcon("▶", tooltip),
            SkyrimSoundAssetType => GetIcon(FASymbol.Volume, tooltip),
            SkyrimTextureAssetType => GetIcon(FASymbol.Image, tooltip),
            SkyrimTranslationAssetType => GetGlyphIcon("ℒ", tooltip),
            _ => GetIcon(FASymbol.Document)
        };
    }

    private static string GetTooltip(IAssetType? assetType) {
        return assetType switch {
            SkyrimBehaviorAssetType => "Behavior",
            SkyrimBodyTextureAssetType => "Body Texture",
            SkyrimDeformedModelAssetType => "Deformed Model",
            SkyrimInterfaceAssetType => "Interface",
            SkyrimModelAssetType => "Model",
            SkyrimMusicAssetType => "Music",
            SkyrimScriptCompiledAssetType => "Script",
            SkyrimScriptSourceAssetType => "Script Source",
            SkyrimSeqAssetType => "Seq",
            SkyrimSoundAssetType => "Sound",
            SkyrimTextureAssetType => "Texture",
            SkyrimTranslationAssetType => "Translation",
            _ => string.Empty
        };
    }
}
