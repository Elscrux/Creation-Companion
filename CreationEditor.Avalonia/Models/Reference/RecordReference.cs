using System.Collections.ObjectModel;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Models.Reference;

public sealed class RecordReference : IReference, IDisposable {
    private readonly IDisposableBucket _disposables = new DisposableBucket();

    private IMajorRecordGetter? _record;
    public IMajorRecordGetter Record => _record ??= _editorEnvironment.LinkCache.TryResolve(_formLinkIdentifier, out var record) ? record : throw new ArgumentException();

    private IReferencedRecord? _referencedRecord;
    public IReferencedRecord ReferencedRecord {
        get {
            if (_referencedRecord == null) {
                _recordReferenceController
                    .GetReferencedRecord(Record, out var referencedRecord)
                    .DisposeWith(_disposables);

                _referencedRecord = referencedRecord;
            }
            return _referencedRecord;
        }
    }

    private readonly IFormLinkIdentifier _formLinkIdentifier;
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IRecordReferenceController _recordReferenceController;

    public string Name => Record.EditorID ?? string.Empty;
    public string Identifier => Record.FormKey.ToString();
    public string Type => Record.Registration.Name;

    private ReadOnlyObservableCollection<IReference>? _children;
    public ReadOnlyObservableCollection<IReference> Children => _children ??= LoadChildren();
    private ReadOnlyObservableCollection<IReference> LoadChildren() {
        return ReferencedRecord.References
            .SelectObservableCollectionSync(
                identifier => new RecordReference(identifier, _editorEnvironment, _recordReferenceController) as IReference,
                _disposables);
    }

    public bool HasChildren => _children is not null ? _children.Count > 0 : ReferencedRecord.References.Count > 0;

    public RecordReference(
        IFormLinkIdentifier formLinkIdentifier,
        IEditorEnvironment editorEnvironment,
        IRecordReferenceController recordReferenceController) {
        _formLinkIdentifier = formLinkIdentifier;
        _editorEnvironment = editorEnvironment;
        _recordReferenceController = recordReferenceController;
    }

    public void Dispose() => _disposables.Dispose();
}
