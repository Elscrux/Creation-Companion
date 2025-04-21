using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Data;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Models.Selectables;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public class DesignModSelectionVM : IModSelectionVM {
    public IObservableCollection<LoadOrderModItem> DisplayedMods { get; } = new ObservableCollectionExtended<LoadOrderModItem> {
        new(new ModInfo(ModKey.FromFileName("Skyrim.esm")), true, 0),
        new(new ModInfo(ModKey.FromFileName("Update.esm")), true, 1),
        new(new ModInfo(ModKey.FromFileName("Dawnguard.esm")), true, 2),
        new(new ModInfo(ModKey.FromFileName("BestMod.esp")), true, 3),
        new(new ModInfo(ModKey.FromFileName("SecondBestMod.esp")), false, 4),
        new(new ModInfo(ModKey.FromFileName("ThirdBestMod.esp")), true, 5),
    };
    public string ModSearchText { get; set; } = string.Empty;
    public LoadOrderModItem? SelectedMod { get; set; } = new(new ModInfo(ModKey.FromFileName("BestMod.esp")), true, 3);
    public IModGetterVM SelectedModDetails { get; init; } = null!;
    public ModCreationVM ModCreationVM => null!;
    public IObservable<bool> AnyModsLoaded { get; } = Observable.Return(true);
    public IObservable<bool> AnyModsActive { get; } = Observable.Return(true);
    public IObservable<bool>? CanSave => null;
    public IObservable<string>? SaveButtonContent => null;
    public ReactiveCommand<Unit, Unit> ToggleActive { get; } = ReactiveCommand.Create(() => {});
    public IBinding LoadOrderItemIsEnabled { get; } = new Binding();
    public Func<IReactiveSelectable, bool> CanSelect { get; } = _ => true;
    public Task<bool> Save() => Task.FromResult(true);
}
