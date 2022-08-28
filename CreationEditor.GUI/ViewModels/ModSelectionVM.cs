using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CreationEditor.GUI.Models;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Skyrim;
using MutagenLibrary.Notification;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.ViewModels;

public class ModSelectionVM : ViewModel {
    private readonly IGameEnvironment _environment;

    [Reactive] public ObservableCollection<ActivatableModItem> Mods { get; set; }
    public readonly Dictionary<ModKey, (HashSet<ModKey> Masters, bool Valid)> MasterInfos = new();

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

    public ModSelectionVM(GameRelease gameRelease, INotifier? notifier = null) {
        _environment = GameEnvironment.Typical.Construct(gameRelease, LinkCachePreferences.OnlyIdentifiers());

        var pathProvider = new PluginListingsPathProvider(new GameReleaseInjection(Constants.GameRelease));
        if (!File.Exists(pathProvider.Path)) MessageBox.Show($"Make sure {pathProvider.Path} exists.");

        // if (!GameLocations.TryGetDataFolder(gameRelease, out var dataDirectory)) {
        //     MessageBox.Show($"Could not detect {gameRelease} game directory.");
        // }
        // var loadOrder = LoadOrder.GetLoadOrderListings(gameRelease, dataDirectory, false);

        UpdateMasterInfos();
        Mods = new ObservableCollection<ActivatableModItem>(_environment.LoadOrder.Keys.Select(modKey => new ActivatableModItem(modKey, MasterInfos[modKey].Valid, MasterInfos[modKey].Masters)));

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

            MainVM.Instance.IsLoading = true;
            await Task.Run(() => {
                //Load all mods that are selected, or masters of selected mods
                var loadedMods = new HashSet<ModKey>();
                var missingMods = new Queue<ModKey>(SelectedMods);
                var modKeys = _environment.LoadOrder.Keys.ToHashSet();
                
                while (missingMods.Any()) {
                    var modKey = missingMods.Dequeue();
                    loadedMods.Add(modKey);

                    foreach (var master in MasterInfos[modKey].Masters.Where(masterMod => !loadedMods.Contains(masterMod))) {
                        missingMods.Enqueue(master);
                    }
                }

                var orderedMods = loadedMods.OrderBy(key => modKeys.IndexOf(key));
                Editor.Build(orderedMods, ActiveMod, notifier);
            });
            MainVM.Instance.IsLoading = false;
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
        MasterInfos.Clear();
        var modKeys = _environment.LoadOrder.Keys.ToHashSet();

        foreach (var listing in _environment.LoadOrder.Items) {
            if (listing.Mod == null) {
                MasterInfos.Add(listing.ModKey, (new HashSet<ModKey>(), false));
            } else {
                var directMasters = listing.Mod.MasterReferences.Select(m => m.Master).ToList();
                var masters = new HashSet<ModKey>(directMasters);
                var valid = true;
                
                //Check that all masters are valid and compile list of all recursive masters
                foreach (var master in directMasters) {
                    if (MasterInfos.TryGetValue(master, out var masterInfo)) {
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
                MasterInfos.Add(listing.ModKey, (masters, valid));
            }
        }
    }
}
