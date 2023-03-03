using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Services.Busy;
using CreationEditor.Extension;
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public sealed class ModSelectionVM : ViewModel {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IGameEnvironment _environment;

    private readonly SourceCache<LoadOrderModItem, ModKey> _mods = new(x => x.ModKey);
    public IObservableCollection<LoadOrderModItem> DisplayedMods { get; }
    private readonly Dictionary<ModKey, (HashSet<ModKey> Masters, bool Valid)> _masterInfos = new();

    [Reactive] public string ModSearchText { get; set; } = string.Empty;

    [Reactive] public LoadOrderModItem? SelectedMod { get; set; }
    public IModGetterVM SelectedModDetails { get; init; }

    public ModKey? ActiveMod => _mods.Items.FirstOrDefault(x => x.IsActive)?.ModKey;
    public IEnumerable<ModKey> SelectedMods => _mods.Items.Where(mod => mod.IsSelected).Select(x => x.ModKey);

    public IObservable<bool> AnyModsLoaded { get; }
    public IObservable<bool> AnyModsActive { get; }

    public ReactiveCommand<Window, Unit> CloseAndLoadMods { get; }
    public ReactiveCommand<Unit, Unit> ToggleActive { get; }
    public Func<IReactiveSelectable, bool> CanSelect { get; } = selectable => selectable is LoadOrderModItem { MastersValid: true };

    public ModSelectionVM(
        IEnvironmentContext environmentContext,
        IEditorEnvironment editorEnvironment,
        IFileSystem fileSystem,
        IModGetterVM modGetterVM,
        IPluginListingsPathProvider pluginListingsProvider,
        IBusyService busyService) {
        _editorEnvironment = editorEnvironment;
        SelectedModDetails = modGetterVM;
        _environment = GameEnvironment.Typical.Construct(environmentContext.GameReleaseContext.Release, LinkCachePreferences.OnlyIdentifiers());

        var filePath = pluginListingsProvider.Get(environmentContext.GameReleaseContext.Release);
        if (!fileSystem.File.Exists(filePath)) MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Warning", $"Make sure {filePath} exists.");

        UpdateMasterInfos();

        // Fill mods with active load order
        _mods.Edit(updater => {
            var modKeys = _environment.LoadOrder.Keys.ToList();
            for (var i = 0; i < modKeys.Count; i++) {
                var modKey = modKeys[i];
                updater.AddOrUpdate(new LoadOrderModItem(modKey, _masterInfos[modKey].Valid, (uint) i));
            }
        });

        var connectedMods = _mods.Connect();

        DisplayedMods = _mods.Connect()
            .Filter(this.WhenAnyValue(x => x.ModSearchText)
                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
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
            .AutoRefresh(modItem => modItem.IsActive)
            .ToCollection();

        AnyModsActive = modActivated
            .Select(collection => collection.Any(mod => mod.IsActive));

        modActivated
            .Subscribe(changedMods => {
                foreach (var changedMod in changedMods) {
                    if (changedMod is { IsActive: true }) {
                        _mods.Items.Where(mod => mod != changedMod)
                            .ForEach(modItem => modItem.IsActive = false);
                    }
                    break;
                }
            });

        var selectedModValid = this
            .WhenAnyValue(x => x.SelectedMod)
            .NotNull()
            .Select(CanSelect);

        ToggleActive = ReactiveCommand.Create(
            canExecute: selectedModValid,
            execute: () => {
                if (SelectedMod == null) return;

                SelectedMod.IsActive = !SelectedMod.IsActive;
            });

        CloseAndLoadMods = ReactiveCommand.CreateFromTask<Window>(async (window, cancellationToken) => {
            window.Close();

            busyService.IsBusy = true;
            await Task.Run(LoadMods, cancellationToken);
            busyService.IsBusy = false;
        });

        this.WhenAnyValue(x => x.SelectedMod)
            .NotNull()
            .Subscribe(selectedMod => {
                var mod = _environment.LoadOrder.First(l => l.Key == selectedMod.ModKey).Value.Mod;
                if (mod == null) return;

                SelectedModDetails.SetTo(mod);
            });
    }

    /// <summary>
    /// Build Dictionary _masterInfos with all masters of a single plugin recursively
    /// </summary>
    private void UpdateMasterInfos() {
        _masterInfos.Clear();
        var modKeys = _environment.LoadOrder.Keys.ToHashSet();

        foreach (var listing in _environment.LoadOrder.Items) {
            if (listing.Mod == null) {
                _masterInfos.Add(listing.ModKey, (new HashSet<ModKey>(), false));
            } else {
                var directMasters = listing.Mod.MasterReferences.Select(m => m.Master).ToList();
                var masters = new HashSet<ModKey>(directMasters);
                var valid = true;

                //Check that all masters are valid and compile list of all recursive masters
                foreach (var master in directMasters) {
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
                _masterInfos.Add(listing.ModKey, (masters, valid));
            }
        }
    }

    private void LoadMods() {
        //Load all mods that are selected, or masters of selected mods
        var loadedMods = new HashSet<ModKey>();
        var missingMods = new Queue<ModKey>(SelectedMods);
        var modKeys = _environment.LoadOrder.Keys.ToHashSet();

        while (missingMods.Any()) {
            var modKey = missingMods.Dequeue();
            loadedMods.Add(modKey);

            foreach (var master in _masterInfos[modKey].Masters.Where(masterMod => !loadedMods.Contains(masterMod))) {
                missingMods.Enqueue(master);
            }
        }

        var orderedMods = loadedMods.OrderBy(key => modKeys.IndexOf(key));
        _editorEnvironment.Build(orderedMods, ActiveMod);
    }
}
