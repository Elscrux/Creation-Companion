using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Services.Busy;
using CreationEditor.Environment;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order.DI;
using MutagenLibrary.Core.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public class ModSelectionVM : ViewModel {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IBusyService _busyService;
    private readonly IGameEnvironment _environment;

    private readonly List<ActivatableModItem> _mods;
    public IObservableCollection<ActivatableModItem> DisplayedMods { get; }
    private readonly Dictionary<ModKey, (HashSet<ModKey> Masters, bool Valid)> _masterInfos = new();

    [Reactive] public string ModSearchText { get; set; } = string.Empty;

    [Reactive] public ActivatableModItem? SelectedMod { get; set; }
    public IModGetterVM SelectedModDetails { get; init; }

    public ModKey? ActiveMod => _mods.FirstOrDefault(x => x.IsActive)?.ModKey;
    public IEnumerable<ModKey> SelectedMods => _mods.Where(mod => mod.IsSelected).Select(x => x.ModKey);

    private readonly ObservableAsPropertyHelper<bool> _anyModsLoaded;
    public bool AnyModsLoaded => _anyModsLoaded.Value;

    private readonly ObservableAsPropertyHelper<bool> _anyModsActive;
    public bool AnyModsActive => _anyModsActive.Value;

    public ReactiveCommand<Window, Unit> CloseAndLoadMods { get; }
    public ReactiveCommand<Unit, Unit> ToggleActive { get; }
    public Func<IReactiveSelectable, bool> CanSelect { get; } = selectable => selectable is ActivatableModItem { MastersValid: true };

    public ModSelectionVM(
        ISimpleEnvironmentContext simpleEnvironmentContext,
        IEditorEnvironment editorEnvironment,
        IFileSystem fileSystem,
        IModGetterVM modGetterVM,
        IPluginListingsPathProvider pluginListingsProvider,
        IBusyService busyService) {
        _editorEnvironment = editorEnvironment;
        _busyService = busyService;
        SelectedModDetails = modGetterVM;
        _environment = GameEnvironment.Typical.Construct(simpleEnvironmentContext.GameReleaseContext.Release, LinkCachePreferences.OnlyIdentifiers());

        var filePath = pluginListingsProvider.Get(simpleEnvironmentContext.GameReleaseContext.Release);
        if (!fileSystem.File.Exists(filePath)) MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Warning", $"Make sure {filePath} exists.");

        UpdateMasterInfos();
        _mods = _environment.LoadOrder.Keys
            .Select(modKey => new ActivatableModItem(modKey, _masterInfos[modKey].Valid))
            .ToList();

        DisplayedMods = this.WhenAnyValue(x => x.ModSearchText)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .ObserveOnTaskpool()
            .Select(searchText => {
                return Observable.Create<ActivatableModItem>((obs, cancel) => {
                    foreach (var mod in _mods) {
                        if (cancel.IsCancellationRequested) return Task.CompletedTask;
                        if (!mod.ModKey.FileName.String.Contains(searchText, StringComparison.OrdinalIgnoreCase)) continue;

                        obs.OnNext(mod);
                    }
                    obs.OnCompleted();
                    return Task.CompletedTask;
                }).ToObservableChangeSet(x => x.ModKey.FileName.String);
            })
            .Switch()
            .ObserveOnGui()
            .ToObservableCollection(this);

        var observableMods = _mods
            .ToObservable()
            .ToObservableChangeSet();

        var modSelected = observableMods
            .AutoRefresh(modItem => modItem.IsSelected);
        
        _anyModsLoaded = modSelected
            .ToCollection()
            .Select(collection => collection.Any(mod => mod.IsSelected))
            .ToProperty(this, vm => vm.AnyModsLoaded);

        var modActivated = observableMods
            .AutoRefresh(modItem => modItem.IsActive);

        _anyModsActive = modActivated
            .ToCollection()
            .Select(collection => collection.Any(mod => mod.IsActive))
            .ToProperty(this, vm => vm.AnyModsActive);

        modActivated.Subscribe(changedMods => {
            foreach (var change in changedMods) {
                var changedMod = change.Item.Current;
                if (changedMod is { IsActive: true }) {
                    _mods?.Where(mod => mod != changedMod)
                        .ForEach(modItem => modItem.IsActive = false);
                }
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

        CloseAndLoadMods = ReactiveCommand.Create<Window>(window => {
            window.Close();
            LoadMods();
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

    private async void LoadMods() {
        _busyService.IsBusy = true;
        await Task.Run(() => {
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
        });
        _busyService.IsBusy = false;
    }
}
