using CreationEditor.Services.Mutagen.References.Record;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.Record;

public interface IRecordController {
    /// <summary>
    /// Create a record of the given type in the active mod.
    /// </summary>
    /// <param name="type">Getter type of record</param>
    /// <returns>Created record</returns>
    IMajorRecord CreateRecord(System.Type type);

    /// <summary>
    /// Create a record of type <typeparamref name="TMajorRecord"/> in the active mod.
    /// </summary>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    /// <returns>Created record</returns>
    TMajorRecord CreateRecord<TMajorRecord, TMajorRecordGetter>()
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter;

    /// <summary>
    /// Create a duplicate record the given record in the active mod.
    /// </summary>
    /// <param name="record">Record to duplicate</param>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    /// <returns>Duplicated record</returns>
    TMajorRecord DuplicateRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter
        where TMajorRecordGetter : class, IMajorRecordGetter;

    /// <summary>
    /// Create a duplicate record the given record in the active mod.
    /// </summary>
    /// <param name="record">Record to duplicate</param>
    /// <returns>Duplicated record</returns>
    IMajorRecord DuplicateRecord(IMajorRecordGetter record);

    /// <summary>
    /// Delete the given record in the active mod.
    /// If the record was defined in another plugin, this will not delete it there, but delete the override in the active mod. 
    /// </summary>
    /// <param name="record">Record to delete</param>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    void DeleteRecord<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter;

    /// <summary>
    /// Delete the given record in the active mod.
    /// If the record was defined in another mod, this will not delete it there, but delete the override in the active mod. 
    /// </summary>
    /// <param name="record">Record to delete</param>
    void DeleteRecord(IMajorRecordGetter record);

    /// <summary>
    /// Creates an override of an existing record in the active mod if it doesn't exist yet.
    /// </summary>
    /// <param name="record">Record to get an override for</param>
    /// <typeparam name="TMajorRecord">Setter record type</typeparam>
    /// <typeparam name="TMajorRecordGetter">Getter record type</typeparam>
    /// <returns>Overwrite of the given record in the active mod</returns>
    TMajorRecord GetOrAddOverride<TMajorRecord, TMajorRecordGetter>(TMajorRecordGetter record)
        where TMajorRecord : class, IMajorRecord, TMajorRecordGetter, IMajorRecordQueryable
        where TMajorRecordGetter : class, IMajorRecordGetter;

    /// <summary>
    /// Creates an override of an existing record in the active mod if it doesn't exist yet.
    /// </summary>
    /// <param name="record">Record to get an override for</param>
    /// <returns>Overwrite of the given record in the active mod</returns>
    IMajorRecord GetOrAddOverride(IMajorRecordGetter record);

    /// <summary>
    /// Registers updates to a given record. The update must be executed in the passed action.
    /// </summary>
    /// <param name="record">Record which updates are registered for</param>
    /// <param name="updateAction">An action that must update to the given record</param>
    void RegisterUpdate(IMajorRecordGetter record, Action updateAction);

    /// <summary>
    /// Replaces given references of a record with with another record.
    /// </summary>
    /// <param name="record">Record to replace references for</param>
    /// <param name="references">References to record to edit</param>
    /// <param name="replacingRecord">Record to replace references with</param>
    void ReplaceReferences(IMajorRecordGetter record, IEnumerable<IFormLinkIdentifier> references, IMajorRecordGetter replacingRecord);

    /// <summary>
    /// Replaces all references of a record with with another record.
    /// </summary>
    /// <param name="record">Record to replace references for</param>
    /// <param name="replacingRecord">Record to replace references with</param>
    void ReplaceReferences(IReferencedRecord record, IMajorRecordGetter replacingRecord);

    /// <summary>
    /// Can be subscribed to in order to call a function before and after record changes
    /// </summary>
    JoinedObservable<IMajorRecordGetter> RecordChangedDiff { get; }

    /// <summary>
    /// Emits a record whenever an existing record was changed
    /// </summary>
    IObservable<IMajorRecordGetter> RecordChanged { get; }

    /// <summary>
    /// Emits a record whenever an existing record was created
    /// </summary>
    IObservable<IMajorRecordGetter> RecordCreated { get; }

    /// <summary>
    /// Emits a record whenever an existing record was deleted
    /// </summary>
    IObservable<IMajorRecordGetter> RecordDeleted { get; }
}
