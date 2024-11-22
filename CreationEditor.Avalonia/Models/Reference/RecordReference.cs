using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
namespace CreationEditor.Avalonia.Models.Reference;

public sealed class RecordReference(
    IFormLinkIdentifier formLinkIdentifier,
    ILinkCacheProvider linkCacheProvider,
    IRecordReferenceController recordReferenceController)
    : IReference, IDisposable {

    private readonly DisposableBucket _disposables = new();

    [field: MaybeNull]
    public IMajorRecordGetter Record {
        get {
            return field ??= linkCacheProvider.LinkCache.TryResolve(formLinkIdentifier, out var record)
                ? record
                : throw new ArgumentException(nameof(formLinkIdentifier));
        }
    }

    [field: AllowNull, MaybeNull]
    public IReferencedRecord ReferencedRecord {
        get {
            if (field is null) {
                recordReferenceController
                    .GetReferencedRecord(Record, out var referencedRecord)
                    .DisposeWith(_disposables);

                field = referencedRecord;
            }
            return field;
        }
    }

    public string Name => Record.EditorID ?? string.Empty;
    public string Identifier => formLinkIdentifier.FormKey.ToString();
    public string Type => formLinkIdentifier.Type.Name;

    private ReadOnlyObservableCollection<IReference>? _children;
    public ReadOnlyObservableCollection<IReference> Children => _children ??= LoadChildren();
    private ReadOnlyObservableCollection<IReference> LoadChildren() {
        return ReferencedRecord.References
            .SelectObservableCollectionSync(
                identifier => new RecordReference(identifier, linkCacheProvider, recordReferenceController) as IReference,
                _disposables);
    }

    public bool HasChildren => _children is not null ? _children.Count > 0 : ReferencedRecord.References.Count > 0;

    public void Dispose() => _disposables.Dispose();
}
