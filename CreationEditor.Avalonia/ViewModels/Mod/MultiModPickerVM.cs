using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public sealed class MultiModPickerVM : ViewModel, IModPickerVM {
    [Reactive] public string ModSearchText { get; set; } = string.Empty;
    [Reactive] public Func<OrderedModItem, bool>? Filter { get; set; }

    public ReadOnlyObservableCollection<OrderedModItem> Mods { get; }
    public IObservable<IReadOnlyCollection<OrderedModItem>> SelectedMods { get; }

    public MultiModPickerVM(
        ILinkCacheProvider linkCacheProvider,
        ISearchFilter searchFilter) {

        Mods = linkCacheProvider.LinkCacheChanged
            .Select(x => x.ListedOrder.AsObservableChangeSet())
            .Switch()
            .Transform((mod, i) => new OrderedModItem(mod.ModKey, (uint) i))
            .Filter(this.WhenAnyValue(x => x.ModSearchText)
                .ThrottleMedium()
                .CombineLatest(this.WhenAnyValue(x => x.Filter),
                    (searchText, filter) => (SearchText: searchText, Filter: filter))
                .Select(x => new Func<OrderedModItem, bool>(mod =>
                    (x.SearchText.IsNullOrEmpty()
                     || searchFilter.Filter(mod.ModKey.FileName.String, x.SearchText))
                 && (x.Filter is null || x.Filter(mod)))))
            .Sort(new FuncComparer<OrderedModItem>((x, y) => x.LoadOrderIndex.CompareTo(y.LoadOrderIndex)))
            .ToObservableCollection(this);

        var modSelected = Mods.ToObservableChangeSet()
            .AutoRefresh(modItem => modItem.IsSelected);

        SelectedMods = modSelected
            .ToCollection()
            .Select(x => x.Where(m => m.IsSelected).ToList());
    }
}
