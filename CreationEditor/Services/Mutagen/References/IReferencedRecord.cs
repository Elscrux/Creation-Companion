using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References;

public interface IReferencedRecord : IReferencedRecordIdentifier {
    new IMajorRecordGetter Record { get; set; }

    string RecordTypeName { get; }
}

public interface IReferencedRecord<TMajorRecordGetter> : IReferencedRecord, IEquatable<IReferencedRecord<TMajorRecordGetter>>
    where TMajorRecordGetter : IMajorRecordIdentifierGetter {

    new TMajorRecordGetter Record { get; set; }

    IMajorRecordGetter IReferencedRecord.Record {
        get => (IMajorRecordGetter) Record;
        set {
            if (value is TMajorRecordGetter tMajor) Record = tMajor;
        }
    }

    IMajorRecordIdentifierGetter IReferencedRecordIdentifier.Record {
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
