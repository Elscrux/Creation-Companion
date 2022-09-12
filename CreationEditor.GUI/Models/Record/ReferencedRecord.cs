using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.GUI.Models.Record;

public record ReferencedRecord<TMajorRecord, TMajorRecordGetter> where TMajorRecord : IMajorRecordIdentifier
    where TMajorRecordGetter : IMajorRecordIdentifier {
    
    public TMajorRecordGetter Record { get; init; }
    public HashSet<IFormLinkIdentifier> References { get; init; }
    
    public ReferencedRecord(TMajorRecordGetter record, HashSet<IFormLinkIdentifier>? references = null) {
        Record = record;
        References = references ?? new HashSet<IFormLinkIdentifier>();
    }
}
