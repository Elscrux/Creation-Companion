using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CreationEditor.GUI.Models.Mod;
using CreationEditor.GUI.Services;
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using Elscrux.Notification;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.Core.Plugins;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.ViewModels.Mod;

public class ModSelectionVM : ViewModel {
    private readonly INotifier _notifier;
    private readonly ISimpleEnvironmentContext _simpleEnvironmentContext;
    private readonly IEditorEnvironment _editorEnvironment;
    public IBusyService BusyService { get; }
    private readonly IGameEnvironment _environment;

    [Reactive] public ObservableCollection<ActivatableModItem> Mods { get; set; }
    private readonly Dictionary<ModKey, (HashSet<ModKey> Masters, bool Valid)> _masterInfos = new();

    [Reactive] public ActivatableModItem? SelectedMod { get; set; }
    [Reactive] public IModGetterVM? SelectedModDetails { get; private set; }

    public ModKey? ActiveMod => Mods.FirstOrDefault(x => x.IsActive)?.ModKey;
    public IEnumerable<ModKey> SelectedMods => Mods.Where(mod => mod.IsSelected).Select(x => x.ModKey);
    [Reactive] public bool NoActiveMod { get; set; } = true;

    private readonly ObservableAsPropertyHelper<bool> _anyModsLoaded;
    public bool AnyModsLoaded => _anyModsLoaded.Value;

    public ICommand Confirm { get; }
    public ICommand SetAsActive { get; }
    public Func<ISelectable, bool> CanSelect { get; } = selectable => selectable is ActivatableModItem { MastersValid: true };

    public ModSelectionVM(
        INotifier notifier,
        ISimpleEnvironmentContext simpleEnvironmentContext,
        IEditorEnvironment editorEnvironment,
        IBusyService busyService) {
        _notifier = notifier;
        _simpleEnvironmentContext = simpleEnvironmentContext;
        _editorEnvironment = editorEnvironment;
        BusyService = busyService;

        _environment = GameEnvironment.Typical.Construct(_simpleEnvironmentContext.GameReleaseContext.Release, LinkCachePreferences.OnlyIdentifiers());

        var pathProvider = new PluginListingsPathProvider(new GameReleaseInjection(_simpleEnvironmentContext.GameReleaseContext.Release));
        if (!File.Exists(pathProvider.Path)) MessageBox.Show($"Make sure {pathProvider.Path} exists.");

        UpdateMasterInfos();
        Mods = new ObservableCollection<ActivatableModItem>(_environment.LoadOrder.Keys.Select(modKey => new ActivatableModItem(modKey, _masterInfos[modKey].Valid, _masterInfos[modKey].Masters)));

        _anyModsLoaded = Mods.ToObservableChangeSet()
            .AutoRefresh(x => x.IsSelected)
            .ToCollection()
            .Select(x => x.Any(mod => mod.IsSelected))
            .ToProperty(this, x => x.AnyModsLoaded);

        SetAsActive = ReactiveCommand.Create(
            canExecute: this
                .WhenAnyValue(x => x.SelectedMod)
                .NotNull()
                .Select(mod => mod.MastersValid),
            execute: () => {
                if (SelectedMod == null) return;

                NoActiveMod = SelectedMod.IsActive;
                if (NoActiveMod) {
                    SelectedMod.IsActive = false;
                } else {
                    Mods.ForEach(modItem => modItem.IsActive = false);
                    SelectedMod.IsActive = true;
                }
            });

        Confirm = ReactiveCommand.Create(async (Window window) => {
            window.Close();

            BusyService.IsBusy = true;
            await Task.Run(async () => {
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
            .Subscribe(_ => {
                if (SelectedMod == null) return;

                var mod = _environment.LoadOrder.First(l => l.Key == SelectedMod.ModKey).Value.Mod;
                if (mod == null) return;

                SelectedModDetails = mod switch {
                    ISkyrimModGetter skyrimMod => new SkyrimModGetterVM(mod.ModKey, skyrimMod.ModHeader),
                    _ => throw new ArgumentOutOfRangeException(nameof(mod))
                };
            });
    }
    
    public void UpdateMasterInfos() {
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
