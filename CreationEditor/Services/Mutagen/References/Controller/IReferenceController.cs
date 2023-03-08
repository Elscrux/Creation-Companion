using CreationEditor.Services.Mutagen.References.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Controller;

public interface IReferenceController {
    public IReferenceCache? ReferenceCache { get; }

    public IDisposable GetRecord<TMajorRecordGetter>(TMajorRecordGetter record, out ReferencedRecord<TMajorRecordGetter> outReferencedRecord)
        where TMajorRecordGetter : IMajorRecordIdentifier;

    public void UpdateReferences<TMajorRecordGetter>(TMajorRecordGetter record, Action updateAction)
        where TMajorRecordGetter : IMajorRecordGetter;
}
