using CreationEditor.Avalonia.Extension;
using CreationEditor.Environment;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Record.Browser;

public sealed class RecordBrowserSettings : ReactiveObject, IRecordBrowserSettings {
    private const char SplitChar = '*';
    
    private readonly IEditorEnvironment _editorEnvironment;

    [Reactive] public bool OnlyActive { get; set; } = false;
    [Reactive] public ILinkCache LinkCache { get; set; } = null!;
    [Reactive] public BrowserScope Scope { get; set; } = BrowserScope.Environment;
    [Reactive] public string SearchTerm { get; set; } = string.Empty;

    public RecordBrowserSettings(
        IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
        
        this.WhenAnyValue(x => x.OnlyActive)
            .ObserveOnGui()
            .Subscribe(_ => Scope = OnlyActive ? BrowserScope.ActiveMod : BrowserScope.Environment);

        this.WhenAnyValue(x => x.Scope)
            .ObserveOnGui()
            .Subscribe(_ => UpdateScope());

        editorEnvironment.LoadOrderChanged
            .ObserveOnGui()
            .Subscribe(_ => UpdateScope());
    }
    
    private void UpdateScope() {
        LinkCache = Scope switch {
            BrowserScope.Environment => _editorEnvironment.LinkCache,
            BrowserScope.ActiveMod => _editorEnvironment.ActiveModLinkCache,
            _ => throw new ArgumentOutOfRangeException(nameof(Scope))
        };
    }

    public bool Filter(IMajorRecordIdentifier record) {
        var editorID = record.EditorID;
    
        return SearchTerm.IsNullOrWhitespace() || editorID != null && SearchTerm.Split(SplitChar).All(term => editorID.Contains(term, StringComparison.OrdinalIgnoreCase));
    }
}
