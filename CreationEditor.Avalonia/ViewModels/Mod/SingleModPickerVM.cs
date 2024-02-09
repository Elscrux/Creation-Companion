using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Environment;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public sealed class SingleModPickerVM : ViewModel, IModPickerVM {
    public ModCreationVM ModCreationVM { get; }

    [Reactive] public Func<LoadOrderModItem, bool>? Filter { get; set; }

    public ReadOnlyObservableCollection<LoadOrderModItem> Mods { get; }
    [Reactive] public LoadOrderModItem? SelectedMod { get; set; }

    [Reactive] public bool CanCreateNewMod { get; set; }

    public SingleModPickerVM(ILinkCacheProvider linkCacheProvider, ModCreationVM modCreationVM) {
        ModCreationVM = modCreationVM;

        Mods = linkCacheProvider.LinkCacheChanged
            .Select(x => x.ListedOrder.AsObservableChangeSet())
            .Switch()
            .Transform((mod, i) => new LoadOrderModItem(mod.ModKey, true, (uint) i))
            .Filter(this.WhenAnyValue(x => x.Filter)
                .Select(filter => new Func<LoadOrderModItem, bool>(mod => filter is null || filter(mod))))
            .Sort(new FuncComparer<LoadOrderModItem>((x, y) => x.LoadOrderIndex.CompareTo(y.LoadOrderIndex)))
            .ToObservableCollection(this);
    }
}
