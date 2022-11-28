using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.Models.Mod;

public class ModItem : ReactiveObject, IReactiveSelectable {
    public const string NewPluginIdentifier = "< new plugin >";
    public static readonly ModKey NewMod = new();

    public ModItem(ModKey modKey) => ModKey = modKey;

    [Reactive] public bool IsSelected { get; set; }
    [Reactive] public ModKey ModKey { get; protected set; }

    public override string ToString() => ModKey == NewMod ? NewPluginIdentifier : ModKey.FileName;
}
