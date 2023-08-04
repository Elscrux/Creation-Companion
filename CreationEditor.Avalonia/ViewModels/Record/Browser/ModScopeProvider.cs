using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public sealed class ModScopeProvider : ViewModel, IModScopeProvider {
    private readonly IEditorEnvironment _editorEnvironment;

    [Reactive] public ILinkCache LinkCache { get; private set; }
    [Reactive] public BrowserScope Scope { get; set; }

    private ReadOnlyObservableCollection<ModItem> Mods { get; }

    private readonly IObservableCache<IModGetter, ModKey> _selectedMods;
    public IEnumerable<ModKey> SelectedModKeys => _selectedMods.Keys;
    public IEnumerable<IModGetter> SelectedMods => _selectedMods.Items;

    public IObservable<Unit> ScopeChanged { get; }
    public IObservable<ILinkCache> LinkCacheChanged { get; }

    public ModScopeProvider(IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
        LinkCache = _editorEnvironment.LinkCache;

        Mods = this.WhenAnyValue(x => x.LinkCache)
            .Select(x => x.ListedOrder.AsObservableChangeSet())
            .Switch()
            .Transform(mod => new ModItem(mod.ModKey) { IsSelected = true })
            .ToObservableCollection(this);

        _selectedMods = Mods
            .ToObservableChangeSet()
            .AutoRefresh(mod => mod.IsSelected)
            .Filter(mod => mod.IsSelected)
            .Transform(x => {
                return LinkCache.ListedOrder.First(mod => mod.ModKey == x.ModKey);
            })
            .AddKey(x => x.ModKey)
            .AsObservableCache();

        this.WhenAnyValue(x => x.Scope)
            .CombineLatest(editorEnvironment.LoadOrderChanged)
            .ObserveOnGui()
            .Subscribe(UpdateScope)
            .DisposeWith(this);

        ScopeChanged = _selectedMods.Connect().Unit();
        LinkCacheChanged = this.WhenAnyValue(x => x.LinkCache);
    }

    private void UpdateScope() {
        LinkCache = Scope switch {
            BrowserScope.Environment => _editorEnvironment.LinkCache,
            BrowserScope.ActiveMod => _editorEnvironment.ActiveModLinkCache,
            _ => throw new ArgumentOutOfRangeException(nameof(Scope))
        };
    }
}
