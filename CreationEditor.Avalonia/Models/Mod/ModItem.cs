using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Services.Mutagen.Mod;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.Mod;

public partial class ModItem(ModKey modKey) : ReactiveObject, ISelectableModKey, IReactiveSelectable {
    [Reactive] public partial bool IsSelected { get; set; }
    public ModKey ModKey { get; } = modKey;

    public override string ToString() => ModKey.ToString();
}
