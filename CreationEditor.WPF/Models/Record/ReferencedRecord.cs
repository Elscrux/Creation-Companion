using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.Models.Record;

public class ReferencedRecord<TMajorRecord, TMajorRecordGetter> : ReactiveObject
    where TMajorRecord : IMajorRecordIdentifier
    where TMajorRecordGetter : IMajorRecordIdentifier {
    
    [Reactive] public TMajorRecordGetter Record { get; set; }
    [Reactive] public HashSet<IFormLinkIdentifier> References { get; set; }
    
    public ReferencedRecord(TMajorRecordGetter record, HashSet<IFormLinkIdentifier>? references = null) {
        Record = record;
        References = references ?? new HashSet<IFormLinkIdentifier>();
    }
}
