using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Resources.Comparer;
using CreationEditor.Services.Environment;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public sealed partial class SingleModPickerVM : ViewModel, IModPickerVM {
    public ModCreationVM ModCreationVM { get; }

    [Reactive] public partial Func<OrderedModItem, bool>? Filter { get; set; }

    public ReadOnlyObservableCollection<OrderedModItem> Mods { get; }

    [Reactive] public partial OrderedModItem? SelectedMod { get; set; }
    public IObservable<OrderedModItem?> SelectedModChanged { get; }
    [Reactive] public partial string SelectionText { get; set; }

    [Reactive] public partial bool CanCreateNewMod { get; set; }

    public IObservable<bool> HasModSelected { get; }

    public SingleModPickerVM(ILinkCacheProvider linkCacheProvider, ModCreationVM modCreationVM) {
        SelectionText = string.Empty;
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
            .Subscribe(mod => SelectMod(mod.ModKey))
            .DisposeWith(this);

        Mods.WhenCollectionChanges()
            .Subscribe(_ => {
                if (SelectedMod is null || (Filter is not null && !Filter(SelectedMod))) {
                    SelectedMod = Mods.FirstOrDefault(mod => mod.ModKey.FileName.String.Equals(SelectionText, StringComparison.OrdinalIgnoreCase));
                }
            })
            .DisposeWith(this);

        SelectedModChanged = this.WhenAnyValue(x => x.SelectedMod);

        HasModSelected = SelectedModChanged
            .NotNull()
            .Select(x => !x.ModKey.IsNull);
    }

    public bool SelectMod(ModKey modKey) {
        var item = Mods.FirstOrDefault(x => x.ModKey == modKey);
        if (item is null) return false;

        SelectedMod = item;
        return true;
    }
}
