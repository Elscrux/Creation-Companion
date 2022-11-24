using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using CreationEditor.Environment;
using CreationEditor.WPF.Models.Mod;
using CreationEditor.WPF.Services;
using DynamicData;
using DynamicData.Binding;
using Elscrux.Notification;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order.DI;
using MutagenLibrary.Core.Plugins;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ISelectable = Noggog.ISelectable;
namespace CreationEditor.WPF.ViewModels.Mod;

public class ModSelectionVM : ViewModel {
    private readonly INotifier _notifier;
    private readonly ISimpleEnvironmentContext _simpleEnvironmentContext;
    private readonly IEditorEnvironment _editorEnvironment;
    public IBusyService BusyService { get; }
    private readonly IGameEnvironment _environment;

    [Reactive] public ObservableCollection<ActivatableModItem> Mods { get; set; }
    private readonly Dictionary<ModKey, (HashSet<ModKey> Masters, bool Valid)> _masterInfos = new();

    [Reactive] public ActivatableModItem? SelectedMod { get; set; }
    [Reactive] public IModGetterVM SelectedModDetails { get; init; }

    public ModKey? ActiveMod => Mods.FirstOrDefault(x => x.IsActive)?.ModKey;
    public IEnumerable<ModKey> SelectedMods => Mods.Where(mod => mod.IsSelected).Select(x => x.ModKey);

    private readonly ObservableAsPropertyHelper<bool> _anyModsLoaded;
    public bool AnyModsLoaded => _anyModsLoaded.Value;

    private readonly ObservableAsPropertyHelper<bool> _anyModsActive;
    public bool AnyModsActive => _anyModsActive.Value;

    public ICommand Confirm { get; }
    public ICommand ToggleActive { get; }
    public Func<ISelectable, bool> CanSelect { get; } = selectable => selectable is ActivatableModItem { MastersValid: true };

    public ModSelectionVM(
        INotifier notifier,
        ISimpleEnvironmentContext simpleEnvironmentContext,
        IEditorEnvironment editorEnvironment,
        IFileSystem fileSystem,
        IModGetterVM modGetterVM,
        IPluginListingsPathProvider pluginListingsProvider,
        IBusyService busyService) {
        _notifier = notifier;
        _simpleEnvironmentContext = simpleEnvironmentContext;
        _editorEnvironment = editorEnvironment;
        SelectedModDetails = modGetterVM;
        BusyService = busyService;

        _environment = GameEnvironment.Typical.Construct(_simpleEnvironmentContext.GameReleaseContext.Release, LinkCachePreferences.OnlyIdentifiers());


        var filePath = pluginListingsProvider.Get(simpleEnvironmentContext.GameReleaseContext.Release, GameInstallMode.Steam);
        if (!fileSystem.File.Exists(filePath)) MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Warning", $"Make sure {filePath} exists.");

        UpdateMasterInfos();
        Mods = new ObservableCollection<ActivatableModItem>(_environment.LoadOrder.Keys.Select(modKey => new ActivatableModItem(modKey, _masterInfos[modKey].Valid)));

        var observableMods = Mods.ToObservableChangeSet();

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
                    Mods?.Where(mod => mod != changedMod)
                        .ForEach(modItem => modItem.IsActive = false);
                }
            }
        });

        ToggleActive = ReactiveCommand.Create(
            canExecute: this
                .WhenAnyValue(x => x.SelectedMod)
                .NotNull()
                .Select(mod => mod.MastersValid),
            execute: () => {
                if (SelectedMod == null) return;
                SelectedMod.IsActive = !SelectedMod.IsActive;
            });

        Confirm = ReactiveCommand.Create<Window>(async window => {
            window.Close();

            BusyService.IsBusy = true;
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
            BusyService.IsBusy = false;
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
}
