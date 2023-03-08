using System.Collections.ObjectModel;
using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Services.Mutagen.References;

public class ReferencedRecord<TMajorRecordGetter> : ReactiveObject, IReferencedRecord
    where TMajorRecordGetter : IMajorRecordIdentifier {

    IMajorRecordGetter IReferencedRecord.Record {
        get => (IMajorRecordGetter) Record;
        set {
            if (value is TMajorRecordGetter tMajor) Record = tMajor;
        }
    }

    IMajorRecordIdentifier IReferencedRecordIdentifier.Record {
        get => Record;
        set {
            if (value is TMajorRecordGetter tMajor) Record = tMajor;
        }
    }

    public FormKey FormKey => Record.FormKey;
    public System.Type Type => LoquiRegistration.GetRegister(Record.GetType()).GetterType;

    public string RecordTypeName => (this as IReferencedRecord).Record.Registration.Name;

    public override bool Equals(object? obj) {
        return obj switch {
            ReferencedRecord<TMajorRecordGetter> referencedRecord => referencedRecord.Equals(this),
            TMajorRecordGetter record => record.FormKey.Equals(Record.FormKey),
            _ => false
        };
    }

    private bool Equals(ReferencedRecord<TMajorRecordGetter> other) {
        return other.Record.FormKey.Equals(Record.FormKey);
    }

    public override int GetHashCode() {
        // While the record can change, its form key will always stay the same
        return HashCode.Combine(Record.FormKey);
    }

    [Reactive] public TMajorRecordGetter Record { get; set; }

    ICollection<IFormLinkIdentifier> IReferenced.References => References;
    public ObservableCollection<IFormLinkIdentifier> References { get; }

    public ReferencedRecord(TMajorRecordGetter record, IEnumerable<IFormLinkIdentifier>? references = null) {
        Record = record;
        References = references == null
            ? new ObservableCollection<IFormLinkIdentifier>()
            : new ObservableCollection<IFormLinkIdentifier>(references);
    }
}
