using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Comparer;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public sealed class ModPickerVM : ViewModel {
    [Reactive] public string ModSearchText { get; set; } = string.Empty;
    [Reactive] public bool MultiSelect { get; set; } = true;
    [Reactive] public Func<LoadOrderModItem, bool>? Filter { get; set; }

    public ReadOnlyObservableCollection<LoadOrderModItem> Mods { get; }
    public IObservable<IReadOnlyCollection<LoadOrderModItem>> SelectedMods { get; }

    public ModPickerVM(
        IEditorEnvironment editorEnvironment) {

        Mods = editorEnvironment.LinkCacheChanged
            .Select(x => x.ListedOrder.AsObservableChangeSet())
            .Switch()
            .Transform((mod, i) => new LoadOrderModItem(mod.ModKey, true, (uint) i))
            .Filter(this.WhenAnyValue(x => x.ModSearchText)
                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                .CombineLatest(this.WhenAnyValue(x => x.Filter),
                    (searchText, filter) => (SearchText: searchText, Filter: filter))
                .Select(x => new Func<LoadOrderModItem, bool>(mod =>
                    (x.SearchText.IsNullOrEmpty()
                     || mod.ModKey.FileName.String.Contains(x.SearchText, StringComparison.OrdinalIgnoreCase))
                    && (x.Filter is null || x.Filter(mod)))))
            .Sort(new FuncComparer<LoadOrderModItem>((x, y) => x.LoadOrderIndex.CompareTo(y.LoadOrderIndex)))
            .ToObservableCollection(this);

        var modSelected = Mods.ToObservableChangeSet()
            .AutoRefresh(modItem => modItem.IsSelected);

        SelectedMods = modSelected
            .ToCollection()
            .Select(x => x.Where(m => m.IsSelected).ToList());
    }
}
