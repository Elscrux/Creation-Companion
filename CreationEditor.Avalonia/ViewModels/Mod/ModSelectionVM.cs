using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Models.Selectables;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Views;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod.Save;
using DynamicData;
using DynamicData.Binding;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public sealed class ModSelectionVM : ViewModel, IModSelectionVM {
    public static readonly IReadOnlyList<ModType> ModTypes = Enum.GetValues<ModType>();

    private readonly IEditorEnvironment _editorEnvironment;

    private readonly ReadOnlyObservableCollection<LoadOrderModItem> _mods;
    public IObservableCollection<LoadOrderModItem> DisplayedMods { get; }

    [Reactive] public string ModSearchText { get; set; } = string.Empty;

    [Reactive] public LoadOrderModItem? SelectedMod { get; set; }
    private readonly MainWindow _mainWindow;
    private readonly IRecordEditorController _recordEditorController;
    private readonly IModSaveService _modSaveService;
    public IModGetterVM SelectedModDetails { get; init; }
    public ModCreationVM ModCreationVM { get; }
    public bool MissingPluginsFile { get; }
    public string PluginsFilePath { get; }

    public ModKey? ActiveMod => _mods.FirstOrDefault(x => x.IsActive)?.ModKey;
    public IEnumerable<ModKey> SelectedMods => _mods.Where(mod => mod.IsSelected).Select(x => x.ModKey);

    public IObservable<bool> CanLoad { get; }
    public IObservable<bool> AnyModsLoaded { get; }
    public IObservable<bool> AnyModsActive { get; }

    private readonly Subject<Unit> _refreshListings = new();

    public ReactiveCommand<Unit, Unit> ToggleActive { get; }
    public ReactiveCommand<Unit, Unit> ToggleSelection { get; }
    public Func<IReactiveSelectable, bool> CanSelect { get; } = selectable => selectable is LoadOrderModItem { MastersValid: true };

    public ModSelectionVM(
        MainWindow mainWindow,
        IRecordEditorController recordEditorController,
        IModSaveService modSaveService,
        IGameReleaseContext gameReleaseContext,
        IEditorEnvironment editorEnvironment,
        IFileSystem fileSystem,
        IModGetterVM modGetterVM,
        ModCreationVM modCreationVM,
        IDataDirectoryProvider dataDirectoryProvider,
        ILoadOrderListingsProvider listingsProvider,
        IPluginListingsPathProvider listingsPathProvider) {
        _editorEnvironment = editorEnvironment;
        _mainWindow = mainWindow;
        _recordEditorController = recordEditorController;
        _modSaveService = modSaveService;
        SelectedModDetails = modGetterVM;
        ModCreationVM = modCreationVM;
        var release = gameReleaseContext.Release;
        PluginsFilePath = listingsPathProvider.Get(release);
        MissingPluginsFile = !fileSystem.File.Exists(PluginsFilePath);

        var directoryPath = dataDirectoryProvider.Path;

        // Mods listed in the plugins file
        var listedModInfos = _refreshListings.Select(_ => listingsProvider.Get()
            .Select(listing => {
                var path = fileSystem.Path.Combine(directoryPath, listing.FileName);
                if (!fileSystem.Path.Exists(path)) return null;

                var binaryReadParameters = new BinaryReadParameters { FileSystem = fileSystem };
                var modPath = new ModPath(listing.ModKey, path);
                var parsingMeta = ParsingMeta.Factory(binaryReadParameters, release, modPath);
                var stream = new MutagenBinaryReadStream(path, parsingMeta);
                using var frame = new MutagenFrame(stream);
                return modGetterVM.GetModInfo(listing.ModKey, frame);
            })
            .NotNull()
            .ToArray());

        // Mods (also memory-only) in the current load order
        var loadOrderModInfos = _editorEnvironment.LinkCacheChanged
            .Select(x => x.ListedOrder.Select(modGetterVM.GetModInfo).NotNull())
            .StartWith(Array.Empty<ModInfo>());

        _mods = listedModInfos
            .CombineLatest(loadOrderModInfos, (listed, loadOrder) => (Listed: listed, LoadOrder: loadOrder.ToList()))
            .Select(x => {
                var mods = x.Listed.Concat(x.LoadOrder).DistinctBy(info => info.ModKey).ToList();
                var masterInfos = GetMasterInfos(mods);
                var activeModKey = _editorEnvironment.ActiveMod.ModKey;
                return mods
                    .Select((modInfo, i) => {
                        var modKey = modInfo.ModKey;
                        var isActive = modKey.Equals(activeModKey);

                        var masterInfo = masterInfos[modKey];
                        return new LoadOrderModItem(modInfo, masterInfo.Valid, (uint) i) {
                            IsActive = isActive,
                            IsSelected = x.LoadOrder.Exists(info => info.ModKey == modKey),
                        };
                    })
                    .NotNull()
                    .ToList();
            })
            .ToObservableCollection(this);

        _editorEnvironment.ActiveModChanged
            .Subscribe(activeMod => {
                foreach (var mod in _mods) {
                    mod.IsActive = mod.ModKey == activeMod;
                }
            })
            .DisposeWith(this);

        var connectedMods = _mods.ToObservableChangeSet();

        DisplayedMods = connectedMods
            .Filter(this.WhenAnyValue(x => x.ModSearchText)
                .ThrottleMedium()
                .Select(searchText => new Func<LoadOrderModItem, bool>(mod =>
                    searchText.IsNullOrWhitespace()
                 || mod.ModKey.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))))
            .AddKey(item => item.ModKey)
            .SortBy(item => item.LoadOrderIndex)
            .ToObservableCollection(this);

        AnyModsLoaded = connectedMods
            .AutoRefresh(modItem => modItem.IsSelected)
            .ToCollection()
            .Select(collection => collection.Any(mod => mod.IsSelected));

        var modActivated = connectedMods
            .AutoRefresh(modItem => modItem.IsActive)
            .ToCollection();

        AnyModsActive = modActivated
            .Select(collection => collection.Any(mod => mod.IsActive));

        modActivated
            .CombineLatest(
                _mods.ObserveCollectionChanges().Select(_ => _mods),
                (changedMods, allMods) => (ChangedMods: changedMods, AllMods: allMods))
            .Subscribe(x => {
                var activeMod = x.ChangedMods.FirstOrDefault(mod => mod.IsActive);
                if (activeMod is null) return;

                foreach (var item in x.AllMods) {
                    if (activeMod.ModKey != item.ModKey) {
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
            })
            .DisposeWith(this);

        ToggleSelection = ReactiveCommand.Create(
            canExecute: selectedModValid,
            execute: () => {
                if (SelectedMod is null) return;

                SelectedMod.IsSelected = !SelectedMod.IsSelected;
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.SelectedMod)
            .NotNull()
            .Subscribe(selectedMod => {
                var mod = _mods.FirstOrDefault(mod => mod.ModKey == selectedMod.ModKey);
                if (mod is null) return;

                SelectedModDetails.SetTo(mod.ModInfo);
            })
            .DisposeWith(this);

        CanLoad = AnyModsLoaded;
        CanLoad = ModCreationVM.IsValid().CombineLatest(
            AnyModsLoaded,
            AnyModsActive,
            (newModValid, anyLoaded, anyActive) => anyLoaded && (newModValid || anyActive));
    }

    /// <summary>
    /// Build Dictionary _masterInfos with all masters of a single plugin recursively
    /// </summary>
    /// <param name="mods">List of all mods</param>
    private Dictionary<ModKey, (HashSet<ModKey> Masters, bool Valid)> GetMasterInfos(IReadOnlyList<ModInfo> mods) {
        var masterInfos = new Dictionary<ModKey, (HashSet<ModKey> Masters, bool Valid)>();
        var modKeyIndices = mods
            .Select((mod, i) => (mod.ModKey, i))
            .ToDictionary(x => x.ModKey, x => x.i);

        foreach (var mod in mods) {
            var masters = new HashSet<ModKey>(mod.Masters);
            var valid = true;

            //Check that all masters are valid and compile list of all recursive masters
            foreach (var master in mod.Masters) {
                if (masterInfos.TryGetValue(master, out var masterInfo) && masterInfo.Valid) {
                    masters.Add(masterInfo.Masters);
                    continue;
                }

                valid = false;
                break;
            }

            if (valid) {
                masters = masters.OrderBy(key => modKeyIndices[key]).ToHashSet();
            } else {
                masters.Clear();
            }

            masterInfos.Add(mod.ModKey, (masters, valid));
        }

        return masterInfos;
    }

    public void RefreshListings() => _refreshListings.OnNext(Unit.Default);

    public async Task<bool> LoadMods() {
        // Prompt to save the active mod when changing to another mod
        if (!await CheckPendingSave()) return false;

        //Load all mods that are selected, or masters of selected mods
        var loadedMods = new HashSet<ModKey>();
        var missingMods = new Queue<ModKey>(SelectedMods);
        var mods = _mods.ToDictionary(x => x.ModKey, x => x);

        while (missingMods.Count != 0) {
            var modKey = missingMods.Dequeue();
            loadedMods.Add(modKey);

            foreach (var master in mods[modKey].ModInfo.Masters.Where(masterMod => !loadedMods.Contains(masterMod))) {
                missingMods.Enqueue(master);
            }
        }

        _editorEnvironment.Update(updater => {
            if (ActiveMod is null) {
                updater.ActiveMod.New(ModCreationVM.ModNameOrBackup, ModCreationVM.NewModType);
            } else {
                updater.ActiveMod.Existing(ActiveMod.Value);
            }

            return updater
                .LoadOrder.SetImmutableMods(loadedMods.OrderBy(key => mods[key].LoadOrderIndex))
                .Build();
        });

        return true;
    }

    private async Task<bool> CheckPendingSave() {
        if (_editorEnvironment.ActiveMod.ModKey.IsNull
         || _editorEnvironment.ActiveMod.ModKey == ActiveMod
         || !_editorEnvironment.ActiveMod.EnumerateMajorRecords().Any()) return true;

        var anyEditorsOpen = _recordEditorController.AnyEditorsOpen();
        var appendix = anyEditorsOpen ? "\nUnless you cancel, this will close all open editors." : string.Empty;
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "Save Changes",
            $"Do you want to save your changes to {_editorEnvironment.ActiveMod.ModKey}?{appendix}",
            ButtonEnum.YesNoCancel,
            Icon.Warning,
            WindowStartupLocation.CenterOwner);

        var pressedButton = await messageBox.ShowWindowDialogAsync(_mainWindow);
        switch (pressedButton) {
            case ButtonResult.Yes:
                _modSaveService.SaveMod(_editorEnvironment.ActiveMod);
                break;
            case ButtonResult.No:
                break;
            case ButtonResult.Cancel:
                return false;
            default:
                throw new InvalidOperationException();
        }

        if (anyEditorsOpen) {
            _recordEditorController.CloseAllEditors();
        }

        return true;
    }
}
