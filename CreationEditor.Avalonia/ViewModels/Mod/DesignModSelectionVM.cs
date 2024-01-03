using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Models.Selectables;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public class DesignModSelectionVM : IModSelectionVM {
    public IObservableCollection<LoadOrderModItem> DisplayedMods { get; } = new ObservableCollectionExtended<LoadOrderModItem> {
        new(ModKey.FromFileName("Skyrim.esm"), true, 0),
        new(ModKey.FromFileName("Update.esm"), true, 1),
        new(ModKey.FromFileName("Dawnguard.esm"), true, 2),
        new(ModKey.FromFileName("BestMod.esp"), true, 3),
        new(ModKey.FromFileName("SecondBestMod.esp"), false, 4),
        new(ModKey.FromFileName("ThirdBestMod.esp"), true, 5),
    };
    public string ModSearchText { get; set; } = string.Empty;
    public LoadOrderModItem? SelectedMod { get; set; } = new(ModKey.FromFileName("BestMod.esp"), true, 3);
    public IModGetterVM SelectedModDetails { get; init; } = null;
    public IObservable<bool> CanLoad { get; } = Observable.Return(true);
    public IObservable<bool> AnyModsLoaded { get; } = Observable.Return(true);
    public IObservable<bool> AnyModsActive { get; } = Observable.Return(true);
    public IObservable<bool> NewModValid { get; } = Observable.Return(true);
    public ReactiveCommand<Unit, Unit> ToggleActive { get; } = ReactiveCommand.Create(() => { });
    public Func<IReactiveSelectable, bool> CanSelect { get; } = _ => true;
    public string NewModName { get; set; } = "NewMod";
    public ModType NewModType { get; set; } = ModType.Plugin;
}
