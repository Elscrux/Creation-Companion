using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Services.Filter;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public sealed class RecordBrowserSettings : ViewModel, IRecordBrowserSettings {
    public ISearchFilter SearchFilter { get; }
    public IModScopeProvider ModScopeProvider { get; }

    [Reactive] public string SearchTerm { get; set; } = string.Empty;
    [Reactive] public Func<IMajorRecordGetter, bool>? CustomFilter { get; set; }

    public IObservable<Unit> SettingsChanged { get; }

    public RecordBrowserSettings(
        ISearchFilter searchFilter,
        IModScopeProvider modScopeProvider) {
        SearchFilter = searchFilter;
        ModScopeProvider = modScopeProvider;

        SettingsChanged = Observable.Merge(
            ModScopeProvider.ScopeChanged.Unit(),
            this.WhenAnyValue(x => x.CustomFilter).Unit(),
            this.WhenAnyValue(x => x.SearchTerm).Unit());
    }

    public bool Filter(IMajorRecordGetter record) {
        if (record.IsDeleted) return false;

        return Filter(record as IMajorRecordIdentifier)
         && (CustomFilter is null || CustomFilter(record));
    }

    public bool Filter(IMajorRecordIdentifier record) {
        if (!ModScopeProvider.SelectedModKeys.Contains(record.FormKey.ModKey)) return false;
        if (SearchTerm.IsNullOrEmpty()) return true;

        var editorID = record.EditorID;
        return (!editorID.IsNullOrEmpty() && SearchFilter.Filter(editorID, SearchTerm)) || SearchFilter.Filter(record.FormKey.ToString(), SearchTerm);
    }
}
