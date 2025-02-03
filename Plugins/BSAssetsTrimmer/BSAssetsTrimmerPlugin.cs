using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.Services.Plugin;
using CreationEditor.Services.Plugin;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Skyrim;
using BSAssetsTrimmer.ViewModels;
using BSAssetsTrimmer.Views;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins;
namespace BSAssetsTrimmer;

public sealed class BSAssetsTrimmerPlugin(
    Func<BSAssetsTrimmerVM> bsAssetsTrimmerVMFactory,
    IEditorEnvironment editorEnvironment,
    PluginContext pluginContext)
    : IMenuPlugin<ISkyrimMod, ISkyrimModGetter> {

    public static readonly ModKey BSAssets = ModKey.FromFileName("BSAssets.esm");
    public static readonly ModKey[] BeyondSkyrimPlugins = [
        ModKey.FromFileName("BSArgonia.esm"),
        ModKey.FromFileName("BSAtmora.esm"),
        ModKey.FromFileName("BSHeartland.esm"),
        ModKey.FromFileName("BSIliacBay.esm"),
        ModKey.FromFileName("BSMorrowind.esm"),
        ModKey.FromFileName("BSRoscrea.esm"),
    ];

    public string Name => "BSAssets Trimmer";
    public string Description => "Trims BSAssets.esm to remove records that are not referenced by Beyond Skyrim mods";
    public Guid Guid => new("9a9dd271-2c7c-466b-9f24-a0d311a60f47");

    public bool CanRegister() {
        return editorEnvironment.GameEnvironment.LoadOrder.ContainsKey(BSAssets);
    }

    public Control GetControl() => new BSAssetsTrimmerView(bsAssetsTrimmerVMFactory());

    public DockMode DockMode { get; set; } = DockMode.Document;
    public Dock Dock { get; set; }
    public object GetIcon() => new SymbolIcon { Symbol = Symbol.Filter };
}
