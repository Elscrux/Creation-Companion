using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Environment;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public sealed class SingleModPickerVM : ViewModel, IModPickerVM {
    public ModCreationVM ModCreationVM { get; }

    [Reactive] public Func<OrderedModItem, bool>? Filter { get; set; }

    public ReadOnlyObservableCollection<OrderedModItem> Mods { get; }

    [Reactive] public OrderedModItem? SelectedMod { get; set; }
    [Reactive] public string SelectionText { get; set; } = string.Empty;

    [Reactive] public bool CanCreateNewMod { get; set; }

    public IObservable<bool> HasModSelected { get; }

    public SingleModPickerVM(ILinkCacheProvider linkCacheProvider, ModCreationVM modCreationVM) {
        ModCreationVM = modCreationVM;

        Mods = linkCacheProvider.LinkCacheChanged
            .Select(x => x.ListedOrder.AsObservableChangeSet())
            .Switch()
            .Transform((mod, i) => new OrderedModItem(mod.ModKey, (uint) i))
            .Filter(this.WhenAnyValue(x => x.Filter)
                .Select(filter => new Func<OrderedModItem, bool>(mod => filter is null || filter(mod))))
            .Sort(new FuncComparer<OrderedModItem>((x, y) => x.LoadOrderIndex.CompareTo(y.LoadOrderIndex)))
            .ToObservableCollection(this);

        this.WhenAnyValue(x => x.Filter)
            .Subscribe(filter => {
                if (filter is null) {
                    SelectedMod ??= Mods.FirstOrDefault();
                } else if (SelectedMod is null || !filter(SelectedMod)) {
                    SelectedMod = Mods.FirstOrDefault(filter);
                }
            })
            .DisposeWith(this);

        // When the own mod creator creates a new mod, select it
        ModCreationVM.CreateModCommand
            .Subscribe(mod => {
                if (mod is null) return;

                SelectMod(mod.ModKey);
            })
            .DisposeWith(this);

        Mods.WhenCollectionChanges()
            .Subscribe(_ => {
                if (SelectedMod is null || (Filter is not null && !Filter(SelectedMod))) {
                    SelectedMod = Mods.FirstOrDefault(mod => mod.ModKey.FileName.String.Equals(SelectionText, StringComparison.OrdinalIgnoreCase));
                }
            })
            .DisposeWith(this);

        HasModSelected = this
            .WhenAnyValue(x => x.SelectedMod)
            .Select(x => x is not null && !x.ModKey.IsNull);
    }

    public bool SelectMod(ModKey modKey) {
        var item = Mods.FirstOrDefault(x => x.ModKey == modKey);
        if (item is null) return false;

        SelectedMod = item;
        return true;
    }
}
