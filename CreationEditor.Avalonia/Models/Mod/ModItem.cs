using CreationEditor.Avalonia.Models.Selectables;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Mod;

public class ModItem : ReactiveObject, IReactiveSelectable {
    public ModItem(ModKey modKey) => ModKey = modKey;

    [Reactive] public bool IsSelected { get; set; }
    public ModKey ModKey { get; }
}
