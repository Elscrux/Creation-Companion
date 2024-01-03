using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using MsBox.Avalonia;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public sealed class ModSelectionVM : ViewModel, IModSelectionVM {
    public static readonly ModType[] ModTypes = Enum.GetValues<ModType>();

    private const string NewModBaseName = "NewMod";
    private static string ReplacementName(int index) => $"{NewModBaseName} ({index})";

    private readonly IEditorEnvironment _editorEnvironment;
    private readonly List<ModInfo> _modInfos;

    private readonly SourceCache<LoadOrderModItem, ModKey> _mods = new(x => x.ModKey);
    public IObservableCollection<LoadOrderModItem> DisplayedMods { get; }
    private readonly Dictionary<ModKey, (HashSet<ModKey> Masters, bool Valid)> _masterInfos = new();

    [Reactive] public string ModSearchText { get; set; } = string.Empty;

    [Reactive] public LoadOrderModItem? SelectedMod { get; set; }
    public IModGetterVM SelectedModDetails { get; init; }

    public ModKey? ActiveMod => _mods.Items.FirstOrDefault(x => x.IsActive)?.ModKey;
    public IEnumerable<ModKey> SelectedMods => _mods.Items.Where(mod => mod.IsSelected).Select(x => x.ModKey);

    public IObservable<bool> CanLoad { get; }
    public IObservable<bool> AnyModsLoaded { get; }
    public IObservable<bool> AnyModsActive { get; }
    public IObservable<bool> NewModValid { get; }

    public ReactiveCommand<Unit, Unit> ToggleActive { get; }
    public Func<IReactiveSelectable, bool> CanSelect { get; } = selectable => selectable is LoadOrderModItem { MastersValid: true };

    [Reactive] public string NewModName { get; set; } = NewModBaseName;
    [Reactive] public ModType NewModType { get; set; } = ModType.Plugin;

    public ModSelectionVM(
        IGameReleaseContext gameReleaseContext,
        IEditorEnvironment editorEnvironment,
        IFileSystem fileSystem,
        IModGetterVM modGetterVM,
        IPluginListingsPathProvider pluginListingsProvider) {
        _editorEnvironment = editorEnvironment;
        SelectedModDetails = modGetterVM;

        // Collect mod infos
        using (var gameEnvironment = GameEnvironment.Typical.Construct(gameReleaseContext.Release, LinkCachePreferences.OnlyIdentifiers())) {
            _modInfos = SelectedModDetails.GetModInfos(gameEnvironment.LinkCache.ListedOrder).ToList();
        }

        // Try use NewModBaseName as new name for the mod, otherwise find a new name
        if (_modInfos.Exists(modInfo => modInfo.ModKey.Name == NewModBaseName)) {
            var counter = 2;
            while (_modInfos.Exists(modInfo => modInfo.ModKey.Name == ReplacementName(counter))) {
                counter++;
            }
            NewModName = ReplacementName(counter);
        }

        NewModValid = this.WhenAnyValue(
                x => x.NewModName,
                x => x.NewModType,
                (name, type) => (Name: name, Type: type))
            .Select(x => _modInfos.TrueForAll(modInfo => modInfo.ModKey.Type != x.Type || modInfo.ModKey.Name != x.Name));

        var filePath = pluginListingsProvider.Get(gameReleaseContext.Release);
        if (!fileSystem.File.Exists(filePath)) MessageBoxManager.GetMessageBoxStandard("Warning", $"Make sure {filePath} exists.");

        UpdateMasterInfos();

        // Fill mods with active load order
        _mods.Edit(updater => {
            for (var i = 0; i < _modInfos.Count; i++) {
                var modKey = _modInfos[i].ModKey;
                var modItem = new LoadOrderModItem(modKey, _masterInfos[modKey].Valid, (uint) i).DisposeWith(this);
                updater.AddOrUpdate(modItem);
            }
        });

        var connectedMods = _mods.Connect();

        DisplayedMods = connectedMods
            .Filter(this.WhenAnyValue(x => x.ModSearchText)
                .ThrottleMedium()
                .Select(searchText => new Func<LoadOrderModItem, bool>(mod =>
                    searchText.IsNullOrEmpty()
                 || mod.ModKey.FileName.String.Contains(searchText, StringComparison.OrdinalIgnoreCase))))
            .SortBy(mod => mod.LoadOrderIndex)
            .ToObservableCollection(this);

        var modSelected = connectedMods
            .AutoRefresh(modItem => modItem.IsSelected)
            .ToCollection();

        AnyModsLoaded = modSelected
            .Select(collection => collection.Any(mod => mod.IsSelected));

        var modActivated = connectedMods
            .AutoRefresh(modItem => modItem.IsActive);

        AnyModsActive = modActivated
            .ToCollection()
            .Select(collection => collection.Any(mod => mod.IsActive));

        modActivated
            .CombineLatest(this.WhenAnyValue(x => x._mods), (changedMods, allMods) => (ChangedMods: changedMods, AllMods: allMods))
            .Subscribe(x => {
                var loadOrderModItems = x.ChangedMods.Select(change => change.Current).Where(mod => mod.IsActive).ToList();
                if (loadOrderModItems.Count == 0) return;

                foreach (var item in x.AllMods.Items) {
                    if (loadOrderModItems[0] != item) {
                        item.IsActive = false;
                    }
                }
            })
            .DisposeWith(this);

        var selectedModValid = this
            .WhenAnyValue(x => x.SelectedMod)
            .NotNull()
            .Select(CanSelect);

        ToggleActive = ReactiveCommand.Create(
            canExecute: selectedModValid,
            execute: () => {
                if (SelectedMod is null) return;

                SelectedMod.IsActive = !SelectedMod.IsActive;
            });

        this.WhenAnyValue(x => x.SelectedMod)
            .NotNull()
            .Subscribe(selectedMod => {
                var mod = _modInfos.First(modInfo => modInfo.ModKey == selectedMod.ModKey);
                if (mod is null) return;

                SelectedModDetails.SetTo(mod);
            })
            .DisposeWith(this);

        CanLoad = NewModValid.CombineLatest(AnyModsLoaded, AnyModsActive,
            (newModValid, anyLoaded, anyActive) => anyLoaded && (newModValid || anyActive));
    }

    /// <summary>
    /// Build Dictionary _masterInfos with all masters of a single plugin recursively
    /// </summary>
    private void UpdateMasterInfos() {
        _masterInfos.Clear();
        var modKeys = _modInfos.Select(x => x.ModKey).ToHashSet();

        foreach (var modInfo in _modInfos) {
            var masters = new HashSet<ModKey>(modInfo.Masters);
            var valid = true;

            //Check that all masters are valid and compile list of all recursive masters
            foreach (var master in modInfo.Masters) {
                if (_masterInfos.TryGetValue(master, out var masterInfo)) {
                    if (masterInfo.Valid) {
                        masters.Add(masterInfo.Masters);
                        continue;
                    }
                }

                valid = false;
                break;
            }

            if (!valid) masters.Clear();
            masters = masters.OrderBy(key => modKeys.IndexOf(key)).ToHashSet();
            _masterInfos.Add(modInfo.ModKey, (masters, valid));

        }
    }

    public void LoadMods() {
        //Load all mods that are selected, or masters of selected mods
        var loadedMods = new HashSet<ModKey>();
        var missingMods = new Queue<ModKey>(SelectedMods);
        var modKeys = _modInfos.Select(x => x.ModKey).ToHashSet();

        while (missingMods.Count != 0) {
            var modKey = missingMods.Dequeue();
            loadedMods.Add(modKey);

            foreach (var master in _masterInfos[modKey].Masters.Where(masterMod => !loadedMods.Contains(masterMod))) {
                missingMods.Enqueue(master);
            }
        }

        var orderedMods = loadedMods.OrderBy(key => modKeys.IndexOf(key));
        if (ActiveMod is null) {
            _editorEnvironment.Build(orderedMods, NewModName, NewModType);
        } else {
            _editorEnvironment.Build(orderedMods, ActiveMod.Value);
        }
    }
}
