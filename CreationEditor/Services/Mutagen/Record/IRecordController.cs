using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Record;

public interface IRecordController {
    TMajorRecord CreateRecord<TMajorRecord, TMajorRecordGetter>()
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter;

    TMajorRecord DuplicateRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter;

    IMajorRecord DuplicateRecord(IMajorRecordGetter record);
    
    void DeleteRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter;

    void DeleteRecord(IMajorRecordGetter record);

    TMajorRecord GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter;

    IMajorRecord GetOrAddOverride(IMajorRecordGetter record);

    void RegisterUpdate(IMajorRecordGetter record, Action updateAction);

    IObservable<IMajorRecordGetter> RecordChanged { get; }
    JoinedObservable<IMajorRecordGetter> RecordChangedDiff { get; }
    IObservable<IMajorRecordGetter> RecordCreated { get; }
    IObservable<IMajorRecordGetter> RecordDeleted { get; }
}
