using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Filter;
using CreationEditor.Services.Mutagen.Mod;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public sealed partial class ModScopeProvider : ViewModel, IModScopeProvider {
    private readonly IEditorEnvironment _editorEnvironment;

    [Reactive] public partial ILinkCache LinkCache { get; private set; }
    [Reactive] public partial BrowserScope Scope { get; set; }

    public ReadOnlyObservableCollection<ISelectableModKey> Mods { get; }

    private readonly IObservableCache<IModGetter, ModKey> _selectedMods;
    public IEnumerable<ModKey> SelectedModKeys => _selectedMods.Keys;
    public IEnumerable<IModGetter> SelectedMods => _selectedMods.Items;

    public IObservable<Unit> ScopeChanged { get; }
    public IObservable<ILinkCache> LinkCacheChanged { get; }

    public ModScopeProvider(IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
        LinkCache = _editorEnvironment.LinkCache;

        LinkCacheChanged = this.WhenAnyValue(x => x.LinkCache);

        var modsChangeSet = LinkCacheChanged
            .Select(x => x.ListedOrder.AsObservableChangeSet())
            .Switch()
            .Transform(mod => new ModItem(mod.ModKey) { IsSelected = true });

        Mods = modsChangeSet
            .Transform(ISelectableModKey (modItem) => modItem)
            .ToObservableCollection(this);

        _selectedMods = modsChangeSet
            .AutoRefresh(mod => mod.IsSelected)
            .Filter(mod => mod.IsSelected)
            .Transform(x => LinkCache.GetMod(x.ModKey))
            .AddKey(x => x.ModKey)
            .AsObservableCache();

        this.WhenAnyValue(x => x.Scope)
            .CombineLatest(editorEnvironment.LoadOrderChanged)
            .ObserveOnGui()
            .Subscribe(UpdateScope)
            .DisposeWith(this);

        ScopeChanged = _selectedMods.Connect().Unit();
    }

    private void UpdateScope() {
        LinkCache = Scope switch {
            BrowserScope.Environment => _editorEnvironment.LinkCache,
            BrowserScope.ActiveMod => _editorEnvironment.ActiveModLinkCache,
            _ => throw new InvalidOperationException(),
        };
    }
}
