using System.Reactive;
using System.Reactive.Linq;
using CreationEditor.Services.Filter;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public sealed partial class RecordBrowserSettings : ViewModel, IRecordBrowserSettings {
    public ISearchFilter SearchFilter { get; }

    public IModScopeProviderVM ModScopeProviderVM { get; }
    public IModScopeProvider ModScopeProvider => ModScopeProviderVM;

    [Reactive] public partial string SearchTerm { get; set; }
    [Reactive] public partial Func<IMajorRecordGetter, bool>? CustomFilter { get; set; }

    public IObservable<Unit> SettingsChanged { get; }

    public RecordBrowserSettings(
        ISearchFilter searchFilter,
        IModScopeProviderVM modScopeProviderVM) {
        ModScopeProviderVM = modScopeProviderVM;
        SearchFilter = searchFilter;

        SearchTerm = string.Empty;
        SettingsChanged = Observable.Merge(
            ModScopeProviderVM.ScopeChanged.Unit(),
            this.WhenAnyValue(x => x.CustomFilter).Unit(),
            this.WhenAnyValue(x => x.SearchTerm).Unit());
    }

    public bool Filter(IMajorRecordGetter record) {
        if (record.IsDeleted) return false;

        return Filter(record as IMajorRecordIdentifierGetter)
         && (CustomFilter is null || CustomFilter(record));
    }

    public bool Filter(IMajorRecordIdentifierGetter record) {
        if (!ModScopeProviderVM.SelectedModKeys.Contains(record.FormKey.ModKey)) return false;
        if (SearchTerm.IsNullOrEmpty()) return true;

        var editorID = record.EditorID;
        return (!editorID.IsNullOrEmpty() && SearchFilter.Filter(editorID, SearchTerm)) || SearchFilter.Filter(record.FormKey.ToString(), SearchTerm);
    }
}
