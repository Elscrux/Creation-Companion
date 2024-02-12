using CreationEditor.Avalonia.Models.Selectables;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Mod;

public class ModItem(ModKey modKey) : ReactiveObject, IReactiveSelectable, IModKeyed {
    [Reactive] public bool IsSelected { get; set; }
    public ModKey ModKey { get; } = modKey;
}