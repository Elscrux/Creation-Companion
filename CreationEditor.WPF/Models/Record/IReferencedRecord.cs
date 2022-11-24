using System.Collections;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.Models.Record;

public interface IReferencedRecord {
    [Reactive] public IMajorRecordGetter Record { get; set; }
    [Reactive] public HashSet<IFormLinkIdentifier> References { get; set; }
}

public class ReferencedRecord<TMajorRecord, TMajorRecordGetter> : ReactiveObject, IReferencedRecord
    where TMajorRecord : IMajorRecordIdentifier
    where TMajorRecordGetter : IMajorRecordIdentifier {

    IMajorRecordGetter IReferencedRecord.Record {
        get => (IMajorRecordGetter) Record;
        set {
            if (value is TMajorRecordGetter tMajor) Record = tMajor;
        }
    }

    [Reactive] public TMajorRecordGetter Record { get; set; }
    public HashSet<IFormLinkIdentifier> References { get; set; }

    public ReferencedRecord(TMajorRecordGetter record, HashSet<IFormLinkIdentifier>? references = null) {
        Record = record;
        References = references ?? new HashSet<IFormLinkIdentifier>();
    }
}

public abstract class ReferencedComparer : IComparer<IReferencedRecord>, IComparer {
    public int Compare(object? x, object? y) {
        if (x is IReferencedRecord r1 && y is IReferencedRecord r2) {
            return Compare(r1, r2);
        }
        
        throw new ArgumentException($"Can't compare {x} and {y}, one of them is not IReferencedRecord");
    }
    
    public abstract int Compare(IReferencedRecord? x, IReferencedRecord? y);
}