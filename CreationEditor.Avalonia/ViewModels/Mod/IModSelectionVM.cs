using System.Reactive;
using Avalonia.Data;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Models.Selectables;
using DynamicData.Binding;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public interface IModSelectionVM {
    IObservableCollection<LoadOrderModItem> DisplayedMods { get; }
    string ModSearchText { get; set; }
    LoadOrderModItem? SelectedMod { get; set; }
    IModGetterVM SelectedModDetails { get; init; }
    ModCreationVM ModCreationVM { get; }
    IObservable<bool> CanLoad { get; }
    IObservable<bool> AnyModsLoaded { get; }
    IObservable<bool> AnyModsActive { get; }
    ReactiveCommand<Unit, Unit> ToggleActive { get; }
    ReactiveCommand<Unit, Unit> ToggleSelection { get; }
    IBinding LoadOrderItemIsEnabled { get; }
    Func<IReactiveSelectable, bool> CanSelect { get; }
}
