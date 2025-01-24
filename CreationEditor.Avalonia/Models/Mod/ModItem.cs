using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Services.Mutagen.Mod;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Mod;

public class ModItem(ModKey modKey) : ReactiveObject, ISelectableModKey, IReactiveSelectable {
    [Reactive] public bool IsSelected { get; set; }
    public ModKey ModKey { get; } = modKey;
}
