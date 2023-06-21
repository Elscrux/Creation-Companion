using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record;

public interface IReferencedRecord : IReferencedRecordIdentifier {
    public new IMajorRecordGetter Record { get; set; }

    public string RecordTypeName { get; }
}

public interface IReferencedRecord<TMajorRecordGetter> : IReferencedRecord, IEquatable<IReferencedRecord<TMajorRecordGetter>>
    where TMajorRecordGetter : IMajorRecordIdentifier {

    public new TMajorRecordGetter Record { get; set; }

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

    FormKey IFormKeyGetter.FormKey => Record.FormKey;
    System.Type ILinkIdentifier.Type => LoquiRegistration.GetRegister(Record.GetType()).GetterType;
    string IReferencedRecord.RecordTypeName => (this as IReferencedRecord).Record.Registration.Name;

    bool IEquatable<IReferencedRecord<TMajorRecordGetter>>.Equals(IReferencedRecord<TMajorRecordGetter>? other) {
        return other is not null && other.Record.FormKey.Equals(Record.FormKey);
    }
}
