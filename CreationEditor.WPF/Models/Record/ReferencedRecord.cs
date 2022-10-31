using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog.WPF;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.Models.Record;

public class ReferencedRecord<TMajorRecord, TMajorRecordGetter> : ViewModel
    where TMajorRecord : IMajorRecordIdentifier
    where TMajorRecordGetter : IMajorRecordIdentifier {
    
    [Reactive] public TMajorRecordGetter Record { get; init; }
    [Reactive] public HashSet<IFormLinkIdentifier> References { get; init; }
    
    public ReferencedRecord(TMajorRecordGetter record, HashSet<IFormLinkIdentifier>? references = null) {
        Record = record;
        References = references ?? new HashSet<IFormLinkIdentifier>();
    }
}
