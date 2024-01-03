using System.Reactive;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Models.Selectables;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public interface IModSelectionVM {
    IObservableCollection<LoadOrderModItem> DisplayedMods { get; }
    string ModSearchText { get; set; }
    LoadOrderModItem? SelectedMod { get; set; }
    IModGetterVM SelectedModDetails { get; init; }
    IObservable<bool> CanLoad { get; }
    IObservable<bool> AnyModsLoaded { get; }
    IObservable<bool> AnyModsActive { get; }
    IObservable<bool> NewModValid { get; }
    ReactiveCommand<Unit, Unit> ToggleActive { get; }
    Func<IReactiveSelectable, bool> CanSelect { get; }
    string NewModName { get; set; }
    ModType NewModType { get; set; }
}
