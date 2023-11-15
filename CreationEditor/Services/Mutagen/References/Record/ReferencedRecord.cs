using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Mutagen.References.Record;

public sealed class ReferencedRecord<TMajorRecordGetter>(
    TMajorRecordGetter record,
    IEnumerable<IFormLinkIdentifier>? references = null)
    : ReactiveObject, IReferencedRecord<TMajorRecordGetter>
    where TMajorRecordGetter : IMajorRecordIdentifier {

    public override bool Equals(object? obj) {
        return obj switch {
            IReferencedRecord<TMajorRecordGetter> referencedRecord => referencedRecord.Equals(this),
            TMajorRecordGetter record => record.FormKey.Equals(Record.FormKey),
            _ => false
        };
    }

    public override int GetHashCode() {
        // While the record can change, its form key will always stay the same
        return HashCode.Combine(Record.FormKey);
    }

    [Reactive] public TMajorRecordGetter Record { get; set; } = record;

    public IObservableCollection<IFormLinkIdentifier> References { get; } = references is null
        ? []
        : new ObservableCollectionExtended<IFormLinkIdentifier>(references);

}
