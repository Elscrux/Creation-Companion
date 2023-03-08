using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Services.Environment;
using DynamicData;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public sealed class RecordBrowserSettingsVM : ViewModel, IRecordBrowserSettingsVM {
    private const char SplitChar = '*';

    private readonly IEditorEnvironment _editorEnvironment;

    public IObservable<Unit> SettingsChanged { get; }
    [Reactive] public Func<IMajorRecordGetter, bool>? RecordFilter { get; set; }

    [Reactive] public bool OnlyActive { get; set; } = false;
    [Reactive] public ILinkCache LinkCache { get; set; }
    [Reactive] public BrowserScope Scope { get; set; } = BrowserScope.Environment;
    [Reactive] public string SearchTerm { get; set; } = string.Empty;

    public ReadOnlyObservableCollection<ModItem> Mods { get; }
    private readonly IObservableCache<ModKey, ModKey> _selectedMods;

    public RecordBrowserSettingsVM(
        IEditorEnvironment editorEnvironment) {
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
            .Transform(x => x.ModKey)
            .AddKey(x => x)
            .AsObservableCache();

        this.WhenAnyValue(x => x.OnlyActive)
            .ObserveOnGui()
            .Subscribe(_ => Scope = OnlyActive ? BrowserScope.ActiveMod : BrowserScope.Environment);

        this.WhenAnyValue(x => x.Scope)
            .CombineLatest(editorEnvironment.LoadOrderChanged)
            .ObserveOnGui()
            .Subscribe(_ => UpdateScope());

        SettingsChanged = Observable.Merge(
                this.WhenAnyValue(x => x.RecordFilter).Unit(),
                this.WhenAnyValue(x => x.SearchTerm).Unit(),
                _selectedMods.Connect().Unit())
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler);
    }

    private void UpdateScope() {
        LinkCache = Scope switch {
            BrowserScope.Environment => _editorEnvironment.LinkCache,
            BrowserScope.ActiveMod => _editorEnvironment.ActiveModLinkCache,
            _ => throw new ArgumentOutOfRangeException(nameof(Scope))
        };
    }

    public bool Filter(IMajorRecordGetter record) {
        if (record.IsDeleted) return false;

        return Filter(record as IMajorRecordIdentifier)
         && (RecordFilter == null || RecordFilter(record));
    }

    public bool Filter(IMajorRecordIdentifier record) {
        if (!_selectedMods.Lookup(record.FormKey.ModKey).HasValue) return false;
        if (SearchTerm.IsNullOrEmpty()) return true;

        var editorID = record.EditorID;

        return !editorID.IsNullOrEmpty()
         && SearchTerm
                .Split(SplitChar)
                .All(term => editorID.Contains(term, StringComparison.OrdinalIgnoreCase));
    }
}
