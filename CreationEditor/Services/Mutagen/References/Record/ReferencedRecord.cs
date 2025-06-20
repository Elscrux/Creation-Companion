using DynamicData.Binding;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Services.Mutagen.References.Record;

public sealed partial class ReferencedRecord<TMajorRecordGetter> : ReactiveObject, IReferencedRecord<TMajorRecordGetter>
    where TMajorRecordGetter : IMajorRecordIdentifierGetter {

    [Reactive] public partial TMajorRecordGetter Record { get; set; }

    public IObservableCollection<DataRelativePath> AssetReferences { get; } = new ObservableCollectionExtended<DataRelativePath>();
    public IObservableCollection<IFormLinkIdentifier> RecordReferences { get; }

    public ReferencedRecord(
        TMajorRecordGetter record,
        IEnumerable<IFormLinkIdentifier>? references = null) {
        Record = record;
        RecordReferences = references is null
            ? []
            : new ObservableCollectionExtended<IFormLinkIdentifier>(references);
    }

    public override bool Equals(object? obj) {
        return obj switch {
            IReferencedRecord<TMajorRecordGetter> referencedRecord => referencedRecord.Equals(this),
            TMajorRecordGetter record => record.FormKey.Equals(Record.FormKey),
            IFormKeyGetter formKeyGetter => formKeyGetter.FormKey.Equals(Record.FormKey),
            _ => false,
        };
    }

    public override int GetHashCode() {
        // While the record can change, its form key will always stay the same
        return HashCode.Combine(Record.FormKey);
    }
}
