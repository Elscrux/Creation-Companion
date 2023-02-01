using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Extension;
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

public sealed class RecordBrowserSettingsVM : ReactiveObject, IRecordBrowserSettingsVM {
    private const char SplitChar = '*';
    
    private readonly IEditorEnvironment _editorEnvironment;

    private readonly ReplaySubject<Unit> _settingsChanged = new();
    public IObservable<Unit> SettingsChanged => _settingsChanged;
    
    [Reactive] public bool OnlyActive { get; set; } = false;
    [Reactive] public ILinkCache LinkCache { get; set; }
    [Reactive] public BrowserScope Scope { get; set; } = BrowserScope.Environment;
    [Reactive] public string SearchTerm { get; set; } = string.Empty;
    public ObservableCollection<ModItem> Mods { get; }
    private List<ModKey> _selectedMods;

    public RecordBrowserSettingsVM(
        IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
        LinkCache = _editorEnvironment.LinkCache;
        Mods = new ObservableCollection<ModItem>(LinkCache.ListedOrder.Select(mod => new ModItem(mod.ModKey) { IsSelected = true }));
        _selectedMods = Mods.Select(mod => mod.ModKey).ToList();

        this.WhenAnyValue(x => x.OnlyActive)
            .ObserveOnGui()
            .Subscribe(_ => Scope = OnlyActive ? BrowserScope.ActiveMod : BrowserScope.Environment);

        this.WhenAnyValue(x => x.Scope)
            .CombineLatest(editorEnvironment.LoadOrderChanged)
            .ObserveOnGui()
            .Subscribe(_ => UpdateScope());

        Mods.ToObservableChangeSet()
            .AutoRefresh(mod => mod.IsSelected)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .Subscribe(_ => {
                _selectedMods = Mods
                    .Where(mod => mod.IsSelected)
                    .Select(mod => mod.ModKey)
                    .ToList();
                
                RequestUpdate();
            });
        
        this.WhenAnyValue(x => x.SearchTerm)
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .Subscribe(RequestUpdate);
    }
    
    private void UpdateScope() {
        LinkCache = Scope switch {
            BrowserScope.Environment => _editorEnvironment.LinkCache,
            BrowserScope.ActiveMod => _editorEnvironment.ActiveModLinkCache,
            _ => throw new ArgumentOutOfRangeException(nameof(Scope))
        };

        Mods.Clear();
        foreach (var modItem in LinkCache.ListedOrder.Select(x => new ModItem(x.ModKey) { IsSelected = true })) {
            Mods.Add(modItem);
        }
    }

    public void RequestUpdate() => _settingsChanged.OnNext(Unit.Default);

    public bool Filter(IMajorRecordGetter record) {
        if (record.IsDeleted) return false;

        return Filter(record as IMajorRecordIdentifier);
    }
    
    public bool Filter(IMajorRecordIdentifier record) {
        if (!_selectedMods.Contains(record.FormKey.ModKey)) return false;
        if (SearchTerm.IsNullOrEmpty()) return true;

        var editorID = record.EditorID;

        return !editorID.IsNullOrEmpty()
         && SearchTerm
                .Split(SplitChar)
                .All(term => editorID.Contains(term, StringComparison.OrdinalIgnoreCase));
    }
}
 